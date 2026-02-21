using Fire_Pixel.Networking;
using UnityEngine;


public class SkillUIManager : MonoBehaviour
{
    public static SkillUIManager Instance { get; private set; }

#pragma warning disable UDR0001
    private static SkillUIBlock[] skillUIBlocks;
    private static TooltipHandler toolTipHandler;
#pragma warning restore UDR0001


    private void Awake()
    {
        Instance = this;

        skillUIBlocks = GetComponentsInChildren<SkillUIBlock>(true);
        toolTipHandler = GetComponent<TooltipHandler>();

        UpdateSkillUIActiveState(false);

        TurnManager.TurnChanged += OnGameStart;
        TurnManager.TurnStarted += OnTurnStarted;
    }
    private void OnGameStart(int clientOnTurnGameId)
    {
        TurnManager.TurnChanged -= OnGameStart;

        int skillSlotCount = skillUIBlocks.Length;
        for (int i = 0; i < skillSlotCount; i++)
        {
            skillUIBlocks[i].Init();
        }

        UpdateSkillUIActiveState(TurnManager.IsMyTurn);
    }

    private void OnTurnStarted() => UpdateSkillUIActiveState(true);
    public void UpdateSkillUIActiveState(bool state)
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

    private void OnDestroy()
    {
        TurnManager.TurnChanged -= OnGameStart;
        TurnManager.TurnStarted -= OnTurnStarted;
    }
}
