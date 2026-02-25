using UnityEngine;


[System.Serializable]
public class SkillAttack : SkillBase
{
    [Header(">>Attack Handling Data<<")]
    [SerializeField] private DefenseWindowParametersSO defenseWindowsSO;
    public DefenseWindowParameters DefenseWindows { get; private set; }
    [SerializeField] private float attackStartupTime;


    [SerializeReference] public SkillOffsensiveEffectBase[] effects;
    public float AttackStartupTime => attackStartupTime;


    public override void Init()
    {
        DefenseWindows = defenseWindowsSO.Value;
    }
    public void Resolve(DefenseResult defenseResult)
    {
        DefenseAbsorptionParameters defenseAbsorptionParams = GameRules.GetDefenseAbsorptionParams(defenseResult);

        int effectCount = effects.Length;
        for (int i = 0; i < effectCount; i++)
        {
            effects[i].Resolve(defenseAbsorptionParams);
        }
    }

#if UNITY_EDITOR
    public override void DebugValidateSkillData(string objName)
    {
        if (defenseWindowsSO == null)
        {
            DebugLogger.LogWarning("No DefenseWindowParametersSO assigned to " + objName + ". Play mode will throw errors");
        }
    }
#endif
}

[System.Serializable]
public struct DefenseWindowParameters
{
    public float Dodge;
    public float Parry;
    public float PerfectParry;

    public static DefenseWindowParameters Default => new DefenseWindowParameters()
    {
        Dodge = 0.4f,
        Parry = 0.25f,
        PerfectParry = 0.1f,
    };
}