using UnityEngine;

public class WaveDirector : MonoBehaviour
{
    public enum WaveState
    {
        Rest,
        BuildUp,
        Peak
    }

    public WaveState currentState = WaveState.Rest;

    [Header("Referencias")]
    public GameObject enemyPrefab;
    public Transform player;

    [Header("Spawn")]
    public float spawnRadiusMin = 10f;
    public float spawnRadiusMax = 20f;
    public float spawnCooldown = 1.5f;
    private float spawnTimer;

    [Header("Control de enemigos")]
    public int maxEnemies = 15;

    [Header("Timing")]
    public float restDuration = 10f;
    public float buildUpDuration = 15f;
    public float peakDuration = 10f;

    private float stateTimer;

    void Update()
    {
        if (PerceptionManager.Instance == null) return;

        stateTimer += Time.deltaTime;

        switch (currentState)
        {
            case WaveState.Rest:
                HandleRest();
                break;

            case WaveState.BuildUp:
                HandleBuildUp();
                break;

            case WaveState.Peak:
                HandlePeak();
                break;
        }
    }

    void HandleRest()
    {
        if (stateTimer >= restDuration)
        {
            ChangeState(WaveState.BuildUp);
        }
    }

    void HandleBuildUp()
    {
        SpawnLogic(0.5f); // intensidad media

        if (stateTimer >= buildUpDuration)
        {
            ChangeState(WaveState.Peak);
        }
    }

    void HandlePeak()
    {
        SpawnLogic(1f); // intensidad máxima

        if (stateTimer >= peakDuration)
        {
            ChangeState(WaveState.Rest);
        }
    }

    void SpawnLogic(float intensityMultiplier)
    {
        spawnTimer += Time.deltaTime;

        float aggro = PerceptionManager.Instance.GetSmoothedPerception();

        // Ajusta velocidad de spawn según percepción + estado
        float dynamicCooldown = spawnCooldown / (1f + aggro * intensityMultiplier);

        if (spawnTimer >= dynamicCooldown)
        {
            if (GameObject.FindGameObjectsWithTag("Enemy").Length < maxEnemies)
            {
                SpawnEnemy();
            }

            spawnTimer = 0;
        }
    }

    void SpawnEnemy()
    {
        Vector3 spawnPos = GetSpawnPosition();

        if (!IsVisibleToPlayer(spawnPos))
        {
            Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        }
    }

    Vector3 GetSpawnPosition()
    {
        Vector2 randomCircle = Random.insideUnitCircle.normalized * Random.Range(spawnRadiusMin, spawnRadiusMax);

        return player.position + new Vector3(randomCircle.x, 0, randomCircle.y);
    }

    bool IsVisibleToPlayer(Vector3 pos)
    {
        Vector3 dir = (pos - player.position).normalized;

        Ray ray = new Ray(player.position, dir);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.CompareTag("Enemy"))
                return true;
        }

        return false;
    }

    void ChangeState(WaveState newState)
    {
        currentState = newState;
        stateTimer = 0;

        Debug.Log("Wave State: " + newState);
    }
}
