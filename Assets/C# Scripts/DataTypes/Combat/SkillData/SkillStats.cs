


[System.Serializable]
public struct SkillStats
{
    public float damage;
    public DefenseWindowParameters defenseWindows;

    public static SkillStats Default => new SkillStats()
    {
        damage = 10,
        defenseWindows = new DefenseWindowParameters(0.3f, 0.15f, 0.05f),
    };
}