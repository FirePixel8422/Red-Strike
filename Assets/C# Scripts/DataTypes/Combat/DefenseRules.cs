


[System.Serializable]
public struct DefenseRules
{
    public DefenseAbsorptionParameters Dodge;
    public DefenseAbsorptionParameters Parry;
    public DefenseAbsorptionParameters PerfectParry;
}

[System.Serializable]
public struct DefenseAbsorptionParameters
{
    public float DamageAbsorptionPercent;
    public bool AbsorbStatusEffects;

    public DefenseAbsorptionParameters(float damageAbsorptionPercent, bool absorbStatusEffects)
    {
        DamageAbsorptionPercent = damageAbsorptionPercent;
        AbsorbStatusEffects = absorbStatusEffects;
    }
}