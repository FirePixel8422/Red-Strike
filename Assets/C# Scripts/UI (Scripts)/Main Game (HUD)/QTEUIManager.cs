using Fire_Pixel.Utility;
using UnityEngine;
using UnityEngine.InputSystem;


/// <summary>
/// Quick Time Event UI Manager. Handles the UI popups through <see cref="QTEUIBlock"/>s for quick time event sequences, which are used for support skills.
/// </summary>
public class QTEUIManager : MonoBehaviour
{
    public static QTEUIManager Instance { get; private set; }

    [SerializeField] private InputActionReference qteInput;
    [SerializeField] private float qteAnimRemovalMultiplier = 0.25f;
    [SerializeField] private float qteGlobalReactionTime = 0.25f;
    public static float QTEGlobalReactionTime => Instance.qteGlobalReactionTime;

#pragma warning disable UDR0001
    private static QTEUIBlock[] qteUIBlocks;
#pragma warning restore UDR0001




    private void Awake()
    {
        Instance = this;
        qteUIBlocks = GetComponentsInChildren<QTEUIBlock>();

        int qteCount = qteUIBlocks.Length;
        for (int i = 0; i < qteCount; i++)
        {
            qteUIBlocks[i].Init(qteInput);
        }
    }
    
    public static void StartQTESequence(QTESequenceParameters qteSequenceParams, float[] randomStartDelays)
    {
        int qteCount = qteSequenceParams.Length;
        for (int i = 0; i < qteCount; i++)
        {
            int capturedI = i;
            float qteDuration = qteSequenceParams[i].Duration;
            float qteWindow = qteSequenceParams[i].SuccesWindow01;

            CallbackScheduler.Invoke(randomStartDelays[capturedI], () =>
            {
                qteUIBlocks[capturedI].Activate(qteDuration, qteWindow, QTEGlobalReactionTime); 
            }, QTESequenceSystem.INVOKE_SYSTEMS_ID_HASH);
        }
    }

    public static void SucceedQTE(int index)
    {
        qteUIBlocks[index].SucceedQTE();
    }
    public static void FailQTE(int index, bool isFailedBecauseExpired)
    {
        qteUIBlocks[index].FailQTE(isFailedBecauseExpired);
    }
    public static void DisableAll(QTESequenceParameters qteSequenceParams, float[] randomStartDelays)
    {
        int qteCount = qteSequenceParams.Length;
        for (int i = 0; i < qteCount; i++)
        {
            float removeDelay = randomStartDelays[i] * Instance.qteAnimRemovalMultiplier;
            CallbackScheduler.Invoke(removeDelay, qteUIBlocks[i].Disable, QTESequenceSystem.INVOKE_SYSTEMS_ID_HASH);
        }
    }

    private void OnDestroy()
    {
        CallbackScheduler.CancelAllInvokesInGroup(QTESequenceSystem.INVOKE_SYSTEMS_ID_HASH);
    }


    public QTESequenceParametersSO testQTESequenceParams;

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.C))
        {
            QTESequenceSystem.DebugStartQTESequence(testQTESequenceParams);
        }
    }
}