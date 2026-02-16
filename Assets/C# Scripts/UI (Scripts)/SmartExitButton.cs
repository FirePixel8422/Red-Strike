using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;



public class SmartExitButton : MonoBehaviour
{
    [SerializeField] private InputActionReference exitMenuInput;
    private Button button;


    private void Awake()
    {
        button = GetComponent<Button>();
    }
    private void OnEnable()
    {
        exitMenuInput.action.Enable();
        exitMenuInput.action.performed += OnExitMenu;
    }
    private void OnDisable()
    {
        exitMenuInput.action.performed -= OnExitMenu;
        exitMenuInput.action.Disable();
    }

    private void OnExitMenu(InputAction.CallbackContext ctx)
    {
        if (ctx.performed == false) return;

        button.onClick.Invoke();
    }
}