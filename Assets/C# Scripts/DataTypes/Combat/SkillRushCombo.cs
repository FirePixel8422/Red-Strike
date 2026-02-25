


[System.Serializable]
public struct SkillRushCombo
{
    public int[] SkillIds;


    public SkillRushCombo(params int[] skillIds)
    {
        SkillIds = skillIds;
    }
}