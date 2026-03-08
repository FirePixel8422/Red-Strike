using Fire_Pixel.Networking;
using Fire_Pixel.Utility;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class SkillUIManager : MonoBehaviour
{
    public static SkillUIManager Instance { get; private set; }


    [SerializeField] private InputActionReference[] skillQuickUseInputs;
    [SerializeField] private TextMeshProUGUI weaponNameText;

    [SerializeField] private float fadeOutTime;
    [SerializeField] private float fadeInTime;
    [SerializeField] private Image screenBlock;

    private CanvasGroup canvasGroup;
    private Action<InputAction.CallbackContext>[] skillUseActions;

#pragma warning disable UDR0001
    private static SkillUIBlock[] skillUIBlocks;
    private static TooltipHandler toolTipHandler;
#pragma warning restore UDR0001


    private void Awake()
    {
        Instance = this;

        canvasGroup = GetComponent<CanvasGroup>();
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
    public static void UpdateSkillUI(WeaponSkillSetData skillSet)
    {
        Instance.weaponNameText.text = skillSet.WeaponName;

        int skillSlotCount = skillUIBlocks.Length;
        if (skillSet.Length > skillSlotCount)
        {
            // Randomize order so a weapon with more skills then there are skillslots, chooses random skills to fill the slots
            skillSet.RandomizeSkillOrder();
        }

        for (int i = 0; i < skillSlotCount; i++)
        {
            skillUIBlocks[i].UpdateUI(skillSet.SkillData[i]);
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


    #region SkillUI Fade In/Out

    public static void FadeIn()
    {
        Instance.screenBlock.enabled = false;
        CallbackScheduler.RegisterUpdate(Instance.FadeInSequence);
    }
    public static void FadeOut()
    {
        Instance.screenBlock.enabled = true;
        CallbackScheduler.RegisterUpdate(Instance.FadeOutSequence);
    }
    private void FadeInSequence()
    {
        float alpha = canvasGroup.alpha;
        canvasGroup.alpha = Mathf.MoveTowards(alpha, 1, Time.deltaTime / fadeInTime);

        if (alpha == 1)
        {
            CallbackScheduler.UnRegisterUpdate(FadeInSequence);
        }
    }
    private void FadeOutSequence()
    {
        float alpha = canvasGroup.alpha;
        canvasGroup.alpha = Mathf.MoveTowards(alpha, 0, Time.deltaTime / fadeOutTime);

        if (alpha == 0)
        {
            CallbackScheduler.UnRegisterUpdate(FadeOutSequence);
        }
    }

    #endregion


    private void OnDestroy()
    {
        TurnManager.TurnStarted -= OnTurnStarted;

        CallbackScheduler.UnRegisterUpdate(FadeInSequence);
        CallbackScheduler.UnRegisterUpdate(FadeOutSequence);

        int skillCount = skillQuickUseInputs.Length;
        for (int i = 0; i < skillCount; i++)
        {
            InputActionReference reference = skillQuickUseInputs[i];
            if (reference == null) continue;

            InputAction action = reference.action;
            if (action == null) continue;

            Action<InputAction.CallbackContext> useAction = skillUseActions[i];
            if (useAction == null) continue;

            action.performed -= useAction;
            action.Disable();
        }
    }
}
