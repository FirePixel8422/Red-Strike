using Fire_Pixel.Networking;
using UnityEngine;


public class SkillUIHandler : MonoBehaviour
{
#pragma warning disable UDR0001
    private static SkillUIBlock[] skillUIBlocks;
    private static TooltipHandler toolTipHandler;
#pragma warning restore UDR0001


    private void Awake()
    {
        skillUIBlocks = GetComponentsInChildren<SkillUIBlock>(true);
        toolTipHandler = GetComponent<TooltipHandler>();

        UpdateSkillUIActiveState(TurnState.Ended);

        TurnManager.TurnChanged += OnGameStart;
        TurnManager.TurnStateChanged += UpdateSkillUIActiveState;

        DebugLogger.LogError("DEBUG TEST");
    }
    private void OnGameStart(int clientOnTurnGameId)
    {
        TurnState turnState = clientOnTurnGameId == ClientManager.LocalClientGameId ?
            TurnState.Started :
            TurnState.Ended;

        DebugLogger.LogError("GaME sTARTED");
        DebugLogger.LogError("You first", turnState == TurnState.Started);

        UpdateSkillUIActiveState(turnState);
        TurnManager.TurnChanged -= OnGameStart;
    }


    private void UpdateSkillUIActiveState(TurnState state)
    {
        bool isActive = state == TurnState.Started;

        int skillCount = skillUIBlocks.Length;
        for (int i = 0; i < skillCount; i++)
        {
            skillUIBlocks[i].UpdateSkillActiveState(isActive);
        }
    }

    public static void UpdateSkillUI(Weapon weapon)
    {
        int skillCount = weapon.Skills.Length;
        for (int i = 0; i < skillCount; i++)
        {
            skillUIBlocks[i].UpdateUI(weapon.Skills[i].Skill);
        }
        // Update tooltip systems
        toolTipHandler.UpdateColoredWords();
    }

    private void OnDestroy()
    {
        TurnManager.TurnStateChanged -= UpdateSkillUIActiveState;
        TurnManager.TurnChanged -= OnGameStart;
    }
}
