using System.Collections.Generic;
using Unity.Collections;


[System.Serializable]
public class PlayerStats
{
    public float Health;
    public float Energy;


    #region Status Effects

    private List<StatusEffect> effectsList = new List<StatusEffect>();

    public bool CheckForEffect(StatusEffectType effectType)
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
    public void AddStatusEffect(StatusEffect statusEffect)
    {
        effectsList.Add(statusEffect);
    }

    public bool IsBurning => CheckForEffect(StatusEffectType.Fire);
    public bool IsBleeding => CheckForEffect(StatusEffectType.Bleeding);
    public bool IsWeakened => CheckForEffect(StatusEffectType.Weakened);
    public bool IsVulnerable => CheckForEffect(StatusEffectType.Vulnerable);
    public bool IsBroken => CheckForEffect(StatusEffectType.Broken);

    #endregion
}