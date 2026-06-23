using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ApplyVideoSettings : MonoBehaviour
{
    public Volume globalVolume;
    private LiftGammaGain liftGammaGain;
    private ColorAdjustments colorAdjustments;

    void Start()
    {
        if (globalVolume != null)
        {
            float savedBrightness = PlayerPrefs.GetFloat("Brightness", 0.5f);
            if (globalVolume.profile.TryGet(out liftGammaGain))
            {
                float gammaOffset = Mathf.Lerp(-0.5f, 0.5f, savedBrightness);
                liftGammaGain.gamma.Override(new Vector4(1, 1, 1, gammaOffset));
            }

            bool savedHighContrast = PlayerPrefs.GetInt("HighContrast", 0) == 1;
            if (globalVolume.profile.TryGet(out colorAdjustments))
            {
                colorAdjustments.contrast.Override(savedHighContrast ? 40f : 0f);
            }
        }
    }
}
