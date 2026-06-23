using UnityEngine;
using UnityEngine.SceneManagement;

public class VolverAlMenu : MonoBehaviour
{
    public string nombreDelMenuPrincipal = "MainMenu";

    public void CargarMenu()
    {
        SceneManager.LoadScene(nombreDelMenuPrincipal);
    }
}
