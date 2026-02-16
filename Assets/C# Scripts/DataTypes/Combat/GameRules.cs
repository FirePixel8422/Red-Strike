


/// <summary>
/// Static class holding the core game setup settings and rules on how the game works.
/// </summary>
public static class GameRules
{
#pragma warning disable UDR0001
    public static DefaultPlayerStatsSO DefaultPlayerStats { get; private set; }
    public static StatusEffectRules StatusEffects { get; private set; }
    public static DefenseRules DefenseStrengths { get; private set; }
    public static MatchSettings MatchSettings { get; private set; }
#pragma warning restore UDR0001

    public static void SetGameRules(DefaultPlayerStatsSO defaultPlayerStats, StatusEffectRules newStatusRules, DefenseRules newDefenseRules, MatchSettings matchSettings)
    {
        DefaultPlayerStats = defaultPlayerStats;
        StatusEffects = newStatusRules;
        DefenseStrengths = newDefenseRules;
        MatchSettings = matchSettings;
    }

    public static BaseStatusEffectRules GetStatusApplyOptions(StatusEffectType statusType)
    {
        return statusType switch
        {
            StatusEffectType.Burning => StatusEffects.Burning,
            StatusEffectType.Bleeding => StatusEffects.Bleeding,
            StatusEffectType.Broken => StatusEffects.Broken,
            StatusEffectType.Empowered => StatusEffects.Empowered,
            StatusEffectType.Weakened => StatusEffects.Weakened,
            StatusEffectType.Vulnerable => StatusEffects.Vulnerability,
            _ => null,
        };
    }
    public static DefenseAbsorptionParameters GetDefenseAbsorptionParams(DefenseResult defenseResult)
    {
        return defenseResult switch
        {
            DefenseResult.Blocked => DefenseStrengths.Block,
            DefenseResult.Parried => DefenseStrengths.Parry,
            DefenseResult.PerfectParried => DefenseStrengths.PerfectParry,
            DefenseResult.None or _ => new DefenseAbsorptionParameters(0, false),
        };
    }
}