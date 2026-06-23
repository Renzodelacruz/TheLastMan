using UnityEngine;
using TMPro;

public class GunPickup : MonoBehaviour
{
    [Header("Settings")]
    public float perceptionBoost = 0.5f;

    [Header("Audio")]
    [SerializeField] private AudioClip pickupSound;

    [Header("References")]
    public GameObject gunInHand;
    public GameObject crosshairUI;
    public TextMeshPro worldText;

    private bool playerInside = false;

    void Start()
    {
        if (gunInHand) gunInHand.SetActive(false);
        if (crosshairUI) crosshairUI.SetActive(false);
        if (worldText) worldText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (playerInside && Input.GetKeyDown(KeyCode.E))
        {
            Pickup();
        }

        if (worldText != null && worldText.gameObject.activeSelf)
        {
            worldText.transform.LookAt(Camera.main.transform);
            worldText.transform.Rotate(0, 180, 0);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            if (worldText) worldText.gameObject.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            if (worldText) worldText.gameObject.SetActive(false);
        }
    }

    void Pickup()
    {
        FPSController playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<FPSController>();

        if (playerScript != null)
        {
            playerScript.hasGun = true;

            if (gunInHand) gunInHand.SetActive(true);
            if (crosshairUI) crosshairUI.SetActive(true);

            if (PerceptionManager.Instance) PerceptionManager.Instance.AddPerception(perceptionBoost);

            if (pickupSound != null)
            {
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            }

            GetComponent<Renderer>().enabled = false;
            GetComponent<Collider>().enabled = false;
            if (worldText) worldText.gameObject.SetActive(false);

            Destroy(gameObject, 0.1f);
            Debug.Log("Pistola recogida: Disparo habilitado.");
        }
    }
}
