using UnityEngine;


/// <summary>
/// SkillEffect that adds true damage to the skill's attack.
/// </summary>
[System.Serializable]
public class SkillTrueDamageEffect : SkillBaseEffect
{
    [Header("True damage done to the defender")]
    public float trueDamage = 10;

    public override void Resolve(CombatContext ctx, DefenseAbsorptionParameters absorptionParams)
    {
        ctx.Defender.TakeDamage(
            trueDamage *
            ctx.Attacker.GetDamageDealtMultiplier() *
            ctx.Defender.GetDamageReceivedMultiplier());
    }
}