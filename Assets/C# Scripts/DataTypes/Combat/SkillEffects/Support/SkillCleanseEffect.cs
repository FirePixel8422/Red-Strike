using UnityEngine;


/// <summary>
/// SkillEffect that cleanses all negative stats of the attacker.
/// </summary>
[System.Serializable]
public class SkillCleanseEffect : SkillSupportEffectBase
{
    [Header("Cleanses all bad statusEffects on the attacker")]
    [SerializeField] private QTEResultBinding<bool> doCleanse;

    public override void Resolve(QTESequenceResult supportQTEResult)
    {
        bool shouldCleanse = doCleanse.GetValue(supportQTEResult);
        if (shouldCleanse == false) return;

        CombatTurnContext.Attacker.CleanseStatusEffects();
    }
}