using UnityEngine;
using UnityEngine.AI;
using TMPro; // Necesario para poder controlar textos de TextMeshPro por código

[RequireComponent(typeof(NavMeshAgent))]
public class Survivor : MonoBehaviour
{
    public Transform player;
    public float followDistance = 3f;
    public bool isRescued = false;

    [Header("Movimiento")]
    public float wanderSpeed = 4f;

    [Header("Interfaz (UI)")]
    [Tooltip("Arrastra aquí el texto de TextMeshPro que quieres mostrar al rescatarlo")]
    public TextMeshProUGUI rescueTextUI;
    [Tooltip("Tiempo exacto que durará el mensaje en pantalla antes de borrarse")]
    public float messageDuration = 3f; // <--- Configurado a 3 segundos por defecto

    private NavMeshAgent agent;
    private Animator anim;
    private bool isSaved = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Buscamos el Animator en los hijos por si el modelo 3D está dentro del objeto vacío
        anim = GetComponentInChildren<Animator>();

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        agent.speed = wanderSpeed;
        agent.stoppingDistance = followDistance - 0.5f;

        // Aseguramos que inicie en Idle al empezar la partida
        if (anim != null)
        {
            anim.SetBool("isWalking", false);
            anim.SetFloat("animSpeed", 1f);
        }

        // Nos aseguramos de que el texto inicie apagado al cargar el nivel
        if (rescueTextUI != null)
        {
            rescueTextUI.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // Si el jugador no existe o el superviviente ya fue entregado en la base, no hace nada
        if (player == null || isSaved) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Detecta al jugador si se acerca a menos de 4 metros por primera vez
        if (!isRescued && distanceToPlayer < 4f)
        {
            isRescued = true;
            Debug.Log("¡Sobreviviente rescatado! Sígueme.");

            // ACTIVAR MENSAJE
            ShowRescueMessage();
        }

        // Lógica de seguimiento activa
        if (isRescued)
        {
            if (distanceToPlayer > followDistance)
            {
                if (agent.isActiveAndEnabled)
                {
                    agent.isStopped = false;
                    agent.SetDestination(player.position);
                }
            }
            else
            {
                if (agent.isActiveAndEnabled && !agent.isStopped)
                {
                    agent.ResetPath();
                    agent.isStopped = true;
                }
            }

            // CONTROL DINÁMICO DE VELOCIDAD Y ANIMACIÓN
            if (anim != null)
            {
                float currentSpeed = agent.isActiveAndEnabled ? agent.velocity.magnitude : 0f;
                bool isMoving = currentSpeed > 0.1f && !agent.isStopped;

                if (anim.GetBool("isWalking") != isMoving)
                {
                    anim.SetBool("isWalking", isMoving);
                }

                if (isMoving)
                {
                    float speedMultiplier = currentSpeed / wanderSpeed;
                    if (speedMultiplier < 0.2f) speedMultiplier = 0.2f;

                    anim.SetFloat("animSpeed", speedMultiplier);
                }
                else
                {
                    anim.SetFloat("animSpeed", 1f);
                }
            }
        }
    }

    // Método para activar el texto y programar su desaparición automática
    void ShowRescueMessage()
    {
        if (rescueTextUI != null)
        {
            rescueTextUI.text = "Robot rescued!\nTake it to the Safe Zone";
            rescueTextUI.gameObject.SetActive(true);

            // Cancela cualquier temporizador previo por si acaso el script se reinicia de golpe
            CancelInvoke("HideRescueMessage");

            // Invoke llamará a la función de ocultado tras los 3 segundos configurados
            Invoke("HideRescueMessage", messageDuration);
        }
    }

    void HideRescueMessage()
    {
        if (rescueTextUI != null)
        {
            rescueTextUI.gameObject.SetActive(false);
        }
    }

    // Esta función la ejecuta la SafeZone automáticamente al recibirlo en su Trigger
    public void SaveSurvivorInBase(Vector3 safeZonePosition)
    {
        isRescued = false;
        isSaved = true;

        // Si el mensaje seguía activo o contando tiempo al llegar a la base, lo fulminamos de inmediato
        CancelInvoke("HideRescueMessage");
        HideRescueMessage();

        if (agent != null)
        {
            agent.isStopped = true;
            agent.ResetPath();
            agent.enabled = false;
        }

        if (anim != null)
        {
            anim.SetBool("isWalking", false);
            anim.SetFloat("animSpeed", 1f);
        }

        transform.position = safeZonePosition;
        Debug.Log("Robot asegurado en la base.");
    }
}