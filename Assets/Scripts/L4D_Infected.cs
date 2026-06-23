using UnityEngine;
using UnityEngine.AI;

public class L4D_Infected : MonoBehaviour
{
    public enum InfectedState { Idle, Wandering, Alerted, Chasing, Attacking }

    [Header("State")]
    public InfectedState currentState = InfectedState.Idle;

    [Header("Movement")]
    public float walkSpeed = 1.5f;
    public float runSpeed = 6.5f;
    public float acceleration = 40f;

    [Header("Combat")]
    public float damage = 10f;
    public float attackRange = 2f;
    public float attackRate = 1.0f;
    private float nextAttackTime;

    private NavMeshAgent agent;
    private Animator anim;
    private Transform player;
    private Vector3 lastNoisePosition;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // Configuración NavMesh estilo L4D (muy ágil)
        agent.speed = walkSpeed;
        agent.acceleration = acceleration;
        agent.stoppingDistance = attackRange - 0.5f;

        // Variar un poco el tamaño y velocidad para que no parezcan clones
        transform.localScale *= Random.Range(0.9f, 1.1f);
        runSpeed *= Random.Range(0.9f, 1.2f);
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        float noiseLevel = (PerceptionManager.Instance != null) ? PerceptionManager.Instance.GetSmoothedPerception() : 0f;

        // LÓGICA DE ESTADOS ESTILO L4D
        if (distanceToPlayer < attackRange)
        {
            currentState = InfectedState.Attacking;
        }
        else if (noiseLevel > 0.1f || distanceToPlayer < 15f)
        {
            currentState = InfectedState.Chasing;
        }
        else
        {
            currentState = InfectedState.Idle;
        }

        ApplyStateBehaviors();
    }

    void ApplyStateBehaviors()
    {
        switch (currentState)
        {
            case InfectedState.Idle:
                agent.isStopped = true;
                UpdateAnimations(false, false);
                break;

            case InfectedState.Chasing:
                agent.isStopped = false;
                agent.speed = runSpeed;
                agent.SetDestination(player.position);
                UpdateAnimations(false, true);
                break;

            case InfectedState.Attacking:
                agent.isStopped = true;
                PerformAttack();
                break;
        }
    }

    void PerformAttack()
    {
        // Mirar al jugador siempre
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;
        transform.forward = Vector3.Lerp(transform.forward, direction, Time.deltaTime * 10f);

        if (Time.time >= nextAttackTime)
        {
            anim.SetTrigger("attack");
            player.GetComponent<PlayerHealth>()?.TakeDamage(damage);
            nextAttackTime = Time.time + attackRate;
        }
    }

    void UpdateAnimations(bool isWalking, bool isRunning)
    {
        if (anim == null) return;
        anim.SetBool("isWalking", isWalking);
        anim.SetBool("isRunning", isRunning);
    }

    public void Die()
    {
        
        Destroy(gameObject);
    }
}