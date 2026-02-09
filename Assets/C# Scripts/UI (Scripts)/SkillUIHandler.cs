using UnityEngine;

public class SkillUIHandler : MonoBehaviour
{
    private static SkillUIBlock[] skillUIBlocks;
    private static TooltipHandler toolTipHandler;


    private void Awake()
    {
        skillUIBlocks = GetComponentsInChildren<SkillUIBlock>(true);
    }

    public static void UpdateSkillUI(Weapon weapon)
    {
        int skillCount = weapon.Skills.Length;
        for (int i = 0; i < skillCount; i++)
        {
            skillUIBlocks[i].UpdateUI(weapon.Skills[i].Skill.Info);
        }
        // Update tooltip systems
        toolTipHandler.UpdateColoredWords();
    }

    public static void UpdateSkillsActiveState(bool isActive)
    {
        int skillCount = skillUIBlocks.Length;
        for (int i = 0; i < skillCount; i++)
        {
            skillUIBlocks[i].UpdateSkillActiveState(isActive);
        }
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Q))
        {
            UpdateSkillsActiveState(true);
        }
        if (Input.GetKeyUp(KeyCode.E))
        {
            UpdateSkillsActiveState(false);
        }
    }
}
