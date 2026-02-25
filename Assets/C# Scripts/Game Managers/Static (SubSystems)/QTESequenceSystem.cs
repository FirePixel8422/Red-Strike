using System;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;


public static class QTESequenceSystem
{
#pragma warning disable UDR0001
    private static QTEInstance[] qteInstances;
    private static int currentIndex;

    private static int skillId;
    private static int succesfulQTECount;

    public static bool CanDoQTE => qteInstances.IsNotNullOrEmpty() && currentIndex < qteInstances.Length;
#pragma warning restore UDR0001


    public static void StartQTESequence(int supportSkillId)
    {
        SkillSupport skill = SkillManager.GlobalSkillList[supportSkillId].AsSupport();
        QTESequenceParameters qteSequenceParams = skill.QTESequenceParameters;

        int qteCount = qteSequenceParams.Length;
        float[] randomStartDelays = new float[qteCount];
        if (qteCount == 0)
        {
            CombatManager.Instance.ResolveSupportSkill_OnAttacker(skillId, QTESequenceResult.Failed);
            return;
        }

        qteInstances = new QTEInstance[qteCount];
        currentIndex = 0;

        float globalTime = Time.unscaledTime;
        float qteActivationGlobalUTime;
        for (int i = 0; i < qteCount; i++)
        {
            int capturedIndex = i;
            QTEParameters cQTEParams = qteSequenceParams[i];

            randomStartDelays[i] = EzRandom.Range(cQTEParams.StartDelayRange);
            qteActivationGlobalUTime = globalTime + randomStartDelays[i] + cQTEParams.Duration;

            qteInstances[i] = new QTEInstance(qteActivationGlobalUTime, cQTEParams.Duration, cQTEParams.SuccesWindowTime);

            float expireTime = qteActivationGlobalUTime + cQTEParams.SuccesWindowTime;
            ExtensionMethods.Invoke(NetworkManager.Singleton, expireTime - globalTime, () => ExpireQTEInstance(capturedIndex));
        }
        float sequenceEndTime = qteInstances[qteCount - 1].ActivationTime + qteSequenceParams[qteCount - 1].Duration;
        float totalQTESequenceDuration = sequenceEndTime - globalTime;

        skillId = supportSkillId;
        succesfulQTECount = 0;

        QTEUIManager.StartQTESequence(qteSequenceParams, randomStartDelays);
        ExtensionMethods.Invoke(NetworkManager.Singleton, totalQTESequenceDuration, () =>
        {
            ResolveQTESequence();
            QTEUIManager.DisableAll(qteSequenceParams, randomStartDelays);
        });
    }
    private static void ResolveQTESequence()
    {
        if (CombatManager.Instance == null) return;

        QTESequenceResult qteResult = QTESequenceResult.Failed;
        int qteCount = qteInstances.Length;
        if (succesfulQTECount == qteCount)
        {
            qteResult = QTESequenceResult.Perfect;
        }
        else if (succesfulQTECount > math.ceil(qteCount * 0.5f))
        {
            qteResult = QTESequenceResult.Success;
        }
        CombatManager.Instance.ResolveSupportSkill_OnAttacker(skillId, qteResult);
    }

    private static void ExpireQTEInstance(int index)
    {
        // If QTE isnt already mistimed failed or well timed suceeded, fail it.
        if (currentIndex == index)
        {
            DebugLogger.Log("FailedQTE");
            QTEUIManager.FailQTE(index);
            currentIndex += 1;
        }
    }

    public static void DoQuickTimeEvent()
    {
        if (currentIndex >= qteInstances.Length) return;

        bool qteActive = qteInstances[currentIndex].IsActive(Time.unscaledTime, out bool succesfulQTE);

        if (qteActive == false) return;
        if (succesfulQTE)
        {
            QTEUIManager.SucceedQTE(currentIndex);
            DebugLogger.Log("SuccesQTE");

            currentIndex += 1;
            succesfulQTECount += 1;
            return;
        }

        QTEUIManager.FailQTE(currentIndex);
        DebugLogger.Log("FailedQTE");
        currentIndex += 1;
    }


    private struct QTEInstance
    {
        public float ActivationTime;
        public float ActiveWindow;
        public float TimingWindow;

        public QTEInstance(float activationTime, float activeWindow, float timingWindow)
        {
            ActivationTime = activationTime;
            ActiveWindow = activeWindow;
            TimingWindow = timingWindow;
        }

        public readonly bool IsActive(float globalTime, out bool succesfulQTE)
        {
            succesfulQTE = TimingWindow > math.distance(globalTime, ActivationTime);
            return ActiveWindow > math.distance(globalTime, ActivationTime);
        }
    }


#if UNITY_EDITOR
    public static void DEBUG_StartQTESequence(QTESequenceParameters qteSequenceParams)
    {
        int qteCount = qteSequenceParams.Length;
        float[] randomStartDelays = new float[qteCount];
        if (qteCount == 0)
        {
            CombatManager.Instance.ResolveSupportSkill_OnAttacker(skillId, QTESequenceResult.Failed);
            return;
        }

        qteInstances = new QTEInstance[qteCount];
        currentIndex = 0;

        float globalTime = Time.unscaledTime;
        float qteActivationGlobalUTime;
        for (int i = 0; i < qteCount; i++)
        {
            int capturedIndex = i;
            QTEParameters cQTEParams = qteSequenceParams[i];

            randomStartDelays[i] = EzRandom.Range(cQTEParams.StartDelayRange);
            qteActivationGlobalUTime = globalTime + randomStartDelays[i] + cQTEParams.Duration;

            qteInstances[i] = new QTEInstance(qteActivationGlobalUTime, cQTEParams.Duration, cQTEParams.SuccesWindowTime);

            float expireTime = qteActivationGlobalUTime + cQTEParams.SuccesWindowTime;
            ExtensionMethods.Invoke(NetworkManager.Singleton, expireTime - globalTime, () => ExpireQTEInstance(capturedIndex));
        }
        float sequenceEndTime = qteInstances[qteCount - 1].ActivationTime + qteSequenceParams[qteCount - 1].Duration;
        float totalQTESequenceDuration = sequenceEndTime - globalTime;

        succesfulQTECount = 0;

        QTEUIManager.StartQTESequence(qteSequenceParams, randomStartDelays);
        ExtensionMethods.Invoke(NetworkManager.Singleton, totalQTESequenceDuration, () =>
        {
            QTEUIManager.DisableAll(qteSequenceParams, randomStartDelays);
        });
    }
#endif
}