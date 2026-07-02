using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("Level1");
    }

public void Options()
    {
        SceneManager.LoadScene("Options");
    }



    public void QuitGame()
    {
        Application.Quit();
    }

    public void Records()
    {
        SceneManager.LoadScene("Records");
    }
}
