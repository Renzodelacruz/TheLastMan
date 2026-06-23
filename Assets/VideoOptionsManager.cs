using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class VideoOptionsManager : MonoBehaviour
{
    public static VideoOptionsManager Instance { get; private set; }

    [Header("UI Components")]
    public TMP_Dropdown resolutionDropdown;
    public Slider brightnessSlider;
    public Button highContrastButton;

    [Tooltip("Arrastra aquí la imagen negra que cubre todo el Canvas")]
    public Image brilloUIFiltro;

    [Header("Post Processing Reference")]
    public Volume globalVolume;

    private ColorAdjustments colorAdjustments;
    private LiftGammaGain liftGammaGain;

    private List<Resolution> uniqueResolutionsList;
    private bool isHighContrastActive = false;

    // NUEVA VARIABLE: Para guardar si el efecto rojo está activo o no
    private bool isRedEffectActive = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RefreshSceneReferences();

        // 1. Cargar y aplicar Brillo
        float savedBrightness = PlayerPrefs.GetFloat("Brightness", 0.5f);
        SetBrightness(savedBrightness);

        // 2. Cargar y aplicar Alto Contraste
        isHighContrastActive = PlayerPrefs.GetInt("HighContrast", 0) == 1;
        ApplyHighContrast(isHighContrastActive);

        // 3. Cargar y aplicar el Efecto Rojo automáticamente en la nueva escena
        isRedEffectActive = PlayerPrefs.GetInt("RedEffectActive", 0) == 1;
        ApplyRedEffect(isRedEffectActive);
    }

    void Start()
    {
        SetupResolutionDropdown();

        float savedBrightness = PlayerPrefs.GetFloat("Brightness", 0.5f);
        if (brightnessSlider != null)
        {
            brightnessSlider.SetValueWithoutNotify(savedBrightness);
        }

        SetupBrightness();

        isHighContrastActive = PlayerPrefs.GetInt("HighContrast", 0) == 1;
        ApplyHighContrast(isHighContrastActive);

        // Cargar estado inicial del efecto rojo
        isRedEffectActive = PlayerPrefs.GetInt("RedEffectActive", 0) == 1;
        ApplyRedEffect(isRedEffectActive);

        if (highContrastButton != null)
        {
            highContrastButton.onClick.AddListener(OnHighContrastButtonClicked);
        }
    }

    void RefreshSceneReferences()
    {
        if (globalVolume == null)
        {
            globalVolume = FindObjectOfType<Volume>();
        }

        if (globalVolume != null)
        {
            if (!globalVolume.profile.TryGet(out colorAdjustments))
            {
                colorAdjustments = globalVolume.profile.Add<ColorAdjustments>(true);
            }

            if (!globalVolume.profile.TryGet(out liftGammaGain))
            {
                liftGammaGain = globalVolume.profile.Add<LiftGammaGain>(true);
            }

            colorAdjustments.postExposure.overrideState = true;
            colorAdjustments.colorFilter.overrideState = true; // Forzamos el estado del filtro de color
            liftGammaGain.gamma.overrideState = true;
        }

        if (brilloUIFiltro == null)
        {
            GameObject filtroGO = GameObject.Find("brilloUIFiltro");
            if (filtroGO != null)
            {
                brilloUIFiltro = filtroGO.GetComponent<Image>();
            }
        }
    }

    void SetupResolutionDropdown()
    {
        if (resolutionDropdown == null) return;

        Resolution[] allResolutions = Screen.resolutions;
        uniqueResolutionsList = new List<Resolution>();
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < allResolutions.Length; i++)
        {
            if (allResolutions[i].refreshRateRatio.value == Screen.currentResolution.refreshRateRatio.value)
            {
                uniqueResolutionsList.Add(allResolutions[i]);
                string option = allResolutions[i].width + " x " + allResolutions[i].height;
                options.Add(option);

                if (allResolutions[i].width == Screen.currentResolution.width &&
                    allResolutions[i].height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = uniqueResolutionsList.Count - 1;
                }
            }
        }

        resolutionDropdown.AddOptions(options);

        int savedRes = PlayerPrefs.GetInt("ResolutionIndex", currentResolutionIndex);
        if (savedRes >= uniqueResolutionsList.Count) savedRes = currentResolutionIndex;

        resolutionDropdown.value = savedRes;
        resolutionDropdown.RefreshShownValue();
        resolutionDropdown.onValueChanged.AddListener(SetResolution);
    }

    public void SetResolution(int resolutionIndex)
    {
        if (uniqueResolutionsList == null || resolutionIndex >= uniqueResolutionsList.Count) return;
        Resolution resolution = uniqueResolutionsList[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode);
        PlayerPrefs.SetInt("ResolutionIndex", resolutionIndex);
        PlayerPrefs.Save();
    }

    void SetupBrightness()
    {
        RefreshSceneReferences();

        if (brightnessSlider != null)
        {
            brightnessSlider.onValueChanged.AddListener(SetBrightness);
            SetBrightness(brightnessSlider.value);
        }
    }

    public void SetBrightness(float value)
    {
        if (colorAdjustments != null)
        {
            float exposureValue = Mathf.Lerp(-8f, 2f, value);
            colorAdjustments.postExposure.value = exposureValue;
        }

        if (liftGammaGain != null)
        {
            float gammaOffset = Mathf.Lerp(-0.7f, 0.4f, value);
            liftGammaGain.gamma.value = new Vector4(gammaOffset, gammaOffset, gammaOffset, 0f);
        }

        if (brilloUIFiltro != null)
        {
            float alphaOcuridad = Mathf.Lerp(1f, 0f, value);
            Color colorActual = brilloUIFiltro.color;
            colorActual.a = alphaOcuridad;
            brilloUIFiltro.color = colorActual;
        }

        PlayerPrefs.SetFloat("Brightness", value);
        PlayerPrefs.Save();
    }

    private void OnHighContrastButtonClicked()
    {
        isHighContrastActive = !isHighContrastActive;
        ApplyHighContrast(isHighContrastActive);
        PlayerPrefs.SetInt("HighContrast", isHighContrastActive ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void ApplyHighContrast(bool activate)
    {
        if (globalVolume != null && globalVolume.profile.TryGet(out ColorAdjustments localColor))
        {
            localColor.contrast.overrideState = true;
            localColor.contrast.value = activate ? 40f : 0f;
        }
    }

    // NUEVO MÉTODO MAESTRO: Llama a esto desde el botón o evento de Opciones que activa tu efecto rojo
    public void ToggleRedEffect(bool activate)
    {
        isRedEffectActive = activate;
        ApplyRedEffect(isRedEffectActive);
        PlayerPrefs.SetInt("RedEffectActive", isRedEffectActive ? 1 : 0);
        PlayerPrefs.Save();
    }

    // NUEVA FUNCIÓN: Se encarga de tintar el volumen actual de rojo o devolverlo a blanco
    private void ApplyRedEffect(bool activate)
    {
        if (colorAdjustments != null)
        {
            // Si está activo, aplicamos el tinte rojo puro. Si no, lo dejamos en blanco puro (color base neutro)
            colorAdjustments.colorFilter.value = activate ? new Color(1f, 0.3f, 0.3f, 1f) : Color.white;
        }
    }
}