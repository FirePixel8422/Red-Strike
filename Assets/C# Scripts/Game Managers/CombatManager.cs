using Fire_Pixel.Networking;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;


public class CombatManager : SmartNetworkBehaviour
{
    public static CombatManager Instance { get; private set; }

    [SerializeField] private PlayerStats[] playerStats;

    [SerializeField] private InputActionReference blockInput;
    [SerializeField] private InputActionReference parryInput;
    private bool canDefend;



    private void Awake()
    {
        Instance = this;

        playerStats = new PlayerStats[GlobalGameData.MAX_PLAYERS];
        for (int i = 0; i < GlobalGameData.MAX_PLAYERS; i++)
        {
            playerStats[i] = GameRules.DefaultPlayerStats.GetStatsCopy();
        }
        CombatTurnContext.Init(playerStats);

        RebindManager.PostRebindsLoaded += () =>
        {
            blockInput.action.Enable();
            blockInput.action.performed += OnBlock;

            parryInput.action.Enable();
            parryInput.action.performed += OnParry;
        };
    }

    protected override void OnNetworkSystemsSetupPostStart()
    {
        PlayerStats.Local = CombatTurnContext.Players[LocalClientGameId];
        PlayerStats.Oponnent = CombatTurnContext.Players[LocalClientGameId == 0 ? 1 : 0];

        for (int i = 0; i < GlobalGameData.MAX_PLAYERS; i++)
        {
            CombatTurnContext.Players[i].UpdateHealthBar();
            CombatTurnContext.Players[i].UpdateEnergyBar();
        }

        WeaponManager.SwapToRandomWeapon();

        TurnManager.TurnStarted += OnTurnStarted;
        TurnManager.TurnEnded += OnTurnEnded;
    }

    private void OnTurnStarted()
    {
        PlayerStats.Local.RestoreEnergy(GameRules.MatchSettings.PassiveEnergyGain);
        PlayerStats.Oponnent.ApplyAndTickDownStatusEffects();
    }
    private void OnTurnEnded()
    {
        PlayerStats.Oponnent.RestoreEnergy(GameRules.MatchSettings.PassiveEnergyGain);
        PlayerStats.Local.ApplyAndTickDownStatusEffects();

        WeaponManager.SwapToRandomWeapon();
    }

    [SerializeField] private GameObject blockCube, parryCube;
    [SerializeField] private Transform spawnpoint;

