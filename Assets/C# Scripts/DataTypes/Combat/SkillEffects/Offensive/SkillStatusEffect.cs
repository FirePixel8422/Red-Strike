using UnityEngine;


/// <summary>
/// SkillEffect that adds a <see cref="StatusEffectStack"/> to the skill's attack
/// </summary>
[System.Serializable]
public class SkillStatusEffect : SkillOffsensiveEffectBase
{
    [Header("Status effect applied to the defender")]
    [SerializeField] private StatusEffectStack toApplyStatusEffect = new StatusEffectStack(new (StatusEffectType.Burning, 1), 1);

    public override void Resolve(DefenseAbsorptionParameters absorptionParams)
    {
        if (absorptionParams.AbsorbStatusEffects) return;

        int effectStackCount = toApplyStatusEffect.StackCount;
        for (int i = 0; i < effectStackCount; i++)
        {
            CombatTurnContext.Defender.AddStatusEffect(toApplyStatusEffect.EffectInstance);
        }
    }

#if UNITY_EDITOR
    public StatusEffectStack ToApplyStatusEffect
    {
        get => toApplyStatusEffect;
        set => toApplyStatusEffect = value;
    }
#endif
}