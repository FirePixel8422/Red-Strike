using UnityEngine;


/// <summary>
/// SkillEffect that adds a <see cref="StatusEffectStack"/> to the skill's support action
/// </summary>
[System.Serializable]
public class SkillEmpowerEffect : SkillSupportEffectBase
{
    [Header("Status effect applied to the attacker")]
    [SerializeField] private QTEResultBinding<StatusEffectStack> toApplyStatusEffect;

    public override void Resolve(QTEResult supportQTEResult)
    {
        StatusEffectStack statusEffectStack = toApplyStatusEffect.GetValue(supportQTEResult);

        int effectStackCount = statusEffectStack.StackCount;
        for (int i = 0; i < effectStackCount; i++)
        {
            CombatTurnContext.Defender.AddStatusEffect(statusEffectStack.EffectInstance);
        }
    }

#if UNITY_EDITOR
    public QTEResultBinding<StatusEffectStack> ToApplyStatusEffect
    {
        get => toApplyStatusEffect;
        set => toApplyStatusEffect = value;
    }
#endif
}