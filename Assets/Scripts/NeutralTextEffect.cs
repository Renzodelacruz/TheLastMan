using UnityEngine;
using TMPro;

public class NeutralTextEffect : MonoBehaviour
{
    public TextMeshPro text;

    [Header("Perception")]
    public float hideThreshold = 0.3f;
    public float visibleThreshold = 0.5f;

    private Color baseColor;

    void Start()
    {
        baseColor = text.color;
    }

    void Update()
    {
        if (PerceptionManager.Instance == null) return;

        float perception = PerceptionManager.Instance.GetSmoothedPerception();

        // 🔥 FADE BASADO EN PERCEPCIÓN
        float visibility = Mathf.InverseLerp(hideThreshold, visibleThreshold, perception);

        Color c = baseColor;
        c.a = visibility;
        text.color = c;
    }
}