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
    [SerializeField] private InputActionReference qteInput;


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
            blockInput.action.performed += OnDodge;

            parryInput.action.Enable();
            parryInput.action.performed += OnParry;

            qteInput.action.Enable();
            qteInput.action.performed += OnQuickTimeEvent;
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


    #region Dodge, Parry and QTE Input

    private void OnDodge(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && DefenseWindowSystem.CanDefend)
        {
            DefenseResult result = DefenseWindowSystem.DoDefendAction(DefenseType.Dodge);

#if Enable_Debug_Systems
            StartCoroutine(DebugDefenseResult_Local(result));
            DisplayDefenseResult_ServerRPC(result);

            //StartCoroutine(DebugDefenseDurationLoop(AttackManager.defenseWindow.Dodge));
#endif
        }
    }
    private void OnParry(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && DefenseWindowSystem.CanDefend && PlayerStats.Local.Energy >= GameRules.MatchSettings.ParryEnergyCost)
        {
            PlayerStats.Local.SpendEnergy(GameRules.MatchSettings.ParryEnergyCost);

            DefenseResult result = DefenseWindowSystem.DoDefendAction(DefenseType.Parry);

#if Enable_Debug_Systems
            StartCoroutine(DebugDefenseResult_Local(result));
            DisplayDefenseResult_ServerRPC(result);

            //StartCoroutine(DebugDefenseDurationLoop(AttackManager.defenseWindow.Parry));
#endif
        }
    }
    private void OnQuickTimeEvent(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && QTESequenceSystem.CanDoQTE)
        {
            QTESequenceSystem.DoQuickTimeEvent();

#if Enable_Debug_Systems

#endif
        }
    }

    #endregion


    private void OnTurnStarted()
    {
        PlayerStats.Oponnent.RestoreEnergy(GameRules.MatchSettings.PassiveEnergyGain);
        PlayerStats.Oponnent.ApplyAndTickDownStatusEffects();
    }
    private void OnTurnEnded()
    {
        PlayerStats.Local.RestoreEnergy(GameRules.MatchSettings.PassiveEnergyGain);
        PlayerStats.Local.ApplyAndTickDownStatusEffects();

        WeaponManager.SwapToRandomWeapon();
    }

    public void UseSkill_OnNetwork(int skillId)
    {
        SkillBase skill = SkillManager.GlobalSkillList[skillId];

        if (skill is SkillAttack)
        {
            UseAttackSkill_ServerRPC(skillId);
        }
        else if (skill is SkillSupport)
        {
            UseSupportSkill_ServerRPC(skillId);
            QTESequenceSystem.StartQTESequence(skillId);
        }
    }


    #region Start Support Sequence

    [ServerRpc(RequireOwnership = false, Delivery = RpcDelivery.Reliable)]
    private void UseSupportSkill_ServerRPC(int skillId, ServerRpcParams rpcParams = default)
    {
        int attackerClientGameId = rpcParams.GetSenderClientGameId();

        StartSupportPhase_ClientRPC(skillId, RPCTargetFilters.SendToOppositeClient(attackerClientGameId));
    }
    [ClientRpc(RequireOwnership = false, Delivery = RpcDelivery.Reliable)]
    private void StartSupportPhase_ClientRPC(int skillId, ClientRpcParams rpcParams = default)
    {
        if (IsHost && RPCTargetFilters.ShouldHostSkip(rpcParams)) return;

        ResolveSkillUseCosts_Local(skillId);
    }

    #endregion


    #region Resolve QTE on attacker and defender

    public void ResolveSupportSkill_OnAttacker(int skillId, QTESequenceResult qteResult)
    {
        ResolveSupportSkill_ServerRPC(skillId, qteResult);
        ResolveSupportSkill_Local(skillId, qteResult);

        TurnManager.Instance.NextTurn_ServerRPC();

#if Enable_Debug_Systems
        StartCoroutine(DebugQTESequenceResult_Local(qteResult));
        DisplayQTESequenceResult_ServerRPC(qteResult);
#endif
    }
    [ServerRpc(RequireOwnership = false, Delivery = RpcDelivery.Reliable)]
    private void ResolveSupportSkill_ServerRPC(int skillId, QTESequenceResult qteResult, ServerRpcParams rpcParams = default)
    {
        int attackerClientGameId = rpcParams.GetSenderClientGameId();

        ResolveSupportSkill_ClientRPC(skillId, qteResult, RPCTargetFilters.SendToOppositeClient(attackerClientGameId));
    }

    [ClientRpc(RequireOwnership = false, Delivery = RpcDelivery.Reliable)]
    private void ResolveSupportSkill_ClientRPC(int skillId, QTESequenceResult qteResult, ClientRpcParams rpcParams = default)
    {
        if (IsHost && RPCTargetFilters.ShouldHostSkip(rpcParams)) return;

        ResolveSupportSkill_Local(skillId, qteResult);
    }
    private void ResolveSupportSkill_Local(int skillId, QTESequenceResult qteResult)
    {
        SkillManager.GlobalSkillList[skillId].AsSupport().Resolve(qteResult);
    }

    #endregion


    #region Start Combat Sequence

    [ServerRpc(RequireOwnership = false, Delivery = RpcDelivery.Reliable)]
    private void UseAttackSkill_ServerRPC(int skillId, ServerRpcParams rpcParams = default)
    {
        int attackerClientGameId = rpcParams.GetSenderClientGameId();

        StartDefensePhase_ClientRPC(skillId, RPCTargetFilters.SendToOppositeClient(attackerClientGameId));
    }
    [ClientRpc(RequireOwnership = false, Delivery = RpcDelivery.Reliable)]
    private void StartDefensePhase_ClientRPC(int skillId, ClientRpcParams rpcParams = default)
    {
        if (IsHost && RPCTargetFilters.ShouldHostSkip(rpcParams)) return;

        DefenseWindowSystem.StartAttackSequence(skillId);
        ResolveSkillUseCosts_Local(skillId);

        float attackImpactDelay = SkillManager.GlobalSkillList[skillId].AsAttack().AttackStartupTime;
        StartAttackAnimation_ServerRPC(attackImpactDelay);
        PlayerVisualsManager.DoAttackAnimation(attackImpactDelay);

#if Enable_Debug_Systems
        //StartCoroutine(DebugAttackImpactDelayLoop(SkillManager.GlobalSkillList[skillId].AttackStartupTime));
#endif
    }

    [ServerRpc(RequireOwnership = false, Delivery = RpcDelivery.Reliable)]
    private void StartAttackAnimation_ServerRPC(float attackImpactDelay, ServerRpcParams rpcParams = default)
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


    #region Resolve attack on defender and attacker

    public void ResolveAttack_OnDefender(int skillId, DefenseResult defenseResult)
    {
        ResolveAttack_ServerRPC(skillId, defenseResult);
        ResolveAttack_Local(skillId, defenseResult);

        TurnManager.Instance.NextTurn_ServerRPC();
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
        SkillManager.GlobalSkillList[skillId].AsAttack().Resolve(defenseResult);

        if (defenseResult == DefenseResult.PerfectParried)
        {
            // Resolve penalty skill on attacker for getting perfect parried.
            ResolvePerfectParry();
        }
    }
    private void ResolvePerfectParry()
    {
        float perfectParriedDamage = GameRules.MatchSettings.PerfectParryRules.AttackerDamageTaken;
        if (perfectParriedDamage > 0)
        {
            CombatTurnContext.Attacker.TakeDamage(perfectParriedDamage);
        }
        StatusEffectInstance perfectParriedEffect = GameRules.MatchSettings.PerfectParryRules.AttackerGainedEffect;
        if (perfectParriedEffect.Type == StatusEffectType.Bleeding || perfectParriedEffect.Duration != 0)
        {
            CombatTurnContext.Attacker.AddStatusEffect(perfectParriedEffect);
        }

        int perfectParriedEnergyGain = GameRules.MatchSettings.PerfectParryRules.DefenderEnergyGain;
        if (perfectParriedEnergyGain != 0)
        {
            CombatTurnContext.Defender.RestoreEnergy(perfectParriedEnergyGain);
        }
    }

    #endregion



    public void ResolveSkillUseCosts_Local(int skillId)
    {
        SkillCosts skillCosts = SkillManager.GlobalSkillList[skillId].Costs;
        if (skillCosts.Amount <= 0) return;

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


    public override void OnDestroy()
    {
        base.OnDestroy();

        TurnManager.TurnStarted -= OnTurnStarted;
        TurnManager.TurnEnded -= OnTurnEnded;

        blockInput.action.performed -= OnDodge;
        blockInput.action.Disable();

        parryInput.action.performed -= OnParry;
        parryInput.action.Disable();

        qteInput.action.performed -= OnQuickTimeEvent;
        qteInput.action.Disable();
    }


#if Enable_Debug_Systems
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
    private IEnumerator DebugDefenseResult_Local(DefenseResult result)
    {
        MultiInstanceText.Instances[2].Text.text = result.ToString();

        yield return new WaitForSeconds(1.5f);
        
        MultiInstanceText.Instances[2].Text.text = "";
    }
    private IEnumerator DebugQTESequenceResult_Local(QTESequenceResult result)
    {
        MultiInstanceText.Instances[3].Text.text = result.ToString();

        yield return new WaitForSeconds(1.5f);

        MultiInstanceText.Instances[3].Text.text = "";
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            PlayerStats.Local.RestoreEnergy(10);
            SkillUIManager.RecalculateCanAffordSkills();
        }
    }

    [ServerRpc(RequireOwnership = false, Delivery = RpcDelivery.Reliable)]
    private void DisplayDefenseResult_ServerRPC(DefenseResult result, ServerRpcParams rpcParams = default)
    {
        ulong senderClientNetworkId = rpcParams.Receive.SenderClientId;
        DisplayDefenseResult_ClientRPC(result, RPCTargetFilters.SendToOppositeClient(senderClientNetworkId));
    }
    [ClientRpc(RequireOwnership = false, Delivery = RpcDelivery.Reliable)]
    private void DisplayDefenseResult_ClientRPC(DefenseResult result, ClientRpcParams rpcParams)
    {
        if (IsHost && RPCTargetFilters.ShouldHostSkip(rpcParams)) return;
        
        StartCoroutine(DebugDefenseResult_Local(result));
    }

    [ServerRpc(RequireOwnership = false, Delivery = RpcDelivery.Reliable)]
    private void DisplayQTESequenceResult_ServerRPC(QTESequenceResult result, ServerRpcParams rpcParams = default)
    {
        ulong senderClientNetworkId = rpcParams.Receive.SenderClientId;
        DisplayQTESequenceResult_ClientRPC(result, RPCTargetFilters.SendToOppositeClient(senderClientNetworkId));
    }
    [ClientRpc(RequireOwnership = false, Delivery = RpcDelivery.Reliable)]
    private void DisplayQTESequenceResult_ClientRPC(QTESequenceResult result, ClientRpcParams rpcParams)
    {
        if (IsHost && RPCTargetFilters.ShouldHostSkip(rpcParams)) return;

        StartCoroutine(DebugQTESequenceResult_Local(result));
    }
#endif
}
