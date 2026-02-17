using UnityEngine;


/// <summary>
/// SkillEffect that adds true damage to the skill's attack.
/// </summary>
[System.Serializable]
public class SkillTrueDamageEffect : SkillBaseEffect
{
    [Header("True damage done to the defender")]
    public float trueDamage = 10;

    public override void Resolve(DefenseAbsorptionParameters absorptionParams)
    {
        CombatTurnContext.Defender.TakeDamage(
            trueDamage *
            CombatTurnContext.Attacker.GetDamageDealtMultiplier() *
            CombatTurnContext.Defender.GetDamageReceivedMultiplier());
    }
}