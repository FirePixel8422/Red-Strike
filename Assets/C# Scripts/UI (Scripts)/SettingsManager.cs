using UnityEngine;
using UnityEngine.InputSystem;


public class SettingsManager : MonoBehaviour
{
    [SerializeField] private InputActionReference toggleMenuInput;
    [SerializeField] private GameObject settingsUI;



    private void OnEnable()
    {
        toggleMenuInput.action.Enable();
        toggleMenuInput.action.performed += OnToggleMenu;
    }
    private void OnDisable()
    {
        toggleMenuInput.action.performed -= OnToggleMenu;
        toggleMenuInput.action.Disable();
    }

    private void OnToggleMenu(InputAction.CallbackContext ctx)
    {
        if (ctx.performed == false) return;

        //GetComponent<Button>().onClick
    }
}