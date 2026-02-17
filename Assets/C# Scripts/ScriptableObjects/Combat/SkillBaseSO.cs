using UnityEngine;



[CreateAssetMenu(fileName = "New Skill", menuName = "ScriptableObjects/Combat/SkillSO", order = -1005)]
public class SkillBaseSO : ScriptableObject
{
    [SerializeReference]
    public SkillBase Skill;

    private void OnValidate()
    {
        if (Application.isPlaying || Skill == null || Skill.effects.IsNullOrEmpty()) return;

        int effectCount = Skill.effects.Length;
        for (int i = 0; i < effectCount; i++)
        {
            SkillStatusEffect statusEffect = Skill.effects[i] as SkillStatusEffect;
            if (statusEffect != null &&
                statusEffect.ToApplyStatusEffect.Type == StatusEffectType.Bleeding &&
                statusEffect.ToApplyStatusEffect.Duration != 0)
            {
                StatusEffectInstance effectInstance = statusEffect.ToApplyStatusEffect;
                effectInstance.Duration = 0;
                statusEffect.ToApplyStatusEffect = effectInstance;

                DebugLogger.Log("Bleeding status effect always has a duration of 0, since it doesnt go away unless you heal");
            }   
        }
    }
}