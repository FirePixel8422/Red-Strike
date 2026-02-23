using UnityEngine;



[CreateAssetMenu(fileName = "New Skill", menuName = "ScriptableObjects/Combat/SkillSO", order = -1005)]
public class SkillBaseSO : ScriptableObject
{
    [SerializeReference]
    public SkillBase Skill;


#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Application.isPlaying || Skill == null) return;

        SkillInfo skillInfo = Skill.Info;
        skillInfo.Name = name;
        Skill.SetSkillInfo(skillInfo);

        ValidateSkillAttackData();
        ValidateSkillSupportData();
    }

    private void ValidateSkillAttackData()
    {
        if (Skill is not SkillAttack skillAttack || skillAttack.effects.IsNullOrEmpty()) return;

        skillAttack.ReloadDefenseWindowParameters(name);

        int effectCount = skillAttack.effects.Length;
        for (int i = 0; i < effectCount; i++)
        {
            SkillStatusEffect statusEffect = skillAttack.effects[i] as SkillStatusEffect;
            if (statusEffect != null &&
                statusEffect.ToApplyStatusEffect.EffectInstance.Type == StatusEffectType.Bleeding &&
                statusEffect.ToApplyStatusEffect.EffectInstance.Duration != 0)
            {
                StatusEffectInstance effectInstance = statusEffect.ToApplyStatusEffect.EffectInstance;
                effectInstance.Duration = 0;

                StatusEffectStack effectStack = statusEffect.ToApplyStatusEffect;
                effectStack.EffectInstance = effectInstance;

                statusEffect.ToApplyStatusEffect = effectStack;

                DebugLogger.Log("Bleeding status effect always has a duration of 0, since it doesnt go away unless you heal");
            }
        }
    }
    private void ValidateSkillSupportData()
    {
        if (Skill is not SkillSupport skillSupport || skillSupport.effects.IsNullOrEmpty()) return;

        skillSupport.ReloadDefenseWindowParameters(name);

        int effectCount = skillSupport.effects.Length;
        for (int i = 0; i < effectCount; i++)
        {
            SkillEmpowerEffect empoweredEffect = skillSupport.effects[i] as SkillEmpowerEffect;
            if (empoweredEffect != null)
            {
                StatusEffectStack[] effectStacks = empoweredEffect.ToApplyStatusEffect.AsArray;
                int effectStackCount = effectStacks.Length;

                for (int j = 0; j < effectStackCount; j++)
                {
                    StatusEffectInstance effectInstance = effectStacks[j].EffectInstance;
                    if (effectInstance.Type != StatusEffectType.Bleeding || effectInstance.Duration == 0) continue;

                    effectInstance.Duration = 0;
                    effectStacks[j].EffectInstance = effectInstance;

                    DebugLogger.Log("Bleeding status effect always has a duration of 0, since it doesnt go away unless you heal");
                }
                QTEResultBinding<StatusEffectStack> statusEffectQTEBinding = empoweredEffect.ToApplyStatusEffect;
                statusEffectQTEBinding.AsArray = effectStacks;
                empoweredEffect.ToApplyStatusEffect = statusEffectQTEBinding;
            }
        }
    }
#endif
}