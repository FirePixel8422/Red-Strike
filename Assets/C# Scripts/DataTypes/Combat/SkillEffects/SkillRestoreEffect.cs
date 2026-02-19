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

    public override void Resolve(DefenseAbsorptionParameters absorptionParams)
    {
        switch (type)
        {
            case PlayerResourceType.Health:
                CombatTurnContext.Attacker.Heal(amount);
                break;

            case PlayerResourceType.Energy:
                CombatTurnContext.Attacker.RestoreEnergy(Mathf.RoundToInt(amount));
                break;

            default:
                break;
        }
    }
}