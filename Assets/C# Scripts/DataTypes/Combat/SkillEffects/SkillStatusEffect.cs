using UnityEngine;


/// <summary>
/// SkillEffect that adds a <see cref="StatusEffectInstance"/> to the skill's attack
/// </summary>
[System.Serializable]
public class SkillStatusEffect : SkillBaseEffect
{
    [Header("Status effect applied to the defender")]
    public StatusEffectInstance ToApplyStatusEffect = new StatusEffectInstance(StatusEffectType.Burning, 1);

    public override void Resolve(DefenseAbsorptionParameters absorptionParams)
    {
        CombatTurnContext.Defender.AddStatusEffect(ToApplyStatusEffect);
    }
}