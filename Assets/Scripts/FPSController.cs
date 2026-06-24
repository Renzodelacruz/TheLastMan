using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem; 

[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour
{
    [Header("Estado del Jugador")]
    public bool hasGun = false;
    private bool isReloading = false;

    [Header("UI Referencias (TextMeshPro)")]
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI warningText;
    public GameObject uiPanel;

    [Header("Crosshair Dinámico")]
    public Image crosshairImage;
    public Color readyColor = Color.white;
    public Color coolingColor = new Color(1, 1, 1, 0.3f);
    public Color reloadColor = Color.yellow;
    public float scalePunch = 1.2f;

    [Header("Movimiento")]
    public float speed = 5f;
    public float gravity = -19.62f;
    public Transform cameraPivot;
    public float mouseSensitivity = 0.1f; // El valor por defecto se reduce para adaptarse al Delta puro

    [Header("Munición y Tiempos")]
    public int bulletsPerMag = 10;
    public int currentAmmo;
    public int totalReserves = 30;
    public float fireRate = 0.1f;
    public float reloadTime = 2.0f;
    private float nextFireTime;
    private float reloadEndTime;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip gunshotSound;
    public AudioClip reloadSound;
    //

    [Header("Referencias")]
    public Camera fpsCamera;

    private CharacterController controller;
    private float xRotation = 0f;
    private float yVelocity;

    // --- VARIABLES DEL NUEVO INPUT SYSTEM ---
    private PlayerControls controls;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool isShootingPressed = false;

    void Awake()
    {
        // Inicializamos la clase C# que generaste desde el mapa de controles
        controls = new PlayerControls();

        // Evento de un solo clic: Cuando se presione la acción "Reload", ejecutamos la recarga
        controls.Gameplay.Reload.performed += ctx => OnReloadInput();
    }

    void OnEnable()
    {
        // Activamos los controles al encender el objeto
        controls.Gameplay.Enable();
    }

    void OnDisable()
    {
        // Desactivamos los controles si el objeto se apaga
        controls.Gameplay.Disable();
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
        currentAmmo = bulletsPerMag;

        // Bloqueamos y ocultamos el puntero para que no se salga de la pantalla en gameplay
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // CRÍTICO: Cargamos las reasignaciones que el jugador haya guardado en el menú de opciones
        string savedOverrides = PlayerPrefs.GetString("InputOverrides_Gameplay", string.Empty);
        if (!string.IsNullOrEmpty(savedOverrides))
        {
            controls.LoadBindingOverridesFromJson(savedOverrides);
        }

        ToggleWeaponUI(hasGun);
        UpdateAmmoUI();
        if (warningText != null) warningText.text = "";
    }

    void Update()
    {
        // FRENO DE SEGURIDAD 1: Si el panel de introducción está activo al iniciar, congelamos al personaje
        if (FindFirstObjectByType<IntroManager>() != null && Time.timeScale == 0f)
        {
            return;
        }

        // FRENO DE SEGURIDAD 2: Si el juego está pausado por el menú de Escape, congelamos al personaje
        if (PauseMenu.isPaused)
        {
            return;
        }

        ToggleWeaponUI(hasGun);

        // LEEMOS LOS INPUTS CONTINUOS CADA FOTOGRAMA
        moveInput = controls.Gameplay.Move.ReadValue<Vector2>(); // WASD (X = Horizontal, Y = Vertical)
        lookInput = controls.Gameplay.Look.ReadValue<Vector2>(); // Movimiento del Mouse Delta puro
        isShootingPressed = controls.Gameplay.Shoot.IsPressed(); // ¿Mantiene presionado el click?

        HandleLook();
        HandleMovement();

        if (hasGun)
        {
            UpdateCrosshairUI();

            if (isShootingPressed && Time.time >= nextFireTime && !isReloading)
            {
                if (currentAmmo > 0)
                {
                    Shoot();
                }
                else if (controls.Gameplay.Shoot.WasPerformedThisFrame())
                {
                    //PSfx
                    ShowWarning("OUT OF AMMO! RELOAD (R)");
                }
                nextFireTime = Time.time + fireRate;
            }
        }
    }

    void OnReloadInput()
    {
        if (hasGun && currentAmmo < bulletsPerMag && totalReserves > 0 && !isReloading)
        {
            StartReload();
        }
    }

    // --- SISTEMA DE UI ---

    void ToggleWeaponUI(bool state)
    {
        if (uiPanel != null && uiPanel.activeSelf != state) uiPanel.SetActive(state);
        if (ammoText != null && ammoText.gameObject.activeSelf != state) ammoText.gameObject.SetActive(state);
        if (crosshairImage != null && crosshairImage.gameObject.activeSelf != state) crosshairImage.gameObject.SetActive(state);
    }

    void UpdateCrosshairUI()
    {
        if (crosshairImage == null) return;

        float progress = 0f;

        if (isReloading)
        {
            float timePassed = Time.time - (reloadEndTime - reloadTime);
            progress = Mathf.Clamp01(timePassed / reloadTime);
            crosshairImage.color = Color.Lerp(reloadColor, readyColor, progress);
        }
        else
        {
            float timeLeft = nextFireTime - Time.time;
            progress = Mathf.Clamp01(1f - (timeLeft / fireRate));
            crosshairImage.color = Color.Lerp(coolingColor, readyColor, progress);
        }

        crosshairImage.fillAmount = progress;
        crosshairImage.transform.localScale = Vector3.Lerp(crosshairImage.transform.localScale, Vector3.one, Time.deltaTime * 10f);
    }

    void UpdateAmmoUI()
    {
        if (ammoText != null)
        {
            ammoText.text = currentAmmo + " <color=#999999>/</color> " + totalReserves;
            ammoText.color = (currentAmmo <= 3) ? Color.red : Color.white;
        }
    }

    void ShowWarning(string message)
    {
        if (warningText != null) warningText.text = message;
    }

    void ClearWarning()
    {
        if (warningText) warningText.text = "";
    }

    void Shoot()
    {
        currentAmmo--;
        UpdateAmmoUI();
        PlaySfx(gunshotSound);

        if (crosshairImage != null)
            crosshairImage.transform.localScale = Vector3.one * scalePunch;

        if (currentAmmo <= 0) ShowWarning("RELOAD (R)");

        int layerMask = ~LayerMask.GetMask("Player");
        RaycastHit hit;

        if (Physics.Raycast(fpsCamera.transform.position, fpsCamera.transform.forward, out hit, 100f, layerMask))
        {
            PerceptionEnemy enemy = hit.transform.GetComponent<PerceptionEnemy>();

            if (enemy != null)
            {
                enemy.TakeDamage(1);
            }
        }

        Debug.DrawRay(fpsCamera.transform.position, fpsCamera.transform.forward * 100f, Color.red, 0.1f);
    }

    void StartReload()
    {
        isReloading = true;
        reloadEndTime = Time.time + reloadTime;

        PlaySfx(reloadSound);
        ShowWarning("RELOADING...");

        Invoke("FinishReload", reloadTime);
    }

    void FinishReload()
    {
        int needed = bulletsPerMag - currentAmmo;
        int toAdd = Mathf.Min(needed, totalReserves);

        currentAmmo += toAdd;
        totalReserves -= toAdd;

        isReloading = false;
        UpdateAmmoUI();
        ShowWarning("READY");
        Invoke("ClearWarning", 1f);
    }

    // --- PROCESAMIENTO SUAVE DE MOVIMIENTO Y CÁMARA ---

    void HandleLook()
    {
        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraPivot.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleMovement()
    {
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        controller.Move(move * speed * Time.deltaTime);

        yVelocity += gravity * Time.deltaTime;
        controller.Move(Vector3.up * yVelocity * Time.deltaTime);
        if (controller.isGrounded && yVelocity < 0) yVelocity = -2f;
    }

    void PlaySfx(AudioClip clip)
    {
        if (audioSource && clip) audioSource.PlayOneShot(clip);
    }
}

