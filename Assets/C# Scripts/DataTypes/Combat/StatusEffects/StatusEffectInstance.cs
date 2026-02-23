using UnityEngine;


[System.Serializable]
public struct StatusEffectInstance
{
    public StatusEffectType Type;
    [Range(0, 10)]
    public int Duration;

    public StatusEffectInstance(StatusEffectType type, int duration)
    {
        Type = type;
        Duration = duration;
    }
}