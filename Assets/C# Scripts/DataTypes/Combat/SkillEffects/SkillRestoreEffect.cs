using UnityEngine;


/// <summary>
/// SkillEffect that restores stats of the attacker.
/// </summary>
[System.Serializable]
public class SkillRestoreEffect : SkillBaseEffect
{
    [Header("Restore done to the attacker")]
    [SerializeField] private PlayerResourceType type;
    [SerializeField] private float amount;

    public override void Resolve(CombatContext ctx, DefenseAbsorptionParameters absorptionParams)
    {
        switch (type)
        {
            case PlayerResourceType.Health:
                ctx.Attacker.Heal(amount);
                break;

            case PlayerResourceType.Energy:
                ctx.Attacker.RestoreEnergy(amount);
                break;

            default:
                break;
        }
    }
}