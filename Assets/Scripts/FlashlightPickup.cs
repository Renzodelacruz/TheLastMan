using UnityEngine;

public class FlashlightPickup : MonoBehaviour
{
    public FlashlightController flashlightController;

    public Light directionalLight;

    private bool playerInRange = false;

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Pickup();
        }
    }

    void Pickup()
    {
        flashlightController.EnableFlashlight();

        if (directionalLight != null)
            directionalLight.enabled = false;

        RenderSettings.ambientIntensity = 0f;
        RenderSettings.ambientLight = Color.black;

        gameObject.SetActive(false);

        Debug.Log("Linterna recogida");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}