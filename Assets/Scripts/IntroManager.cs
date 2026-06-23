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

    [Header("Audio (Opcional)")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip sonidoTecla;

    private void Start()
    {
        // Al comenzar, preparamos la escena activando el panel y ocultando el botón
        panelIntro.SetActive(true);
        botonContinuar.gameObject.SetActive(false);
        textoDestino.text = "";

        // Pausamos el juego inmediatamente para congelar físicas y enemigos
        Time.timeScale = 0f;

        // SOLUCIÓN CRÍTICA: Forzamos la liberación del mouse al final del frame para ganarle al FPSController
        StartCoroutine(LiberarMouseConRetraso());

        // Iniciamos el efecto de máquina de escribir
        StartCoroutine(EfectoEscribir());
    }

    IEnumerator LiberarMouseConRetraso()
    {
        // Esperamos a que todos los scripts de la escena terminen su Inicialización (Start)
        yield return new WaitForEndOfFrame();

        // Rompemos el bloqueo del FPSController y obligamos al mouse a ser libre y visible
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    IEnumerator EfectoEscribir()
    {
        // Pequeña espera antes de empezar a escribir en pantalla
        yield return new WaitForSecondsRealtime(0.5f);

        foreach (char letra in mensajeIntro.ToCharArray())
        {
            textoDestino.text += letra;

            // Sonido de teclado si tienes uno asignado en el Inspector
            if (audioSource && sonidoTecla)
                audioSource.PlayOneShot(sonidoTecla);

            // Usamos WaitForSecondsRealtime porque el Time.timeScale está en 0
            yield return new WaitForSecondsRealtime(velocidadEscritura);
        }

        // Al terminar de escribir todo el lore de los 3 robots, mostramos el botón para continuar
        botonContinuar.gameObject.SetActive(true);
    }

    public void ComenzarJuego()
    {
        // Quitamos el panel narrativo y reanudamos el tiempo del juego
        panelIntro.SetActive(false);
        Time.timeScale = 1f;

        // Volvemos a bloquear y esconder el mouse para que funcione el apuntado del shooter
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("Misión iniciada: Recuperar 3 robots.");
    }
}