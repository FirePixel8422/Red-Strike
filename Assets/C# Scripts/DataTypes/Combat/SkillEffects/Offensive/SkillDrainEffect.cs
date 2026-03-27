using UnityEngine;


/// <summary>
/// SkillEffect that drains resources of the defender.
/// </summary>
[System.Serializable]
public class SkillDrainEffect : SkillOffsensiveEffectBase
{
    [Header("Drain done to the defender")]
    [SerializeField] private PlayerResourceType type;
    [SerializeField] private float amount;

    public override void Resolve(CombatTurnContext ctx, DefenseAbsorptionParameters absorptionParams)
    {
        switch (type)
        {
            case PlayerResourceType.Health:
                ctx.Attacker.TakeDamage(amount);
                break;

            case PlayerResourceType.Energy:
                ctx.Attacker.SpendEnergy(Mathf.RoundToInt(amount));
                break;

            default:
                break;
        }
    }
}