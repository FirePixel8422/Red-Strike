using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;


[System.Serializable]
public class PlayerStats
{
    public static PlayerStats Local { get; set; }


    public float[] Resources;
    public float Health
    {
        get => Resources[(int)PlayerResourceType.Health];
        private set => Resources[(int)PlayerResourceType.Health] = value;
    }
    public float Energy
    {
        get => Resources[(int)PlayerResourceType.Energy];
        private set => Resources[(int)PlayerResourceType.Energy] = value;
    }

    public PlayerStats(float health, int energy)
    {
        Resources = new float[2];
        Health = health;
        Energy = energy;
    }


    #region Update Resources and auto update UI bars

    public void Heal(float amount)
    {
        DebugLogger.LogError("amount MUST be > 0", amount <= 0f);
        for (int i = effectsList.Count - 1; i >= 0; i--)
        {
            if (effectsList[i].Type == StatusEffectType.Bleeding)
            {
                effectsList.RemoveAtSwapBack(i);
            }
        }

        Health = math.clamp(Health + amount, 0, GameRules.DefaultPlayerStats.MaxHealth);
        UpdateHealthBar();
    }
    public void TakeDamage(float amount)
    {
        DebugLogger.LogError("amount MUST be > 0", amount <= 0f);

        Health -= amount;
        UpdateHealthBar();
    }

    public void RestoreEnergy(float amount)
    {
        DebugLogger.LogError("amount MUST be > 0", amount <= 0f);

        Energy = math.clamp(Energy + amount, 0, GameRules.DefaultPlayerStats.MaxEnergy);
        UpdateEnergyBar();
    }
    public void SpendEnergy(float amount)
    {
        DebugLogger.LogError("amount MUST be > 0", amount <= 0f);

        Energy -= amount;
        UpdateEnergyBar();
    }

    public void UpdateHealthBar()
    {
        float healthPercent01 = Health / GameRules.DefaultPlayerStats.MaxHealth;

        ResourceBarUI healthBar = this == Local ? HUDHandler.Instance.LocalHealthBar : HUDHandler.Instance.OpponentHealthBar;
        healthBar?.UpdateBar(healthPercent01);
    }
    public void UpdateEnergyBar()
    {
        float energyPercent01 = math.round(Energy / GameRules.DefaultPlayerStats.MaxEnergy * 10) * 0.1f;

        // other client doesn’t update your energy bar
        if (this == Local)
        { 
            HUDHandler.Instance.LocalEnergyBar.UpdateBar(energyPercent01);
        }
    }

    #endregion


    [SerializeField] private List<StatusEffectInstance> effectsList = new List<StatusEffectInstance>();
    public List<StatusEffectInstance> EffectsList => effectsList;



    #region Status Effects

    /// <summary>
    /// Add status effect of type <paramref name="statusEffect"/> to player.
    /// </summary>
    public void AddStatusEffect(StatusEffectInstance statusEffect)
    {
        BaseStatusEffectRules statusApplyRules = GameRules.GetStatusApplyOptions(statusEffect.Type);

        switch (statusApplyRules.StackMode)
        {
            case StatusEffectStackMode.Independent:
                effectsList.Add(statusEffect);
                break;

            case StatusEffectStackMode.RefreshDuration:
                if (TryGetEffect(statusEffect.Type, out StatusEffectInstance existingEffect, out int effectId))
                {
                    int newDuration = math.max(existingEffect.Duration, statusEffect.Duration);
                    effectsList.ModifyAt(effectId, (ref StatusEffectInstance effect) => effect.Duration = newDuration);
                }
                else
                {
                    effectsList.Add(statusEffect);
                }
                break;

            case StatusEffectStackMode.CombineDuration:
                if (TryGetEffect(statusEffect.Type, out existingEffect, out effectId))
                {
                    int newDuration = existingEffect.Duration + statusEffect.Duration;
                    effectsList.ModifyAt(effectId, (ref StatusEffectInstance effect) => effect.Duration = newDuration);
                }
                else
                {
                    effectsList.Add(statusEffect);
                }
                break;

            case StatusEffectStackMode.Skip:
                if (HasEffect(statusEffect.Type))
                {
                    effectsList.Add(statusEffect);
                }
                break;

            default: break;
        }
    }

