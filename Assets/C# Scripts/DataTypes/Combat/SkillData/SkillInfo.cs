


[System.Serializable]
public struct SkillInfo
{
    public string Name;
    public string Description;

    public static SkillInfo Default => new SkillInfo()
    {
        Name = "New Skill",
        Description = "You shouldve entered some skill info here..."
    };
}