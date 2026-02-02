



[System.Serializable]
public struct StatusEffect
{
    public StatusEffectType Type;
    public int Duration;

    public StatusEffect(StatusEffectType type, int duration)
    {
        Type = type;
        Duration = duration;
    }
}