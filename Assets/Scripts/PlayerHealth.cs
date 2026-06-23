using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("UI Reference")]
    public Image healthBarImage;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip damageSound;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateUI();

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateUI();

        if (damageSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(damageSound);
        }

        if (DamageFlash.instance != null)
        {
            DamageFlash.instance.PlayDamageEffect(currentHealth, maxHealth);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateUI()
    {
        if (healthBarImage != null)
        {
            healthBarImage.fillAmount = currentHealth / maxHealth;
        }

        if (DamageFlash.instance != null)
        {
            DamageFlash.instance.UpdateHealthStatus(currentHealth, maxHealth);
        }
    }

    public void Die()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.GameOver();
        }
        else
        {
            Debug.LogError("GameManager instance not found.");
        }
    }
}
