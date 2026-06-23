using UnityEngine;

public class FlashlightController : MonoBehaviour
{
    public Light flashlight;

    private bool canUse = false;
    private bool isOn = false;

    void Start()
    {
        if (flashlight != null)
            flashlight.enabled = false;
    }

    void Update()
    {
        if (!canUse) return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            ToggleFlashlight();
        }
    }

    public void EnableFlashlight()
    {
        canUse = true;

        isOn = true;
        flashlight.enabled = true;

        Debug.Log("Linterna activada");
    }

    void ToggleFlashlight()
    {
        isOn = !isOn;
        flashlight.enabled = isOn;

        Debug.Log("Toggle linterna: " + isOn);
    }
}