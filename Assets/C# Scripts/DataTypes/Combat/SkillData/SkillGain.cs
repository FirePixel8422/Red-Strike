


[System.Serializable]
public struct SkillGain
{
    public int EnergyGain;
    public int HealthGain;

    public static SkillGain Default => new SkillGain()
    {
        EnergyGain = 1,
        HealthGain = 0,
    };
}