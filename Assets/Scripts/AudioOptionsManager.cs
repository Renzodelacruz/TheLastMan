using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioOptionsManager : MonoBehaviour
{
    [Header("Audio Mixer Reference")]
    [SerializeField] private AudioMixer mainMixer;

    [Header("UI Sliders")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider ambienceSlider;
    [SerializeField] private Slider musicSlider;

    private const string MasterKey = "MasterVolume";
    private const string SFXKey = "SFXVolume";
    private const string AmbienceKey = "AmbienceVolume";
    private const string MusicKey = "MusicVolume";

    private void Start()
    {
        LoadAudioSettings();

        if (masterSlider != null) masterSlider.onValueChanged.AddListener(SetMasterVolume);
        if (sfxSlider != null) sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        if (ambienceSlider != null) ambienceSlider.onValueChanged.AddListener(SetAmbienceVolume);
        if (musicSlider != null) musicSlider.onValueChanged.AddListener(SetMusicVolume);
    }

    public void SetMasterVolume(float value)
    {
        SetMixerVolume("MasterVol", value);
        PlayerPrefs.SetFloat(MasterKey, value);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float value)
    {
        SetMixerVolume("SFXVol", value);
        PlayerPrefs.SetFloat(SFXKey, value);
        PlayerPrefs.Save();
    }

    public void SetAmbienceVolume(float value)
    {
        SetMixerVolume("AmbienceVol", value);
        PlayerPrefs.SetFloat(AmbienceKey, value);
        PlayerPrefs.Save();
    }

    public void SetMusicVolume(float value)
    {
        SetMixerVolume("MusicVol", value);
        PlayerPrefs.SetFloat(MusicKey, value);
        PlayerPrefs.Save();
    }

    private void SetMixerVolume(string parameterName, float sliderValue)
    {
        float dB = Mathf.Log10(Mathf.Max(sliderValue, 0.0001f)) * 20f;
        mainMixer.SetFloat(parameterName, dB);
    }

    private void LoadAudioSettings()
    {
        float masterVal = PlayerPrefs.GetFloat(MasterKey, 0.75f);
        float sfxVal = PlayerPrefs.GetFloat(SFXKey, 0.75f);
        float ambienceVal = PlayerPrefs.GetFloat(AmbienceKey, 0.75f);
        float musicVal = PlayerPrefs.GetFloat(MusicKey, 0.75f);

        if (masterSlider != null) masterSlider.SetValueWithoutNotify(masterVal);
        if (sfxSlider != null) sfxSlider.SetValueWithoutNotify(sfxVal);
        if (ambienceSlider != null) ambienceSlider.SetValueWithoutNotify(ambienceVal);
        if (musicSlider != null) musicSlider.SetValueWithoutNotify(musicVal);

        SetMixerVolume("MasterVol", masterVal);
        SetMixerVolume("SFXVol", sfxVal);
        SetMixerVolume("AmbienceVol", ambienceVal);
        SetMixerVolume("MusicVol", musicVal);
    }
}