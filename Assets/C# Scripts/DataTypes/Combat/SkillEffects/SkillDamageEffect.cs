using UnityEngine;


/// <summary>
/// SkillEffect that adds damage to the skill's attack.
/// </summary>
[System.Serializable]
public class SkillDamageEffect : SkillBaseEffect
{
    [Header("Damage dealt to the defender")]
    public float damage = 10;

    public override void Resolve(CombatContext ctx, DefenseAbsorptionParameters absorptionParams)
    {
        ctx.Defender.TakeDamage( 
            damage *
            ctx.Attacker.GetDamageDealtMultiplier() *
            ctx.Defender.GetDamageReceivedMultiplier() *
            (1 - absorptionParams.DamageAbsorptionPercent));
    }
}