


[System.Serializable]
public struct SkillStats
{
    public float damage;
    public ParryWindowParameters parryParamaters;

    public static SkillStats Default => new SkillStats()
    {
        damage = 10,
        parryParamaters = new ParryWindowParameters(0.3f, 0.1f),
    };
}