    private void OnBlock(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && canDefend)
        {
            canDefend = false;
            AttackManager.DoDefendAction(DefenseType.Block);
            StartCoroutine(DebugDefenseDurationLoop(AttackManager.defenseWindow.Block));
        }
    }
    private void OnParry(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && canDefend)
        {
            canDefend = false;
            AttackManager.DoDefendAction(DefenseType.Parry);
            StartCoroutine(DebugDefenseDurationLoop(AttackManager.defenseWindow.Parry));
        }
    }

    private IEnumerator DebugDefenseDurationLoop(float defenseDuration)
    {
        while (defenseDuration > 0)
        {
            defenseDuration -= Time.deltaTime;
            MultiInstanceText.Instances[0].Text.text = "Defense:" + (Mathf.Round(defenseDuration * 100) / 100).ToString();

            yield return null;
        }

        MultiInstanceText.Instances[0].Text.text = "";
    }


    #region Start Combat Sequence

    [ServerRpc(RequireOwnership = false, Delivery = RpcDelivery.Reliable)]
    public void Attack_ServerRPC(int skillId, ServerRpcParams rpcParams = default)
    {
        int attackerClientGameId = rpcParams.GetSenderClientGameId();

        StartDefensePhase_ClientRPC(skillId, RPCTargetFilters.SendToOppositeClient(attackerClientGameId));
    }
    [ClientRpc(RequireOwnership = false, Delivery = RpcDelivery.Reliable)]
    private void StartDefensePhase_ClientRPC(int skillId, ClientRpcParams rpcParams = default)
    {
        if (IsHost && RPCTargetFilters.ShouldHostSkip(rpcParams)) return;

        AttackManager.StartAttackSequence(skillId);
        ResolveSkillUseCosts_Attacker(skillId);

        canDefend = true;

        float attackImpactDelay = SkillManager.GlobalSkillList[skillId].AttackStartupTime;
        StartAttackAnimation_ServerRPC(attackImpactDelay);
        PlayerVisualsManager.DoAttackAnimation(attackImpactDelay);
        
        StartCoroutine(DebugAttackImpactDelayLoop(SkillManager.GlobalSkillList[skillId].AttackStartupTime));
    }
    private IEnumerator DebugAttackImpactDelayLoop(float impactDelay)
    {
        while (impactDelay > 0)
        {
            impactDelay -= Time.deltaTime;
            MultiInstanceText.Instances[1].Text.text = "Impact:" + (Mathf.Round(impactDelay * 100) / 100).ToString();

            yield return null;
        }

        MultiInstanceText.Instances[1].Text.text = "";
    }
    public void ResolveSkillUseCosts_Attacker(int skillId)
    {
        SkillCosts skillCosts = SkillManager.GlobalSkillList[skillId].Costs;
        if (skillCosts.Amount > 0)
        {
            switch (skillCosts.Type)
            {
                case PlayerResourceType.Health:
                    CombatTurnContext.Attacker.TakeDamage(skillCosts.Amount);
                    break;

                case PlayerResourceType.Energy:
                    CombatTurnContext.Attacker.SpendEnergy(skillCosts.Amount);
                    break;

                default:
                    break;
            }
        }
    }

    [ServerRpc(RequireOwnership = false, Delivery = RpcDelivery.Reliable)]
    public void StartAttackAnimation_ServerRPC(float attackImpactDelay, ServerRpcParams rpcParams = default)
    {
        int attackerClientGameId = rpcParams.GetSenderClientGameId();

        StartAttackAnimation_ClientRPC(attackImpactDelay, RPCTargetFilters.SendToOppositeClient(attackerClientGameId));
    }
    [ClientRpc(RequireOwnership = false, Delivery = RpcDelivery.Reliable)]
    private void StartAttackAnimation_ClientRPC(float attackImpactDelay, ClientRpcParams rpcParams = default)
    {
        if (IsHost && RPCTargetFilters.ShouldHostSkip(rpcParams)) return;

        PlayerVisualsManager.DoAttackAnimation(attackImpactDelay);
    }

    #endregion


    #region Resolve Attack on defender and attacker

    public void ResolveAttack_OnDefender(int skillId, DefenseResult defenseResult)
    {
        canDefend = false;

        ResolveAttack_ServerRPC(skillId, defenseResult);
        ResolveAttack_Local(skillId, defenseResult);

        TurnManager.Instance.EndTurn_ServerRPC();
    }
    [ServerRpc(RequireOwnership = false, Delivery = RpcDelivery.Reliable)]
    private void ResolveAttack_ServerRPC(int skillId, DefenseResult defenseResult, ServerRpcParams rpcParams = default)
    {
        int attackerClientGameId = rpcParams.GetSenderClientGameId();

        ResolveAttack_ClientRPC(skillId, defenseResult, RPCTargetFilters.SendToOppositeClient(attackerClientGameId));
    }

    [ClientRpc(RequireOwnership = false, Delivery = RpcDelivery.Reliable)]
    private void ResolveAttack_ClientRPC(int skillId, DefenseResult defenseResult, ClientRpcParams rpcParams = default)
    {
        if (IsHost && RPCTargetFilters.ShouldHostSkip(rpcParams)) return;

        ResolveAttack_Local(skillId, defenseResult);
    }
    private void ResolveAttack_Local(int skillId, DefenseResult defenseResult)
    {
        SkillManager.GlobalSkillList[skillId].Resolve(defenseResult);

        if (defenseResult == DefenseResult.PerfectParried)
        {
            // Resolve penalty skill on attacker for getting perfect parried.
            CombatTurnContext.Attacker.TakeDamage(GameRules.MatchSettings.PerfectParryPenalty.DamageTaken);
            CombatTurnContext.Attacker.AddStatusEffect(GameRules.MatchSettings.PerfectParryPenalty.VulnerableEffect);
        }

        //DebugLogger.LogError("Defender Health" + CombatTurnContext.Defender.Health);
    }

    #endregion


    public override void OnDestroy()
    {
        base.OnDestroy();

        TurnManager.TurnStarted -= OnTurnStarted;
        TurnManager.TurnEnded -= OnTurnEnded;

        blockInput.action.performed -= OnBlock;
        blockInput.action.Disable();

        parryInput.action.performed -= OnParry;
        parryInput.action.Disable();
    }
}
