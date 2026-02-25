using Fire_Pixel.Networking;
using System;
using UnityEngine;
using UnityEngine.InputSystem;


public class SkillUIManager : MonoBehaviour
{
    [SerializeField] private InputActionReference[] skillQuickUseInputs;

    private Action<InputAction.CallbackContext>[] skillUseActions;

#pragma warning disable UDR0001
    private static SkillUIBlock[] skillUIBlocks;
    private static TooltipHandler toolTipHandler;
#pragma warning restore UDR0001


    private void Awake()
    {
        skillUIBlocks = GetComponentsInChildren<SkillUIBlock>(true);
        toolTipHandler = GetComponent<TooltipHandler>();

        UpdateSkillUIActiveState(false);

        MatchManager.PostMatchStarted += OnGameStart;
        TurnManager.TurnStarted += OnTurnStarted;

        int skillCount = skillQuickUseInputs.Length;
        skillUseActions = new Action<InputAction.CallbackContext>[skillCount];

        RebindManager.PostRebindsLoaded += () =>
        {
            for (int i = 0; i < skillCount; i++)
            {
                // local copy for closure
                int skillSlotId = i;

                skillUseActions[i] = CreateSkillUseAction(skillSlotId);

                skillQuickUseInputs[i].action.performed += skillUseActions[skillSlotId];
                skillQuickUseInputs[i].action.Enable();
            }
        };
    }
    private Action<InputAction.CallbackContext> CreateSkillUseAction(int skillSlotId)
    {
        return ctx =>
        {
            if (ctx.performed == false) return;

            skillUIBlocks[skillSlotId].TryUseSkill();
        };
    }

    private void OnGameStart()
    {
        int skillSlotCount = skillUIBlocks.Length;
        for (int i = 0; i < skillSlotCount; i++)
        {
            skillUIBlocks[i].Init();
        }
        UpdateSkillUIActiveState(TurnManager.IsMyTurn);
    }

    private void OnTurnStarted() => UpdateSkillUIActiveState(true);


    #region Manage Skill UI

    public static void UpdateSkillUIActiveState(bool state)
    {
        int skillSlotCount = skillUIBlocks.Length;
        for (int i = 0; i < skillSlotCount; i++)
        {
            skillUIBlocks[i].UpdateSkillActiveState(state);
        }
    }
    public static void UpdateSkillUI(SkillSet skillSet)
    {
        int skillSlotCount = skillUIBlocks.Length;
        if (skillSet.Length > skillSlotCount)
        {
            // Randomize order so a weapon with more skills then there are skillslots, chooses random skills to fill the slots
            skillSet.RandomizeSkillOrder();
        }

        for (int i = 0; i < skillSlotCount; i++)
        {
            skillUIBlocks[i].UpdateUI(skillSet[i]);
        }
        // Update tooltip systems
        toolTipHandler.UpdateColoredWords();
    }
    public static void RecalculateCanAffordSkills()
    {
        int skillSlotCount = skillUIBlocks.Length;
        for (int i = 0; i < skillSlotCount; i++)
        {
            skillUIBlocks[i].RecalculateCanAffordSkill();
        }
    }

    #endregion


    private void OnDestroy()
    {
        TurnManager.TurnStarted -= OnTurnStarted;

        int skillCount = skillQuickUseInputs.Length;
        for (int i = 0; i < skillCount; i++)
        {
            InputActionReference reference = skillQuickUseInputs[i];
            if (reference == null) continue;

            InputAction action = reference.action;
            if (action == null) continue;

            action.performed -= skillUseActions[i];
            action.Disable();
        }
    }
}
