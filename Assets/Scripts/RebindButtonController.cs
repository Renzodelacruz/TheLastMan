using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class RebindButtonController : MonoBehaviour
{
    [Header("Configuración de la Acción")]
    [Tooltip("Arrastra aquí la acción específica desde tu archivo de Input Actions")]
    [SerializeField] private InputActionReference actionReference;

    [Header("Componentes de la Interfaz (UI)")]
    [Tooltip("El componente botón de este objeto UI")]
    [SerializeField] private Button rebindButton;

    [Tooltip("El texto dentro del botón que cambiará de nombre (ej: 'W', 'E')")]
    [SerializeField] private TMP_Text buttonText;

    // AHORA ELIGES EL BOTÓN DIRECTAMENTE CON UN NÚMERO EN EL INSPECTOR:
    [Header("Index de Asignación (Para WASD)")]
    [Tooltip("Para acciones simples (Interact, Reload, Shoot) déjalo en 0.\nPara WASD:\n1 = W (Up)\n2 = S (Down)\n3 = A (Left)\n4 = D (Right)")]
    [SerializeField] private int bindingIndex = 0;

    private InputActionRebindingExtensions.RebindingOperation rebindOperation;

    private void Start()
    {
        if (rebindButton != null)
        {
            rebindButton.onClick.AddListener(StartRebinding);
        }

        UpdateButtonText();
    }

    private void StartRebinding()
    {
        if (actionReference == null || actionReference.action == null) return;

        rebindButton.interactable = false;
        if (buttonText != null) buttonText.text = "...";

        actionReference.action.actionMap.Disable();

        rebindOperation = actionReference.action.PerformInteractiveRebinding()
            .WithControlsExcluding("<Mouse>/position")
            .WithControlsExcluding("<Mouse>/delta")
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(operation => FinishRebinding())
            .OnCancel(operation => FinishRebinding());

        // Si es una parte del WASD, le decimos de forma limpia el índice numérico directo
        if (bindingIndex > 0 && bindingIndex < actionReference.action.bindings.Count)
        {
            rebindOperation.WithTargetBinding(bindingIndex);
        }

        rebindOperation.Start();
    }

    private void FinishRebinding()
    {
        rebindOperation.Dispose();
        actionReference.action.actionMap.Enable();
        rebindButton.interactable = true;

        string overrides = actionReference.action.actionMap.ToJson();
        PlayerPrefs.SetString("InputOverrides_" + actionReference.action.actionMap.name, overrides);
        PlayerPrefs.Save();

        UpdateButtonText();
    }

    private void UpdateButtonText()
    {
        if (actionReference == null || actionReference.action == null || buttonText == null) return;

        if (bindingIndex >= 0 && bindingIndex < actionReference.action.bindings.Count)
        {
            buttonText.text = InputControlPath.ToHumanReadableString(
                actionReference.action.bindings[bindingIndex].effectivePath,
                InputControlPath.HumanReadableStringOptions.OmitDevice);
        }
    }
}