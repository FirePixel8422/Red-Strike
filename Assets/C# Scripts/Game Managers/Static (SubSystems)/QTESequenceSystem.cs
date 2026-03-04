using Fire_Pixel.Utility;
using Unity.Mathematics;
using UnityEngine;


public static class QTESequenceSystem
{
#pragma warning disable UDR0001
    private static QTEInstance[] qteInstances;
    private static int currentIndex;

    private static int skillId;
    private static int succesfulQTECount;

    public static bool CanDoQTE => qteInstances.IsNotNullOrEmpty() && currentIndex < qteInstances.Length;

    public static readonly int INVOKE_SYSTEMS_ID_HASH = "QTE_System".GetHashCode();
#pragma warning restore UDR0001


    public static void StartQTESequence(int supportSkillId)
    {
        if (currentIndex < qteInstances?.Length)
        {
            DebugLogger.LogWarning("Trying to start QTE Sequence while another is still active. This is not supported and will cause issues. Ignoring command.");
            return;
        }


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

            // Create QTE Instance config
            randomStartDelays[i] = EzRandom.Range(cQTEParams.StartDelayRange);
            qteActivationGlobalUTime = globalTime + randomStartDelays[i] + cQTEParams.Duration + QTEUIManager.QTEGlobalReactionTime;

            // Create new QTE Instance
            qteInstances[i] = new QTEInstance(qteActivationGlobalUTime, cQTEParams.Duration, cQTEParams.SuccesWindowTime);

            // Expire QTE Instance after delay
            float expireTime = qteActivationGlobalUTime;
            CallbackScheduler.Invoke(expireTime - globalTime, () => ExpireQTEInstance(capturedIndex));
        }
        float sequenceEndTime = qteInstances[qteCount - 1].ActivationTime + qteSequenceParams[qteCount - 1].Duration;
        float totalQTESequenceDuration = sequenceEndTime - globalTime;

        skillId = supportSkillId;
        succesfulQTECount = 0;

        QTEUIManager.StartQTESequence(qteSequenceParams, randomStartDelays);
        CallbackScheduler.Invoke(totalQTESequenceDuration, () =>
        {
            ResolveQTESequence();
            QTEUIManager.DisableAll(qteSequenceParams, randomStartDelays);
        }, INVOKE_SYSTEMS_ID_HASH);
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
        else if (succesfulQTECount >= Mathf.CeilToInt(qteCount * 0.5f))
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
            QTEUIManager.FailQTE(index, true);
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

        QTEUIManager.FailQTE(currentIndex, false);
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






    public static void DebugStartQTESequence(QTESequenceParametersSO qteSo)
    {
        QTESequenceParameters qteSequenceParams = qteSo.Value;

        int qteCount = qteSequenceParams.Length;
        float[] randomStartDelays = new float[qteCount];
        if (qteCount == 0)
        {
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

            // Create QTE Instance config
            randomStartDelays[i] = EzRandom.Range(cQTEParams.StartDelayRange);
            qteActivationGlobalUTime = globalTime + randomStartDelays[i] + cQTEParams.Duration + QTEUIManager.QTEGlobalReactionTime;

            // Create new QTE Instance
            qteInstances[i] = new QTEInstance(qteActivationGlobalUTime, cQTEParams.Duration, cQTEParams.SuccesWindowTime);

            // Expire QTE Instance after delay
            float expireTime = qteActivationGlobalUTime;
            CallbackScheduler.Invoke(expireTime - globalTime, () => ExpireQTEInstance(capturedIndex), INVOKE_SYSTEMS_ID_HASH);
        }
        float sequenceEndTime = qteInstances[qteCount - 1].ActivationTime + qteSequenceParams[qteCount - 1].Duration;
        float totalQTESequenceDuration = sequenceEndTime - globalTime;

        succesfulQTECount = 0;

        QTEUIManager.StartQTESequence(qteSequenceParams, randomStartDelays);
        CallbackScheduler.Invoke(totalQTESequenceDuration, () =>
        {
            QTEUIManager.DisableAll(qteSequenceParams, randomStartDelays);
        }, INVOKE_SYSTEMS_ID_HASH);
    }
}