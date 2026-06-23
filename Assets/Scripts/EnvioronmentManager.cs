using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class EnvironmentManager : MonoBehaviour
{
    public static EnvironmentManager instance;

    [Header("References")]
    public Volume globalVolume;

    private ColorAdjustments colorAdjustments;
    private Vignette vignette;

    [Header("Visual Tuning")]
    public float minExposure = 0f;
    public float maxDarkExposure = -8f;

    public float minVignette = 0f;
    public float maxVignette = 0.5f;

    public float transitionSpeed = 2f;

    [Header("Breathing")]
    public float breathingSpeed = 1.5f;
    public float breathingStrength = 0.5f;

    private float currentExposure = 0f;
    private float currentVignette = 0f;

    void Awake()
    {
        instance = this;

        if (globalVolume.profile.TryGet(out colorAdjustments) == false)
        {
            Debug.LogError("No ColorAdjustments found in Volume!");
        }

        if (globalVolume.profile.TryGet(out vignette) == false)
        {
            Debug.LogError("No Vignette found in Volume!");
        }
    }

    void Update()
    {
        
        float p = PerceptionManager.Instance.GetSmoothedPerception();

       
        float targetExposure = Mathf.Lerp(minExposure, maxDarkExposure, p);
        float targetVignette = Mathf.Lerp(minVignette, maxVignette, p);

        
        currentExposure = Mathf.Lerp(currentExposure, targetExposure, Time.deltaTime * transitionSpeed);
        currentVignette = Mathf.Lerp(currentVignette, targetVignette, Time.deltaTime * transitionSpeed);

       
        float breath = Mathf.Sin(Time.time * breathingSpeed) * breathingStrength * p;

       
        colorAdjustments.postExposure.value = currentExposure + breath;

        if (vignette != null)
        {
            vignette.intensity.value = currentVignette + (breath * 0.1f);
        }
    }

    
    public void OnEnemyKilled()
    {
        PerceptionManager.Instance.AddKill();
    }
}