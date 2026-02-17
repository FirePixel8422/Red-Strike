using Fire_Pixel.Networking;
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

        blockInput.action.performed += OnBlock;
        parryInput.action.performed += OnParry;

        RebindManager.RebindsLoaded += () =>
        {
            blockInput.action.Enable();
            parryInput.action.Enable();
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
        PlayerStats.Local.ApplyAndTickDownStatusEffects();
        PlayerStats.Local.RestoreEnergy(GameRules.MatchSettings.PassiveEnergyGain);
    }
    private void OnTurnEnded()
    {
        PlayerStats.Oponnent.ApplyAndTickDownStatusEffects();
        WeaponManager.SwapToRandomWeapon();
    }

    private void OnBlock(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && canDefend)
        {
            canDefend = false;
            AttackManager.DoDefendAction(DefenseType.Block);
        }
    }
    private void OnParry(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && canDefend)
        {
            canDefend = false;
            AttackManager.DoDefendAction(DefenseType.Parry);
        }
    }


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
    }


    #region Resolve Attack on defender and attacker

    public void ResolveAttack_OnDefender(int skillId, DefenseResult defenseResult)
    {
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

        DebugLogger.LogError("Defender Health" + CombatTurnContext.Defender.Health);
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
