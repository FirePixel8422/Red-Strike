using Fire_Pixel.Utility;
using UnityEngine;
using UnityEngine.InputSystem;


/// <summary>
/// Quick Time Event UI Manager. Handles the UI popups through <see cref="QTEUIBlock"/>s for quick time event sequences, which are used for support skills.
/// </summary> 
[RequireComponent(typeof(ForceStateOnLoad))]
public class QTEUIManager : MonoBehaviour
{
    public static QTEUIManager Instance { get; private set; }

    [SerializeField] private InputActionReference qteInput;
    [SerializeField] private float qteAnimRemovalMultiplier = 0.25f;
    [SerializeField] private float qteGlobalReactionTime = 0.25f;
    public float QTEGlobalReactionTime => qteGlobalReactionTime;

#pragma warning disable UDR0001
    private QTEUIBlock[] qteUIBlocks;
    private DefenseUIBlock defenseUIBlock;
#pragma warning restore UDR0001



    private void Awake()
    {
        Instance = this;
        qteUIBlocks = GetComponentsInChildren<QTEUIBlock>(true);
        defenseUIBlock = GetComponentInChildren<DefenseUIBlock>(true);

        int qteCount = qteUIBlocks.Length;
        for (int i = 0; i < qteCount; i++)
        {
            qteUIBlocks[i].Init(qteInput);
        }
        defenseUIBlock.Init();
    }
    
    public void StartQTESequence(QTESequenceParameters qteSequenceParams, float[] randomStartDelays)
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
    public void StartCombatQTE(float qteDuration, DefenseWindowParameters defenseWindows)
    {
        defenseUIBlock.Activate(qteDuration, QTEGlobalReactionTime, defenseWindows);
    }

    public void SucceedQTE(int index)
    {
        qteUIBlocks[index].SucceedQTE();
    }
    public void FailQTE(int index, bool isFailedBecauseExpired)
    {
        qteUIBlocks[index].FailQTE(isFailedBecauseExpired);
    }

    public void SucceedCombatQTE()
    {
        defenseUIBlock.SucceedQTE();
    }
    public void FailCombatQTE(bool isFailedBecauseExpired)
    {
        defenseUIBlock.FailQTE(isFailedBecauseExpired);
    }
    public void DisableAll(QTESequenceParameters qteSequenceParams, float[] randomStartDelays)
    {
        int qteCount = qteSequenceParams.Length;
        for (int i = 0; i < qteCount; i++)
        {
            float removeDelay = randomStartDelays[i] * Instance.qteAnimRemovalMultiplier;
            CallbackScheduler.Invoke(removeDelay, qteUIBlocks[i].Disable, QTESequenceSystem.INVOKE_SYSTEMS_ID_HASH);
        }
    }
    public void DisableCombatQTE(float randomStartDelay)
    {
        float removeDelay = randomStartDelay * Instance.qteAnimRemovalMultiplier;
        CallbackScheduler.Invoke(removeDelay, defenseUIBlock.Disable, DefenseWindowSystem.INVOKE_SYSTEMS_ID_HASH);
    }

    private void OnDestroy()
    {
        CallbackScheduler.CancelAllInvokesInGroup(QTESequenceSystem.INVOKE_SYSTEMS_ID_HASH);
        CallbackScheduler.CancelAllInvokesInGroup(DefenseWindowSystem.INVOKE_SYSTEMS_ID_HASH);
    }


#if Enable_Debug_Systems
    public QTESequenceParametersSO testQTESequenceParams;

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.Y))
        {
            QTESequenceSystem.DebugStartQTESequence(testQTESequenceParams);
        }
    }
#endif
}