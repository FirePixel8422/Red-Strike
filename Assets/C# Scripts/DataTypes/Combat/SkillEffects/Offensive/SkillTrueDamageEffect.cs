using UnityEngine;


/// <summary>
/// SkillEffect that adds true damage to the skill's attack.
/// </summary>
[System.Serializable]
public class SkillTrueDamageEffect : SkillOffsensiveEffectBase
{
    [Header("True (unblockable) damage done to the defender")]
    [SerializeField] private float trueDamage = 10;

    public override void Resolve(DefenseAbsorptionParameters absorptionParams)
    {
        CombatTurnContext.Defender.TakeDamage(
            trueDamage *
            CombatTurnContext.Attacker.GetDamageDealtMultiplier() *
            CombatTurnContext.Defender.GetDamageReceivedMultiplier());
    }
}