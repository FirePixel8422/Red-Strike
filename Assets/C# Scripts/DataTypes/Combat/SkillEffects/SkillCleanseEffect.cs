using UnityEngine;


/// <summary>
/// SkillEffect that restores stats of the attacker.
/// </summary>
[System.Serializable]
public class SkillCleanseEffect : SkillBaseEffect
{
#if UNITY_EDITOR
    [Header("Cleanses all bad statusEffects on the attacker")]
    [Tooltip("Does nothing, just need it for the info tab...")]
    [SerializeField] private int Header_Dummy;
#endif

    public override void Resolve(DefenseAbsorptionParameters absorptionParams)
    {
        CombatTurnContext.Attacker.CleanseStatusEffects();
    }
}