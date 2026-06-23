using UnityEngine;
using UnityEngine.SceneManagement;

public static class SaveSystem
{
    private const string SaveKey = "SavedSceneIndex";

    public static void SaveGame()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        PlayerPrefs.SetInt(SaveKey, currentSceneIndex);
        PlayerPrefs.Save();
        Debug.Log("Partida guardada en la escena índice: " + currentSceneIndex);
    }

    public static void LoadGame()
    {
        if (PlayerPrefs.HasKey(SaveKey))
        {
            int savedSceneIndex = PlayerPrefs.GetInt(SaveKey);
            SceneManager.LoadScene(savedSceneIndex);
        }
        else
        {
            Debug.LogWarning("No hay ninguna partida guardada.");
        }
    }

    public static bool HasSavedGame()
    {
        return PlayerPrefs.HasKey(SaveKey);
    }
}