



public static class GameRules
{
#pragma warning disable UDR0001
    public static StatusEffectRules StatusEffectRules;
#pragma warning restore UDR0001

    public static BaseStatusEffectRules GetStatusApplyOptions(StatusEffectType statusType)
    {
        return statusType switch
        {
            StatusEffectType.Burning => StatusEffectRules.Burning,
            StatusEffectType.Bleeding => StatusEffectRules.Bleeding,
            StatusEffectType.Broken => StatusEffectRules.Broken,
            StatusEffectType.Empowered => StatusEffectRules.Empowered,
            StatusEffectType.Weakened => StatusEffectRules.Weakened,
            StatusEffectType.Vulnerable => StatusEffectRules.Vulnerability,
            _ => null,
        };
    }
}