using UnityEngine;
using UnityEngine.Rendering;

public class VibeManager : MonoBehaviour
{
    public Volume greenVolume;
    public Volume redVolume;
    public Volume blueVolume;

    public enum VibeState
    {
        Green,
        Red,
        Blue
    }

    public VibeState currentVibe;

    void Start()
    {
        SetVibe(VibeState.Green);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SetVibe(VibeState.Green);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            SetVibe(VibeState.Red);

        if (Input.GetKeyDown(KeyCode.Alpha3))
            SetVibe(VibeState.Blue);
    }

    public void SetVibe(VibeState vibe)
    {
        currentVibe = vibe;

        // reset global (IMPORTANTE)
        greenVolume.weight = 0f;
        redVolume.weight = 0f;
        blueVolume.weight = 0f;

        // activar solo uno
        switch (vibe)
        {
            case VibeState.Green:
                greenVolume.weight = 1f;
                break;

            case VibeState.Red:
                redVolume.weight = 1f;
                break;

            case VibeState.Blue:
                blueVolume.weight = 1f;
                break;
        }
    }
}
