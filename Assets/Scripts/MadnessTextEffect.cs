using UnityEngine;
using TMPro;

public class MadnessTextEffect : MonoBehaviour
{
    public TextMeshPro text;

    [Header("Distortion")]
    public float distortionStrength = 5f;
    public float distortionSpeed = 5f;

    [Header("Visibility")]
    public float appearThreshold = 0.3f; // empieza a aparecer
    public float fullVisibleThreshold = 0.7f; // totalmente visible

    private TMP_TextInfo textInfo;
    private Color baseColor;

    void Start()
    {
        baseColor = text.color;
    }

    void Update()
    {
        if (PerceptionManager.Instance == null) return;

        float madness = PerceptionManager.Instance.GetSmoothedPerception();

        // 🔥 VISIBILIDAD PROGRESIVA
        float visibility = Mathf.InverseLerp(appearThreshold, fullVisibleThreshold, madness);

        Color c = baseColor;
        c.a = visibility;
        text.color = c;

        // ❌ Si no hay locura → no hacer nada más
        if (madness < appearThreshold)
            return;

        // 🔥 DISTORSIÓN DE LETRAS
        text.ForceMeshUpdate();
        textInfo = text.textInfo;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            if (!textInfo.characterInfo[i].isVisible) continue;

            int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
            int vertexIndex = textInfo.characterInfo[i].vertexIndex;

            Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

            // ruido caótico por letra
            float offsetX = Mathf.PerlinNoise(i, Time.time * distortionSpeed) - 0.5f;
            float offsetY = Mathf.PerlinNoise(Time.time * distortionSpeed, i) - 0.5f;

            Vector3 offset = new Vector3(offsetX, offsetY, 0) * madness * distortionStrength * 10f;

            vertices[vertexIndex + 0] += offset;
            vertices[vertexIndex + 1] += offset;
            vertices[vertexIndex + 2] += offset;
            vertices[vertexIndex + 3] += offset;
        }

        // aplicar cambios
        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
            text.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
    }
}