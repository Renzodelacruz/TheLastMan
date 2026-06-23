using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement; // Necesario para el botón de volver al menú

public class LeaderboardDisplay : MonoBehaviour
{
    [Header("UI de los Récords (Top 5)")]
    // Listas para arrastrar tus TMPs individuales en orden del 1 al 5
    public List<TextMeshProUGUI> nameTexts;
    public List<TextMeshProUGUI> timeTexts;

    [Header("Configuración de Escenas")]
    public string mainMenuSceneName = "MainMenu";

    void Start()
    {
        DisplayLeaderboard();
    }

    void DisplayLeaderboard()
    {
        // Recorremos las 5 posiciones del Top
        for (int i = 0; i < 5; i++)
        {
            // Comprobamos si la base de datos tiene un récord en esta posición
            if (PlayerPrefs.HasKey("LeaderboardName_" + i))
            {
                string pName = PlayerPrefs.GetString("LeaderboardName_" + i);
                float pTime = PlayerPrefs.GetFloat("LeaderboardTime_" + i);

                // Formateamos las centésimas de tiempo de forma estética (00:00.00)
                int minutes = Mathf.FloorToInt(pTime / 60f);
                int seconds = Mathf.FloorToInt(pTime % 60f);
                int milliseconds = Mathf.FloorToInt((pTime * 100f) % 100f);
                string formattedTime = string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, milliseconds);

                // Si asignaste los TMPs en el inspector, les inyectamos la información
                if (i < nameTexts.Count && nameTexts[i] != null)
                    nameTexts[i].text = (i + 1) + ". " + pName;

                if (i < timeTexts.Count && timeTexts[i] != null)
                    timeTexts[i].text = formattedTime;
            }
            else
            {
                // Si la casilla está vacía en la base de datos, ponemos guiones por defecto
                if (i < nameTexts.Count && nameTexts[i] != null)
                    nameTexts[i].text = (i + 1) + ". Empty";

                if (i < timeTexts.Count && timeTexts[i] != null)
                    timeTexts[i].text = "--:--.--";
            }
        }
    }

    // FUNCIÓN PARA EL BOTÓN DE VOLVER:
    // Vincula esta función al evento OnClick() de tu botón "Back to Menu"
    public void GoToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}