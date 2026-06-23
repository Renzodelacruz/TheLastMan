using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PerceptionEnemy : MonoBehaviour
{
    public enum State { Wander, Hunt, Rage, Attack }
    public State currentState = State.Wander;
    public Transform player;

    [Header("Ajustes de Percepción (Oído)")]
    public float hearingRadius = 12f;
    private bool hasDetectedPlayer = false;

    [Header("Ajustes de Patrulla (Wander Autónomo)")]
    public float patrolRadius = 10f;        // Radio máximo para buscar un punto al que caminar
    public float minWaitTime = 2f;          // Tiempo mínimo de espera al llegar a un punto
    public float maxWaitTime = 6f;          // Tiempo máximo de espera al llegar a un punto
    private Vector3 wanderTarget;
    private float waitTimer;
    private bool isWaiting = false;

    [Header("Ajustes de Muerte")]
    public float timeBeforeDestroy = 5.0f;
    private bool isDead = false;

    [Header("Ajustes de Combate")]
    public float damageAmount = 10f;
    public float attackRate = 1.5f;
    public float attackDistance = 2.2f;
    private float nextAttackTime;

    [Header("Movimiento")]
    public float rageSpeed = 6f;
    public float wanderSpeed = 2f;

    private NavMeshAgent agent;
    private Animator anim;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        agent.acceleration = 60f;
        agent.stoppingDistance = 1.5f;

        // Buscar el primer punto aleatorio al iniciar
        SetNewWanderTarget();
    }

    void Update()
    {
        if (player == null || isDead) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // 1. SISTEMA DE AUDICIÓN
        if (distance <= hearingRadius)
        {
            hasDetectedPlayer = true;
        }
        else
        {
            if (distance > hearingRadius * 1.5f)
            {
                hasDetectedPlayer = false;
            }
        }

        // 2. MÁQUINA DE ESTADOS
        if (hasDetectedPlayer)
        {
            isWaiting = false; // Interrumpir esperas si detecta al jugador

            if (distance <= attackDistance)
                currentState = State.Attack;
            else if (distance < 15f)
                currentState = State.Rage;
            else
                currentState = State.Wander;
        }
        else
        {
            currentState = State.Wander; // Libre y patrullando de forma autónoma
        }

        HandleBehavior();
        UpdateAnimations();
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;
        Die();
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        if (agent != null)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
            agent.enabled = false;
        }

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        if (anim != null)
        {
            anim.SetTrigger("fallforward");
            anim.SetBool("isAttacking", false);
        }

        Destroy(gameObject, timeBeforeDestroy);
    }

    void HandleBehavior()
    {
        if (currentState == State.Attack)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;

            Vector3 dir = (player.position - transform.position).normalized;
            dir.y = 0;
            if (dir != Vector3.zero)
                transform.forward = Vector3.Slerp(transform.forward, dir, Time.deltaTime * 10f);

            if (Time.time >= nextAttackTime)
            {
                DoDamage();
                nextAttackTime = Time.time + attackRate;
            }
        }
        else
        {
            agent.isStopped = false;
            agent.speed = (currentState == State.Rage) ? rageSpeed : wanderSpeed;

            // CASO A: El zombie te está persiguiendo porque te escuchó
            if (hasDetectedPlayer)
            {
                agent.SetDestination(player.position);
            }
            // CASO B: No te ha escuchado, patrulla el mapa aleatoriamente
            else
            {
                PatrolLogic();
            }
        }
    }

    void PatrolLogic()
    {
        // Si ya llegó al punto de destino (o está muy cerca)
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!isWaiting)
            {
                // Empezar a esperar en el sitio antes de caminar a otro lado
                isWaiting = true;
                waitTimer = Random.Range(minWaitTime, maxWaitTime);
            }
            else
            {
                // Cuenta atrás del tiempo de espera
                waitTimer -= Time.deltaTime;
                if (waitTimer <= 0)
                {
                    isWaiting = false;
                    SetNewWanderTarget();
                }
            }
        }
    }

    // Calcula una posición aleatoria válida dentro del NavMesh
    void SetNewWanderTarget()
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += transform.position; // Centrado en la posición actual del zombie

        NavMeshHit navHit;
        // Mapea la posición matemática 3D a un punto transitable real del NavMesh
        if (NavMesh.SamplePosition(randomDirection, out navHit, patrolRadius, -1))
        {
            wanderTarget = navHit.position;
            agent.SetDestination(wanderTarget);
        }
    }

    void DoDamage()
    {
        if (anim)
        {
            anim.SetTrigger("attack");
            anim.SetBool("isAttacking", true);
        }

        PlayerHealth health = player.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.TakeDamage(damageAmount);
        }
    }

    void UpdateAnimations()
    {
        if (!anim || isDead) return;

        bool moving = agent.enabled && agent.velocity.magnitude > 0.1f && !agent.isStopped;

        anim.SetBool("isWalking", moving && currentState == State.Wander);
        anim.SetBool("isRunning", moving && currentState == State.Rage);
        anim.SetBool("isAttacking", currentState == State.Attack);
    }

    private void OnDrawGizmosSelected()
    {
        // Radio de audición (Amarillo)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, hearingRadius);

        // Radio de patrulla aleatoria (Azul)
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, patrolRadius);
    }
}