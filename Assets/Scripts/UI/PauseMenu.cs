using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Menú de pausa: Esc (o Start en el mando) lo abre y lo cierra.
// Pausar = Time.timeScale a 0: todo lo que usa Time.deltaTime (movimiento, física, reloj del juego) se para.
// La acción "Pause" está en el mapa UI, que nunca se desactiva, así funciona a pie, en vehículo y en pausa.
public class PauseMenu : MonoBehaviour
{
    [Tooltip("Panel del menú (se muestra al pausar).")]
    [SerializeField] private GameObject panel;

    [SerializeField] private Button resumeButton;

    [Tooltip("Opcional: mientras duermes no se puede pausar (rompería el fundido).")]
    [SerializeField] private SleepController sleepController;

    [SerializeField] private string mainMenuSceneName = "MainMenu";

    public bool IsPaused { get; private set; }

    private InputAction pauseAction;
    private InputActionMap playerMap;
    private InputActionMap vehicleMap;

    // Cómo estaban los controles antes de pausar, para dejarlos igual al continuar
    private bool playerMapWasEnabled;
    private bool vehicleMapWasEnabled;

    private void Awake()
    {
        pauseAction = InputSystem.actions.FindAction("UI/Pause", throwIfNotFound: true);
        playerMap = InputSystem.actions.FindActionMap("Player", throwIfNotFound: true);
        vehicleMap = InputSystem.actions.FindActionMap("Vehicle", throwIfNotFound: true);
        panel.SetActive(false);
    }

    private void Update()
    {
        if (!pauseAction.WasPressedThisFrame()) return;

        if (IsPaused) Resume();
        else Pause();
    }

    public void Pause()
    {
        if (IsPaused) return;
        if (sleepController != null && sleepController.IsSleeping) return;

        IsPaused = true;
        Time.timeScale = 0f;

        // Sin controles de juego: si no, la cámara seguiría el ratón con el menú abierto
        playerMapWasEnabled = playerMap.enabled;
        vehicleMapWasEnabled = vehicleMap.enabled;
        playerMap.Disable();
        vehicleMap.Disable();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        panel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(resumeButton.gameObject);
    }

    public void Resume()
    {
        if (!IsPaused) return;

        IsPaused = false;
        Time.timeScale = 1f;

        if (playerMapWasEnabled) playerMap.Enable();
        if (vehicleMapWasEnabled) vehicleMap.Enable();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        panel.SetActive(false);
    }

    public void BackToMainMenu()
    {
        // timeScale es global: si no lo devolvemos a 1, el menú (y la siguiente partida) seguirían congelados
        Time.timeScale = 1f;
        playerMap.Enable();
        vehicleMap.Enable();
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // Si se destruye estando en pausa (por ejemplo, al parar el Play), que no quede el tiempo congelado
    private void OnDestroy()
    {
        if (IsPaused) Time.timeScale = 1f;
    }
}
