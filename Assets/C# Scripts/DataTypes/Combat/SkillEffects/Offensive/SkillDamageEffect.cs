using UnityEngine;


/// <summary>
/// SkillEffect that adds damage to the skill's attack.
/// </summary>
[System.Serializable]
public class SkillDamageEffect : SkillOffsensiveEffectBase
{
    [Header("Damage dealt to the defender")]
    [SerializeField] private float damage = 10;

    public override void Resolve(DefenseAbsorptionParameters absorptionParams)
    {
        CombatTurnContext.Defender.TakeDamage( 
            damage *
            CombatTurnContext.Attacker.GetDamageDealtMultiplier() *
            CombatTurnContext.Defender.GetDamageReceivedMultiplier() *
            (1 - absorptionParams.DamageAbsorptionPercent));
    }
}