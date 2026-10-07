using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Menú principal: Continuar (si hay partida guardada), Nueva partida y Salir.
// Los botones llaman a estos métodos (se conectan en el Inspector, en el evento OnClick de cada botón).
// Antes de cambiar de escena rellena GameSession, que es lo que lee SaveManager al empezar.
public class MainMenu : MonoBehaviour
{
    [Tooltip("Nombre de la escena del juego (debe estar en File > Build Profiles > Scene List).")]
    [SerializeField] private string gameSceneName = "SampleScene";

    [SerializeField] private Button continueButton;
    [SerializeField] private Button newGameButton;

    private void Start()
    {
        // El juego bloquea y oculta el ratón; en el menú tiene que verse
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Sin partida guardada no se puede continuar
        continueButton.interactable = SaveSystem.HasSave;

        // Primer botón seleccionado: así el menú también se usa con teclado o mando
        Button first = continueButton.interactable ? continueButton : newGameButton;
        EventSystem.current.SetSelectedGameObject(first.gameObject);
    }

    public void ContinueGame() => StartGame(continueSave: true);

    public void NewGame() => StartGame(continueSave: false);

    public void QuitGame()
    {
#if UNITY_EDITOR
        // En el editor no se puede "cerrar el juego": paramos el Play
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void StartGame(bool continueSave)
    {
        GameSession.StartedFromMenu = true;
        GameSession.ContinueRequested = continueSave;
        SceneManager.LoadScene(gameSceneName);
    }
}
