using UnityEngine;
using UnityEngine.AI;
using TMPro;

[RequireComponent(typeof(NavMeshAgent))]
public class Survivor : MonoBehaviour
{
    public Transform player;
    public float followDistance = 3f;
    public bool isRescued = false;

    [Header("Movimiento")]
    public float wanderSpeed = 4f;

    [Header("Interfaz (UI)")]
    public TextMeshProUGUI rescueTextUI;
    public float messageDuration = 3f;

    [Header("Audio de Rescate")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip sonidoRescate;

    private NavMeshAgent agent;
    private Animator anim;
    private bool isSaved = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        agent.speed = wanderSpeed;
        agent.stoppingDistance = followDistance - 0.5f;

        if (anim != null)
        {
            anim.SetBool("isWalking", false);
            anim.SetFloat("animSpeed", 1f);
        }

        if (rescueTextUI != null)
        {
            rescueTextUI.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (player == null || isSaved) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (!isRescued && distanceToPlayer < 4f)
        {
            isRescued = true;
            Debug.Log("¡Sobreviviente rescatado! Sígueme.");

            if (audioSource && sonidoRescate)
            {
                audioSource.PlayOneShot(sonidoRescate);
            }

            ShowRescueMessage();
        }

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

    void ShowRescueMessage()
    {
        if (rescueTextUI != null)
        {
            rescueTextUI.text = "Robot rescued!\nTake it to the Safe Zone";
            rescueTextUI.gameObject.SetActive(true);

            CancelInvoke("HideRescueMessage");
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

    public void SaveSurvivorInBase(Vector3 safeZonePosition)
    {
        isRescued = false;
        isSaved = true;

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