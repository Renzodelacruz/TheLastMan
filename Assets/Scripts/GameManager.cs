using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("UI de Fin de Juego")]
    public GameObject victoryPanel;
    public GameObject gameOverPanel;

    private bool gameHasEnded = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (victoryPanel) victoryPanel.SetActive(false);
        if (gameOverPanel) gameOverPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void Victory()
    {
        if (gameHasEnded) return;
        gameHasEnded = true;

        if (victoryPanel) victoryPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f;
    }

    public void GameOver()
    {
        if (gameHasEnded) return;
        gameHasEnded = true;

        if (gameOverPanel) gameOverPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}