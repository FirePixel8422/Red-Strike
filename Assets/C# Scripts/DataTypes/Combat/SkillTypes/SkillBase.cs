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
    [SerializeField] private string animationName;
    public SkillInfo Info => info;
    public SkillCosts Costs => costs;
    public int AnimationNameHash { get; private set; }


    /// <summary>
    /// Loads SO data into skill and sets up/creates data for skill usage.
    /// </summary>
    public virtual void Init()
    {
        AnimationNameHash = Animator.StringToHash(animationName);
    }

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
    public void SetSkillData(string animationName)
    {
        this.animationName = animationName;
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