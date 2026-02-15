using UnityEngine;


/// <summary>
/// SkillEffect that restores stats of the attacker.
/// </summary>
[System.Serializable]
public class SkillRestoreEffect : SkillBaseEffect
{
    [Header("Restore done to the attacker")]
    [SerializeField] private int ToAddHealth;
    [SerializeField] private int ToAddEnergy;

    public override void Resolve(CombatContext ctx, DefenseAbsorptionParameters absorptionParams)
    {
        ctx.Attacker.Heal(ToAddHealth);
        ctx.Attacker.RestoreEnergy(ToAddEnergy);
    }
}