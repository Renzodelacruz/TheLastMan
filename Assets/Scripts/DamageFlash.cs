using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DamageFlash : MonoBehaviour
{
    public static DamageFlash instance;

    [Header("Componentes")]
    [SerializeField] private Volume damageVolume;
    [SerializeField] private AudioSource audioSource;

    [Header("Configuración del Efecto")]
    [SerializeField] private float flashDuration = 0.5f;
    [SerializeField] private AudioClip damageSound;

    [Header("Poca Vida")]
    [SerializeField] private float lowHealthThreshold = 30f;
    [SerializeField] private float maxLowHealthVignette = 0.6f;

    private Vignette vignette;
    private Coroutine flashCoroutine;
    private float currentHealthPercent = 100f;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        if (damageVolume != null && damageVolume.profile.TryGet(out Vignette activeVignette))
        {
            vignette = activeVignette;
            damageVolume.weight = 1f;
            vignette.intensity.overrideState = true;
            vignette.intensity.value = 0f;
        }
    }

    public void UpdateHealthStatus(float currentHealth, float maxHealth)
    {
        currentHealthPercent = (currentHealth / maxHealth) * 100f;

        if (flashCoroutine == null && vignette != null)
        {
            vignette.intensity.value = GetTargetWeight();
        }
    }

    public void PlayDamageEffect(float currentHealth, float maxHealth)
    {
        currentHealthPercent = (currentHealth / maxHealth) * 100f;

        if (audioSource != null && damageSound != null)
        {
            audioSource.PlayOneShot(damageSound);
        }

        if (vignette != null)
        {
            if (flashCoroutine != null)
            {
                StopCoroutine(flashCoroutine);
            }
            flashCoroutine = StartCoroutine(FadeFlash());
        }
    }

    private IEnumerator FadeFlash()
    {
        float startWeight = 1f;
        float targetWeight = GetTargetWeight();
        float elapsedTime = 0f;

        while (elapsedTime < flashDuration)
        {
            elapsedTime += Time.deltaTime;
            targetWeight = GetTargetWeight();
            vignette.intensity.value = Mathf.Lerp(startWeight, targetWeight, elapsedTime / flashDuration);
            yield return null;
        }

        vignette.intensity.value = GetTargetWeight();
        flashCoroutine = null;
    }

    private float GetTargetWeight()
    {
        if (currentHealthPercent <= 0) return 0f;

        if (currentHealthPercent <= lowHealthThreshold)
        {
            float t = 1f - (currentHealthPercent / lowHealthThreshold);
            return Mathf.Lerp(0f, maxLowHealthVignette, t);
        }

        return 0f;
    }
}