using UnityEngine;

public class MadnessTextController : MonoBehaviour
{
    public Material textMaterial;

    void Update()
    {
        if (PerceptionManager.Instance == null) return;

        float madness = PerceptionManager.Instance.GetSmoothedPerception();

        textMaterial.SetFloat("_Madness", madness);
    }
}