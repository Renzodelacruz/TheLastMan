using UnityEngine;
using TMPro;

public class SafeZone : MonoBehaviour
{
    [Header("Configuración de la Zona Segura")]
    [SerializeField] private Color zoneColor = new Color(0f, 1f, 0f, 0.3f);
    [SerializeField] private int survivorsRequired = 3;
    private int survivorsSavedCount = 0;

    [Header("UI del Juego (Canvas en Pantalla)")]
    [SerializeField] private TextMeshProUGUI counterText;

    private void Start()
    {
        UpdateCounterUI();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Survivor[] allSurvivors = FindObjectsOfType<Survivor>();
            Survivor rescuedSurvivor = null;

            foreach (Survivor s in allSurvivors)
            {
                if (s.isRescued)
                {
                    rescuedSurvivor = s;
                    break;
                }
            }

            if (rescuedSurvivor != null)
            {
                rescuedSurvivor.SaveSurvivorInBase(transform.position);

                survivorsSavedCount++;
                UpdateCounterUI();

                if (survivorsSavedCount >= survivorsRequired)
                {
                    if (GameManager.instance != null)
                    {
                        GameManager.instance.Victory();
                    }
                    else
                    {
                        Debug.LogError("GameManager instance not found.");
                    }
                }
            }
        }
    }

    void UpdateCounterUI()
    {
        if (counterText != null)
        {
            counterText.text = "Robots Rescued: " + survivorsSavedCount + " / " + survivorsRequired;
        }
    }

    private void OnDrawGizmos()
    {
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        if (boxCollider != null)
        {
            Gizmos.color = zoneColor;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(boxCollider.center, boxCollider.size);
        }
    }
}