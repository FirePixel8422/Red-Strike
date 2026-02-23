using UnityEngine;


[System.Serializable]
public struct StatusEffectStack
{
    public StatusEffectInstance EffectInstance;
    [Range(1, 10)]
    public int StackCount;

    public StatusEffectStack(StatusEffectInstance effectInstance, int stackCount)
    {
        EffectInstance = effectInstance;
        StackCount = stackCount;
    }
}