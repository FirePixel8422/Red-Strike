using UnityEngine;


[System.Serializable]
public class SkillSupport : SkillBase
{
    [Header(">>Support Handling Data<<")]
    [SerializeField] private QTEWindowParametersSO qteWindowsSO;
    public QTEWindowParameters QTEWindowParameters { get; private set; }


    [SerializeReference] public SkillSupportEffectBase[] effects;


    public void Resolve(QTEResult supportQTEResult)
    {
        int effectCount = effects.Length;
        for (int i = 0; i < effectCount; i++)
        {
            effects[i].Resolve(supportQTEResult);
        }
    }

#if UNITY_EDITOR
    public void ReloadDefenseWindowParameters(string objName)
    {
        if (qteWindowsSO == null)
        {
            DebugLogger.LogWarning("No QTEWindowParametersSO assigned to " + objName + ". Play mode will throw errors");
            return;
        }
        QTEWindowParameters = qteWindowsSO.Value;
    }
#endif
}

[System.Serializable]
public struct QTEWindowParameters
{
    [Header("Appear Timings:")]
    public float StartDelay;
    [Range(0, 4)]
    public int Count;
    public float Interval;
    public float Delay;

    [Header("Succes/Perfect Window Rules:")]
    public float SuccesWindow;
    public float PerfectWindow;
    public float PerfectPercentageRequirement;

    public static QTEWindowParameters Default => new QTEWindowParameters()
    {
        StartDelay = 1f,
        Count = 2,
        Interval = 1f,
        Delay = 0.5f,
        SuccesWindow = 0.5f,
        PerfectWindow = 0.1f,
    };
}