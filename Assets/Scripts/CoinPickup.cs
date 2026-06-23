using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    public int value = 1;
    public float rotateSpeed = 100f;

    [Header("Perception Effect")]
    public float perceptionDecrease = 0.03f;

    [Header("Magnet System")]
    public float detectRadius = 8f;
    public float moveSpeed = 6f;
    public float pickupDistance = 1f;

    private Transform player;

    void Start()
    {
        
        FPSController fps = FindFirstObjectByType<FPSController>();

        if (fps != null)
        {
            player = fps.transform;
            Debug.Log(" Coin linked to player: " + player.name);
        }
        else
        {
            Debug.LogError(" No FPSController found in scene!");
        }
    }

    void Update()
    {
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);

        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        
        if (distance < detectRadius)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                player.position,
                moveSpeed * Time.deltaTime
            );
        }

        
        if (distance < pickupDistance)
        {
            Collect();
        }
    }

    void Collect()
    {
        
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.AddCoins(value);
        }

        
        if (PerceptionManager.Instance != null)
        {
            PerceptionManager.Instance.AddPerception(-perceptionDecrease);
        }

        Debug.Log(" Coin collected: " + value);

        Destroy(gameObject);
    }
}