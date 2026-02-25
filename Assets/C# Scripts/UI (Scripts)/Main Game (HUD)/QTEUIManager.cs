using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;



public class QTEUIManager : MonoBehaviour
{
    [SerializeField] private InputActionReference qteInput;
    [SerializeField] private float qteAnimRemovalInterval = 0.5f;
    [SerializeField] private QTESequenceParametersSO DEBUG_QTESequenceSO;

#pragma warning disable UDR0001
    private static QTEUIBlock[] qteUIBlocks;
#pragma warning restore UDR0001




    private void Awake()
    {
        qteUIBlocks = GetComponentsInChildren<QTEUIBlock>(true);

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

            ExtensionMethods.Invoke(NetworkManager.Singleton, randomStartDelays[capturedI], () =>
            {
                qteUIBlocks[capturedI].Activate(qteDuration, qteWindow); 
            });
        }
    }

    public static void SucceedQTE(int index)
    {
        qteUIBlocks[index].SucceedQTE();
    }
    public static void FailQTE(int index)
    {
        qteUIBlocks[index].FailQTE();
    }
    public static void DisableAll(QTESequenceParameters qteSequenceParams, float[] randomStartDelays)
    {
        int qteCount = qteSequenceParams.Length;
        for (int i = 0; i < qteCount; i++)
        {
            float removeDelay = randomStartDelays[i] * 0.5f;
            ExtensionMethods.Invoke(NetworkManager.Singleton, removeDelay, qteUIBlocks[i].Disable);
        }
    }


#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            QTESequenceSystem.DEBUG_StartQTESequence(DEBUG_QTESequenceSO.Value);
        }
    }
#endif
}