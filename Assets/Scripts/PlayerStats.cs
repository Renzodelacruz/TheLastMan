using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;

    [Header("Speed")]
    public float baseSpeed = 5f;
    public float currentSpeed;

    void Awake()
    {
        Instance = this;
        currentSpeed = baseSpeed;
    }

    public void AddSpeed(float amount)
    {
        currentSpeed += amount;
        Debug.Log("Speed increased to: " + currentSpeed);
    }
}