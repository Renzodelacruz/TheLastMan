using UnityEngine;
using System.Collections.Generic;
using TMPro; // Necesario para la UI moderna de TextMeshPro
using UnityEngine.SceneManagement; // Necesario para cambiar de escena al pulsar Submit

public class LeaderboardManager : MonoBehaviour
{
    // Singleton para acceder fácilmente a este script desde cualquier otro (ej: desde la SafeZone)
    public static LeaderboardManager Instance;

    [Header("UI del Cronómetro")]
    [Tooltip("Arrastra aquí el texto que muestra el tiempo en tiempo real durante la partida")]
    public TextMeshProUGUI timerText;

    [Header("UI de Victoria (Win Panel)")]
    [Tooltip("El panel entero de victoria que se encenderá al ganar")]
    public GameObject winPanel;
    [Tooltip("El campo de texto (Input Field) donde el jugador escribe su nombre")]
    public TMP_InputField nameInputField;
    [Tooltip("El texto dentro del panel de victoria que mostrará el tiempo final del jugador")]
    public TextMeshProUGUI currentScoreText;

    [Header("Configuración de Escenas")]
    [Tooltip("Nombre exacto de la escena a la que irá el juego tras guardar el récord")]
    public string recordsSceneName = "RecordsScene";

    private float elapsedTime = 0f;
    private bool isTimerRunning = false;
    private const int MAX_RECORDS = 5; // Guardaremos un Top 5 de mejores tiempos

    // Estructura interna para organizar los datos de cada jugador
    [System.Serializable]
    public class RecordData
    {
        public string name;
        public float time;
    }

    void Awake()
    {
        // Configuración del Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Nos aseguramos de que el panel de victoria inicie apagado al empezar el nivel
        if (winPanel != null)
            winPanel.SetActive(false);

        // ?? ASEGURAR QUE EL JUGADOR COMIENCE CON EL RATÓN BLOQUEADO PARA MOVER LA CÁMARA
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Nos aseguramos de que el tiempo corra normalmente por si venimos de un menú pausado
        Time.timeScale = 1f;

        UpdateTimerUI();
    }

    void Update()
    {
        // 

        // El cronómetro solo avanza si isTimerRunning es verdadero
        if (isTimerRunning)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimerUI();
        }
    }

    // --- CONTROLES DEL CRONÓMETRO ---

    // Llama a esta función desde el botón "Continuar" de tu panel de introducción
    public void StartTimer()
    {
        elapsedTime = 0f;
        isTimerRunning = true;
        Debug.Log("¡Cronómetro iniciado!");

        // Volvemos a asegurar el bloqueo al cerrar la introducción por seguridad
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
    }

    // Llama a esta función desde tu script de victoria o SafeZone al asegurar todos los robots
    public void StopAndShowWinPanel()
    {
        isTimerRunning = false;

        if (winPanel != null)
        {
            winPanel.SetActive(true);

            // Mostramos el tiempo de la partida actual formateado en el panel de victoria
            if (currentScoreText != null)
            {
                currentScoreText.text = "Your Time: " + FormatTime(elapsedTime);
            }

            // ?? LIBERAR EL MOUSE ÚNICAMENTE PARA LA UI DE VICTORIA
            Cursor.lockState = CursorLockMode.None; // Desbloquea el ratón del centro de la pantalla
            Cursor.visible = true;                  // Hace que el cursor sea visible
        }
    }

    // Actualiza el texto en pantalla
    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            timerText.text = FormatTime(elapsedTime);
        }
    }

    // Convierte los segundos puros en un formato estético de minutos:segundos.centesimas
    string FormatTime(float timeToFormat)
    {
        int minutes = Mathf.FloorToInt(timeToFormat / 60f);
        int seconds = Mathf.FloorToInt(timeToFormat % 60f);
        int milliseconds = Mathf.FloorToInt((timeToFormat * 100f) % 100f);
        return string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, milliseconds);
    }

    // --- SISTEMA DE ALMACENAMIENTO (BASE DE DATOS LOCAL) ---

    // Esta función la debe ejecutar el botón "Submit" de tu Panel de Victoria
    public void SubmitRecord()
    {
        // Si el jugador no escribió nada, le asignamos "Anonymous" por defecto
        string playerName = "Anonymous";
        if (nameInputField != null && !string.IsNullOrEmpty(nameInputField.text))
        {
            playerName = nameInputField.text;
        }

        // 1. Cargamos el Top 5 actual desde la base de datos local (PlayerPrefs)
        List<RecordData> leaderboard = LoadLeaderboard();

        // 2. Creamos y añadimos el nuevo récord de esta partida
        RecordData newRecord = new RecordData { name = playerName, time = elapsedTime };
        leaderboard.Add(newRecord);

        // 3. Ordenamos la lista. (En contrarreloj, el que hizo MENOS tiempo va primero)
        leaderboard.Sort((x, y) => x.time.CompareTo(y.time));

        // 4. Si la lista supera el Top 5, eliminamos los que queden rezagados abajo
        if (leaderboard.Count > MAX_RECORDS)
        {
            leaderboard.RemoveRange(MAX_RECORDS, leaderboard.Count - MAX_RECORDS);
        }

        // 5. Guardamos la lista limpia y actualizada en el disco
        SaveLeaderboard(leaderboard);

        Debug.Log("Récord guardado con éxito de: " + playerName);

        // 6. FLUJO AUTOMÁTICO: Saltamos directamente a la escena de récords
        SceneManager.LoadScene(recordsSceneName);
    }

    // Carga los datos guardados en PlayerPrefs uno a uno y los mete en una lista ejecutable
    public List<RecordData> LoadLeaderboard()
    {
        List<RecordData> list = new List<RecordData>();
        for (int i = 0; i < MAX_RECORDS; i++)
        {
            if (PlayerPrefs.HasKey("LeaderboardName_" + i))
            {
                RecordData record = new RecordData();
                record.name = PlayerPrefs.GetString("LeaderboardName_" + i);
                record.time = PlayerPrefs.GetFloat("LeaderboardTime_" + i);
                list.Add(record);
            }
        }
        return list;
    }

    // Graba los datos de la lista de forma permanente en el sistema operativo
    void SaveLeaderboard(List<RecordData> list)
    {
        for (int i = 0; i < MAX_RECORDS; i++)
        {
            if (i < list.Count)
            {
                PlayerPrefs.SetString("LeaderboardName_" + i, list[i].name);
                PlayerPrefs.SetFloat("LeaderboardTime_" + i, list[i].time);
            }
            else
            {
                // Limpieza de llaves por seguridad
                PlayerPrefs.DeleteKey("LeaderboardName_" + i);
                PlayerPrefs.DeleteKey("LeaderboardTime_" + i);
            }
        }
        PlayerPrefs.Save(); // Asegura la escritura inmediata en el disco duro
    }
}