using UnityEngine;

public class Target : MonoBehaviour
{
    public float health = 50f;

    public void TakeDamage(float amount)
    {
        health -= amount;

        if (health <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        
        if (EnvironmentManager.instance != null)
        {
            EnvironmentManager.instance.OnEnemyKilled();
        }

        Destroy(gameObject);
    }
}