    /// <summary>
    /// Tick down the duration off all current status effect on player and apply the damage of the bleeding and burning status effect
    /// </summary>
    public void ApplyAndTickDownStatusEffects()
    {
        int effectCount = effectsList.Count;
        for (int i = effectCount - 1; i >= 0; i--)
        {
            // Bleeding doesnt go away by itself
            if (effectsList[i].Type == StatusEffectType.Bleeding) continue;

            int newDuration = effectsList[i].Duration;
            newDuration -= 1;

            if (newDuration <= 0)
            {
                effectsList.RemoveAtSwapBack(i);
                continue;
            }

            effectsList.ModifyAt(i, (ref StatusEffectInstance effect) => effect.Duration = newDuration);
        }

        float fireDamage = CalculateEffectStrength(StatusEffectType.Burning, GameRules.StatusEffects.Burning.StrengthRules);
        float bleedDamage = CalculateEffectStrength(StatusEffectType.Bleeding, GameRules.StatusEffects.Bleeding.StrengthRules);

        Health -= (fireDamage + bleedDamage);
    }

    /// <summary>
    /// Cleanse all stacks of every bad status effect of current effectlist
    /// </summary>
    public void CleanseStatusEffects()
    {
        int effectCount = effectsList.Count;
        for (int i = effectCount - 1; i >= 0; i--)
        {
            StatusEffectType type = effectsList[i].Type;
            
            // Cleanse only bad status effects
            if (type != StatusEffectType.Empowered)
            {
                effectsList.RemoveAtSwapBack(i);
            }
        }
    }


    /// <returns>Wheather player has <paramref name="effectType"/> status effect, that corresponding <see cref="StatusEffectInstance"/> and the effectId in the effectList</returns>
    public bool TryGetEffect(StatusEffectType effectType, out StatusEffectInstance effect, out int effectId)
    {
        int effectCount = effectsList.Count;
        for (int i = 0; i < effectCount; i++)
        {
            if (effectsList[i].Type == effectType)
            {
                effect = effectsList[i];
                effectId = i;
                return true;
            }
        }
        effect = default;
        effectId = -1;
        return false;
    }
    /// <returns>Wheather player has <paramref name="effectType"/> status effect
    public bool HasEffect(StatusEffectType effectType)
    {
        int effectCount = effectsList.Count;
        for (int i = 0; i < effectCount; i++)
        {
            if (effectsList[i].Type == effectType)
            {
                return true;
            }
        }
        return false;
    }

    /// <returns>Does player have the <see cref="StatusEffectType.Broken"/> effect</returns>
    public bool IsBroken => HasEffect(StatusEffectType.Broken);


    /// <returns>Damage multiplier based on all active statusEffects that affect damage output</returns>
    public float GetDamageDealtMultiplier()
    {
        return 1 +
            CalculateEffectStrength(StatusEffectType.Empowered, GameRules.StatusEffects.Empowered.StrengthRules) -
            CalculateEffectStrength(StatusEffectType.Weakened, GameRules.StatusEffects.Weakened.StrengthRules);
    }

    /// <returns>Damage received multiplier based on all active statusEffects that affect damage received</returns>
    public float GetDamageReceivedMultiplier()
    {
        return 1 + CalculateEffectStrength(StatusEffectType.Vulnerable, GameRules.StatusEffects.Vulnerability.StrengthRules);
    }


    /// <returns>EffectStrength based on current stacks of that effect and <see cref="EffectStackRules"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private float CalculateEffectStrength(StatusEffectType effectType, EffectStackRules strengthRules)
    {
        if (!GetEffectStackCount(effectType, out int stacks))
        {
            return 0;
        }

        float strength = 0;
        float strengthMultiplier = 1;

        // Every stack multiplies its power by decayMultiplier after the first stack
        for (int i = 0; i < stacks; i++)
        {
            strength += strengthRules.StrengthPerStack * strengthMultiplier;
            strengthMultiplier *= strengthRules.StackDecayMultiplier;
        }

        return strength;
    }

    /// <returns>Get total stacks of <paramref name="effectType"/> effect on player</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool GetEffectStackCount(StatusEffectType effectType, out int stacks)
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