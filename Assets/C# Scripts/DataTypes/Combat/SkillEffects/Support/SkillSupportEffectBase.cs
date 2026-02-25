using UnityEngine;


/// <summary>
/// Base class for modular effects that run on the attacker when a skill is used.
/// Skill effects operate on <see cref="CombatTurnContext"/> to modify damage, apply status effects, or execute additional combat logic.
/// </summary>
[System.Serializable]
public abstract class SkillSupportEffectBase
{
    public virtual void Resolve(QTESequenceResult supportQTEResult) { }
}

/// <summary>
/// SkillEffect container that holds different values for each possible QTE result, and returns the correct one based on the result of the QTE.
/// </summary>
[System.Serializable]
public struct QTEResultBinding<T>
{
    [SerializeField] private T FailedQTE, NoneQTE, SuccesfulQTE, PerfectQTE;

    public readonly T GetValue(QTESequenceResult result)
    {
        return result switch
        {
            QTESequenceResult.Success => SuccesfulQTE,
            QTESequenceResult.Perfect => PerfectQTE,
            QTESequenceResult.Failed or _ => NoneQTE,
        };
    }


#if UNITY_EDITOR
    public T[] AsArray
    {
        get => new T[] { NoneQTE, SuccesfulQTE, PerfectQTE };
        set
        {
            NoneQTE = value[0];
            SuccesfulQTE = value[1];
            PerfectQTE = value[2];
        }
    }
#endif
}