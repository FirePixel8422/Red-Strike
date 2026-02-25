using Unity.Mathematics;
using UnityEngine;


[System.Serializable]
public class SkillSupport : SkillBase
{
    [Header(">>Support Handling Data<<")]
    [SerializeField] private QTESequenceParametersSO qteWindowsSO;
    public QTESequenceParameters QTESequenceParameters { get; private set; }


    [SerializeReference] public SkillSupportEffectBase[] effects;


    public override void Init()
    {
        QTESequenceParameters = qteWindowsSO.Value;
    }
    public void Resolve(QTESequenceResult supportQTEResult)
    {
        int effectCount = effects.Length;
        for (int i = 0; i < effectCount; i++)
        {
            effects[i].Resolve(supportQTEResult);
        }
    }

#if UNITY_EDITOR
    public override void DebugValidateSkillData(string objName)
    {
        if (qteWindowsSO == null)
        {
            DebugLogger.LogWarning("No QTEWindowParametersSO assigned to " + objName + ". Play mode will throw errors");
        }
    }
#endif
}

[System.Serializable]
public struct QTESequenceParameters
{
    public QTEParameters[] QuickTimeEvents;

    public static QTESequenceParameters Default => new QTESequenceParameters()
    {
        QuickTimeEvents = new QTEParameters[]
        {
            QTEParameters.Default,
            QTEParameters.Default,
        }
    };

    public QTEParameters this[int i] => QuickTimeEvents[i];
    public int Length => QuickTimeEvents.Length;
}

[System.Serializable]
public struct QTEParameters
{
    public MinMaxFloat StartDelayRange;
    [Range(0, 1.5f)]
    public float Duration;
    [Range(0, 1)]
    public float SuccesWindow01;

    /// <summary>
    /// Time in seconds to hit the QTE succesfully
    /// </summary>
    public readonly float SuccesWindowTime => Duration * SuccesWindow01;


    public static QTEParameters Default => new QTEParameters()
    {
        StartDelayRange = new MinMaxFloat(0.5f, 0.5f),
        Duration = 0.5f,
        SuccesWindow01 = 0.25f,
    };
}