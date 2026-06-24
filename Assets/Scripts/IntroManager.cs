using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class IntroManager : MonoBehaviour
{
    [Header("Configuración de Texto")]
    [TextArea(5, 10)]
    [SerializeField] private string mensajeIntro;
    [SerializeField] private float velocidadEscritura = 0.05f;

    [Header("Referencias UI")]
    [SerializeField] private TMP_Text textoDestino;
    [SerializeField] private GameObject panelIntro;
    [SerializeField] private Button botonContinuar;

    [Header("Audio de Narración")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip narracionCompleta;

    private void Start()
    {
        panelIntro.SetActive(true);
        botonContinuar.gameObject.SetActive(false);
        textoDestino.text = "";

        Time.timeScale = 0f;

        StartCoroutine(LiberarMouseConRetraso());
        StartCoroutine(EfectoEscribirYNarrar());
    }

    IEnumerator LiberarMouseConRetraso()
    {
        yield return new WaitForEndOfFrame();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    IEnumerator EfectoEscribirYNarrar()
    {
        yield return new WaitForSecondsRealtime(0.5f);

        if (audioSource && narracionCompleta)
        {
            audioSource.clip = narracionCompleta;
            audioSource.Play();
        }

        foreach (char letra in mensajeIntro.ToCharArray())
        {
            textoDestino.text += letra;
            yield return new WaitForSecondsRealtime(velocidadEscritura);
        }

        if (audioSource != null)
        {
            while (audioSource.isPlaying)
            {
                yield return null;
            }
        }

        botonContinuar.gameObject.SetActive(true);
    }

    public void ComenzarJuego()
    {
        if (audioSource && audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        panelIntro.SetActive(false);
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("Misión iniciada: Recuperar 3 robots.");
    }
}