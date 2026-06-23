using UnityEngine;

public class PerceptionManager : MonoBehaviour
{
    public static PerceptionManager Instance;

    [Header("Perception")]
    [Range(0f, 1f)]
    public float targetPerception = 0f;

    private float currentPerception = 0f;

    public float smoothSpeed = 3f;
    public float decaySpeed = 0.08f;

    [Header("Rage Mode")]
    public bool globalAggro = false;
    public float rageThreshold = 0.7f;
    public float calmThreshold = 0.3f;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        
        targetPerception -= Time.deltaTime * decaySpeed;
        targetPerception = Mathf.Clamp01(targetPerception);

       
        currentPerception = Mathf.Lerp(currentPerception, targetPerception, Time.deltaTime * smoothSpeed);

        
        if (!globalAggro && targetPerception >= rageThreshold)
        {
            globalAggro = true;
            Debug.Log(" RAGE MODE ACTIVADO");
        }

        
        if (globalAggro && targetPerception <= calmThreshold)
        {
            globalAggro = false;
            Debug.Log("🧘 Rage mode desactivado");
        }
    }

    public float GetSmoothedPerception()
    {
        return currentPerception;
    }

    public float GetPerception()
    {
        return targetPerception;
    }

    public void AddPerception(float amount)
    {
        targetPerception += amount;
        targetPerception = Mathf.Clamp01(targetPerception);
    }

    public void AddKill()
    {
        AddPerception(0.25f);
    }
}