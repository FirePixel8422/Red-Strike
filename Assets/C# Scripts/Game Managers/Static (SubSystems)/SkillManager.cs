


public static class SkillManager
{
    public static SkillBase[] GlobalSkillList { get; private set; }

    public static void Init(GlobalSkillListSO globalSkillListSO)
    {
        int skillCount = globalSkillListSO.SkillList.Length;
        GlobalSkillList = new SkillBase[skillCount];

        for (int i = 0; i < skillCount; i++)
        {
            SkillBase skill = globalSkillListSO.SkillList[i].Skill;
            skill.SetId(i);

            GlobalSkillList[i] = skill;
        }
    }
}
