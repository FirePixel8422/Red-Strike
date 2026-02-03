using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;


[System.Serializable]
public class PlayerStats
{
    public float Health;
    public float Energy;


    private List<StatusEffect> effectsList = new List<StatusEffect>();


    #region Status Effects

    public void AddStatusEffect(StatusEffect statusEffect)
    {
        BaseStatusEffectRules statusApplyRules = GameRules.GetStatusApplyOptions(statusEffect.Type);

        switch (statusApplyRules.StackMode)
        {
            case StatusEffectStackMode.Independent:
                effectsList.Add(statusEffect);
                break;

            case StatusEffectStackMode.RefreshDuration:
                if (TryGetEffect(statusEffect.Type, out StatusEffect existingEffect))
                {
                    existingEffect.Duration = math.max(existingEffect.Duration, statusEffect.Duration);
                }
                break;

            case StatusEffectStackMode.CombineDuration:
                if (TryGetEffect(statusEffect.Type, out existingEffect))
                {
                    existingEffect.Duration += statusEffect.Duration;
                }
                break;

            case StatusEffectStackMode.Skip:
            default: break;
        }

    }
    public void TickDownEffects()
    {
        int effectCount = effectsList.Count;
        for (int i = 0; i < effectCount; i++)
        {
            int cDuration = effectsList[i].Duration;
            cDuration -= 1;

            if (cDuration <= 0)
            {
                effectsList.RemoveAtSwapBack(i);
                i--;
                continue;
            }
            effectsList[i] = new StatusEffect(effectsList[i].Type, cDuration);
        }
    }

    public bool TryGetEffect(StatusEffectType effectType, out StatusEffect effect)
    {
        int effectCount = effectsList.Count;
        for (int i = 0; i < effectCount; i++)
        {
            if (effectsList[i].Type == effectType)
            {
                effect = effectsList[i];
                return true;
            }
        }
        effect = default;
        return false;
    }

    public bool IsBroken => TryGetEffect(StatusEffectType.Broken, out _);

    public float GetDamageOutputMultiplier()
    {
        float multiplier = 1;
        multiplier *= GetEffectMultiplier(StatusEffectType.Empowered, GameRules.StatusEffectRules.Empowered.Multiplier);
        multiplier *= GetEffectMultiplier(StatusEffectType.Weakened, GameRules.StatusEffectRules.Weakened.Multiplier);

        return multiplier;
    }
    public float GetDamageReceivedMultiplier()
    {
        return GetEffectMultiplier(StatusEffectType.Vulnerable, GameRules.StatusEffectRules.Vulnerability.Multiplier);
    }

    private float GetEffectMultiplier(StatusEffectType effectType, float multiplier)
    {
        if (GetEffectStackCount(effectType, out int stacks))
        {
            float addedEffect = 0f;

            for (int i = 1; i <= stacks; i++)
            {
                float stackEffect = (multiplier > 1f ? (multiplier - 1f) : (1f - multiplier)) / (i * i);
                addedEffect += stackEffect;
            }

            return multiplier > 1f ? 1f + addedEffect : 1f - addedEffect;
        }
        return 1;
    }
    private bool GetEffectStackCount(StatusEffectType effectType, out int stacks)
    {
        int effectCount = effectsList.Count;
        stacks = 0;

        for (int i = 0; i < effectCount; i++)
        {
            if (effectsList[i].Type == effectType)
            {
                stacks += 1;
            }
        }
        return stacks > 0;
    }

    #endregion
}