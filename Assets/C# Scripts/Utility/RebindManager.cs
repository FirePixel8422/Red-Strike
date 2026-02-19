using Fire_Pixel.Utility;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;


public class RebindManager : MonoBehaviour
{
    public static RebindManager Instance { get; private set; }

#pragma warning disable UDR0001
    public static OneTimeAction PostRebindsLoaded = new OneTimeAction();
#pragma warning restore UDR0001


    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private InputActionReference cancelRebindAction;

    [SerializeField] private GameObject rebindPopupObj;
    [SerializeField] private TextMeshProUGUI toRebindControlText;


    private InputActionRebindingExtensions.RebindingOperation currentOperation;

    private const string REBINDS_PATH = "Input/Rebinds";

#if Enable_Debug_Logging
    [SerializeField] private bool logRebindOperations = true;
#endif


    private void OnEnable()
    {
        cancelRebindAction.action.performed += OnCancelRebind;
        cancelRebindAction.action.Enable();
    }

    private void OnDisable()
    {
        cancelRebindAction.action.performed -= OnCancelRebind;
        cancelRebindAction.action.Disable();
    }

    private void Awake()
    {
        Instance = this;
        _ = LoadRebindsAsync();
    }

    private void OnCancelRebind(InputAction.CallbackContext ctx)
    {
        if (ctx.performed == false) return;

        CancelRebind();
    }


    public void StartRebind(string actionName, string bindingId, TextMeshProUGUI rebindKeyText)
    {
        if (currentOperation != null)
        {
            return;
        }

        if (!inputActions.TryFindAction(actionName, out InputAction action))
        {
            DebugLogger.Log($"Action {actionName} not found.", logRebindOperations);
            return;
        }

        int bindingIndex = action.bindings
            .Select((b, i) => new { binding = b, index = i })
            .FirstOrDefault(x => x.binding.id.ToString() == bindingId)?.index ?? -1;

        if (bindingIndex == -1)
        {
            DebugLogger.Log($"Binding ID {bindingId} not found on action {actionName}.", logRebindOperations);
            return;
        }

        // Prevent rebinding composite headers
        if (action.bindings[bindingIndex].isComposite)
        {
            DebugLogger.Log("Cannot rebind a composite root binding.", logRebindOperations);
            return;
        }

        action.Disable();
        rebindPopupObj.SetActive(true);
        toRebindControlText.text = $"Rebinding '{actionName}' ({action.GetBindingDisplayString(bindingIndex)})";

        currentOperation = action
            .PerformInteractiveRebinding(bindingIndex)
            .WithCancelingThrough("<Keyboard>/escape")
            .OnMatchWaitForAnother(0.1f)
            .OnCancel(operation =>
            {
                Cleanup(action, operation);
                DebugLogger.Log($"Rebind for {actionName} canceled.", logRebindOperations);
            })
            .OnComplete(operation =>
            {
                DebugLogger.Log(
                    $"Rebound {actionName} to {action.bindings[bindingIndex].effectivePath}",
                    logRebindOperations
                );

                rebindKeyText.text = action.GetBindingDisplayString(bindingIndex);
                rebindPopupObj.SetActive(false);

                Cleanup(action, operation);
                _ = SaveRebindsAsync();
            });

        currentOperation.Start();
    }

    public void CancelRebind()
    {
        currentOperation?.Cancel();
        rebindPopupObj.SetActive(false);
    }
    private void Cleanup(InputAction action, InputActionRebindingExtensions.RebindingOperation operation)
    {
        action.Enable();

        operation.Dispose();
        currentOperation = null;
    }

    private async Task LoadRebindsAsync()
    {
        (bool success, ValueWrapper<string> rebindJson) = await FileManager.LoadInfoAsync<ValueWrapper<string>>(REBINDS_PATH);

        if (success && !string.IsNullOrEmpty(rebindJson.Value))
        {
            inputActions.LoadBindingOverridesFromJson(rebindJson.Value);
            DebugLogger.Log("Rebinds loaded.", logRebindOperations);
        }
        PostRebindsLoaded?.Invoke();
    }
    private async Task SaveRebindsAsync()
    {
        string json = inputActions.SaveBindingOverridesAsJson();
        await FileManager.SaveInfoAsync(new ValueWrapper<string>(json), REBINDS_PATH);
        DebugLogger.Log("Rebinds saved.", logRebindOperations);
    }

    public void ResetRebinds()
    {
        inputActions.RemoveAllBindingOverrides();

#if Enable_Debug_Logging
        bool success = FileManager.TryDeleteFile(REBINDS_PATH);

        if (logRebindOperations)
        {
            if (success)
            {
                DebugLogger.Log("Rebinds reset and file deleted.");
            }
            else
            {
                DebugLogger.Log("Rebinds reset but no file found.");
            }
        }
#else
        FileManager.TryDeleteFile(REBINDS_PATH);
#endif
    }

    private void OnDestroy()
    {
        CancelRebind();
    }
}
