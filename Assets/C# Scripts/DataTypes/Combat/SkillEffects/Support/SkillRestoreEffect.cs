using UnityEngine;


/// <summary>
/// SkillEffect that restores resources of the attacker.
/// </summary>
[System.Serializable]
public class SkillRestoreEffect : SkillSupportEffectBase
{
    [Header("Restore done to the attacker")]
    [SerializeField] private EnumStructArray<QTESequenceResult, RestoreEffectInstance> restoreEffects;

    public override void Resolve(CombatTurnContext ctx, QTESequenceResult supportQTEResult)
    {
        RestoreEffectInstance targetEffect = restoreEffects.GetValue(supportQTEResult);
        switch (targetEffect.Type)
        {
            case PlayerResourceType.Health:
                ctx.Attacker.Heal(targetEffect.Amount);
                break;

            case PlayerResourceType.Energy:
                ctx.Attacker.RestoreEnergy(Mathf.RoundToInt(targetEffect.Amount));
                break;

            default:
                break;
        }
    }

    [System.Serializable]
    private struct RestoreEffectInstance
    {
        public PlayerResourceType Type;
        public float Amount;
    }
}