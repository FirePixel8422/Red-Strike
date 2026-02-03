


[System.Serializable]
public struct SkillCosts
{
    public int EnergyCost;
    public int HealthCost;

    public static SkillCosts Default => new SkillCosts()
    {
        EnergyCost = 1,
        HealthCost = 0,
    };
}
