using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool isPaused = false;

    [Header("Canvas de UI Separados")]
    [SerializeField] private GameObject juegoOPrincipalCanvas;
    [SerializeField] private GameObject optionsCanvas;

    [Header("Paneles Internos")]
    [SerializeField] private GameObject pausePanel;

    [Header("Configuración de Escenas")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    void Start()
    {
        if (pausePanel) pausePanel.SetActive(false);
        if (optionsCanvas) optionsCanvas.SetActive(false);
        if (juegoOPrincipalCanvas) juegoOPrincipalCanvas.SetActive(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                if (optionsCanvas != null && optionsCanvas.activeSelf)
                {
                    CloseOptions();
                }
                else
                {
                    Resume();
                }
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        if (pausePanel) pausePanel.SetActive(false);
        if (optionsCanvas) optionsCanvas.SetActive(false);
        if (juegoOPrincipalCanvas) juegoOPrincipalCanvas.SetActive(true);

        Time.timeScale = 1f;
        isPaused = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Pause()
    {
        if (juegoOPrincipalCanvas) juegoOPrincipalCanvas.SetActive(true);
        if (pausePanel) pausePanel.SetActive(true);
        if (optionsCanvas) optionsCanvas.SetActive(false);

        Time.timeScale = 0f;
        isPaused = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OpenOptions()
    {
        if (juegoOPrincipalCanvas) juegoOPrincipalCanvas.SetActive(false);
        if (optionsCanvas) optionsCanvas.SetActive(true);
    }

    public void CloseOptions()
    {
        SaveSystem.SaveGame();

        if (optionsCanvas) optionsCanvas.SetActive(false);
        if (juegoOPrincipalCanvas) juegoOPrincipalCanvas.SetActive(true);
        if (pausePanel) pausePanel.SetActive(true);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        isPaused = false;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}