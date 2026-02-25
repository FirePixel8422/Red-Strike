using UnityEngine;


[System.Serializable]
public abstract class SkillBase
{
    public int Id { get; private set; }
    public void SetId(int id)
    {
        Id = id;
    }

    [SerializeField] private SkillInfo info = SkillInfo.Default;
    [SerializeField] private SkillCosts costs = SkillCosts.Default;
    public SkillInfo Info => info;
    public SkillCosts Costs => costs;


    /// <summary>
    /// Loads SO data into skill.
    /// </summary>
    public virtual void Init() { }

    /// <summary>
    /// Casts to  <see cref="SkillAttack"/>
    /// </summary>
    public SkillAttack AsAttack() => this as SkillAttack;
    /// <summary>
    /// Casts to <see cref="SkillSupport"/>
    /// </summary>
    public SkillSupport AsSupport() => this as SkillSupport;


#if UNITY_EDITOR
    public void SetSkillInfo(SkillInfo newInfo)
    {
        info = newInfo;
    }
    public virtual void DebugValidateSkillData(string objName) { }
#endif
}

[System.Serializable]
public struct SkillInfo
{
    public string Name;
    [TextArea]
    public string Description;

    public static SkillInfo Default => new SkillInfo()
    {
        Name = "New Skill",
        Description = "You shouldve entered some skill info here..."
    };
}

[System.Serializable]
public struct SkillCosts
{
    public PlayerResourceType Type;
    public int Amount;

    public static SkillCosts Default => new SkillCosts()
    {
        Type = PlayerResourceType.Energy,
        Amount = 0,
    };
}