using UnityEngine;


[System.Serializable]
public class SkillBase
{
    public int Id { get; private set; }
    public void SetId(int id)
    {
        Id = id;
    }

    public SkillInfo Info = SkillInfo.Default;
    public SkillStats Stats = SkillStats.Default;
    public SkillCosts Costs = SkillCosts.Default;
    public SkillGain Gain = SkillGain.Default;

    [SerializeReference] public SkillEffectBase[] Effects;

    public virtual void Execute()
    {

    }
}