using UnityEngine;


/// <summary>
/// SkillEffect that adds true damage to the skill's attack.
/// </summary>
[System.Serializable]
public class SkillTrueDamageEffect : SkillOffsensiveEffectBase
{
    [Header("True (unblockable) damage done to the defender")]
    [SerializeField] private float trueDamage = 10;

    public override void Resolve(CombatTurnContext ctx, DefenseAbsorptionParameters absorptionParams)
    {
        ctx.Defender.TakeDamage(
            trueDamage *
            ctx.Attacker.GetDamageDealtMultiplier() *
            ctx.Defender.GetDamageReceivedMultiplier());
    }
}