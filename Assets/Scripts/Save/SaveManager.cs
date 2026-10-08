using UnityEngine;

// Conecta la partida guardada con los sistemas de la escena.
// - Al empezar: si se pidió "Continuar", carga el archivo y reparte los datos (hora, dinero, vehículos).
// - Al pasar la noche (evento de SleepController): reúne los datos y los guarda.
// Va con orden -50: su Start corre después de los Awake de todos (TimeManager, EconomyManager...)
// pero antes que los Start normales, así la UI y el generador de pedidos ya ven los datos cargados.
[DefaultExecutionOrder(-50)]
public class SaveManager : MonoBehaviour
{
    [SerializeField] private SleepController sleepController;

    [Tooltip("Solo para probar en el editor sin menú: al darle a Play directamente en esta escena, carga la partida guardada.")]
    [SerializeField] private bool loadWhenStartedInEditor;

    private void OnEnable() => sleepController.OnNightPassed += HandleNightPassed;
    private void OnDisable() => sleepController.OnNightPassed -= HandleNightPassed;

    private void Start()
    {
        bool wantsContinue = GameSession.StartedFromMenu ? GameSession.ContinueRequested : loadWhenStartedInEditor;
        if (wantsContinue) LoadGame();
    }

    private void HandleNightPassed()
    {
        SaveGame();
        sleepController.AddSummaryLine("Partida guardada");
    }

    public void SaveGame()
    {
        var data = new SaveData
        {
            day = TimeManager.Instance.Day,
            timeOfDay = TimeManager.Instance.TimeOfDay,
            money = EconomyManager.Instance.Money,
        };
        foreach (VehicleOwnership vehicle in VehicleOwnership.All)
        {
            if (vehicle.IsOwned) data.ownedVehicles.Add(vehicle.SaveId);
        }

        SaveSystem.Save(data);
        Debug.Log($"Partida guardada en {SaveSystem.FilePath}");
    }

    public bool LoadGame()
    {
        if (!SaveSystem.TryLoad(out SaveData data))
        {
            Debug.Log("No hay partida guardada: se empieza de cero.");
            return false;
        }

        TimeManager.Instance.LoadState(data.day, data.timeOfDay);
        EconomyManager.Instance.LoadState(data.money);
        foreach (VehicleOwnership vehicle in VehicleOwnership.All)
        {
            vehicle.LoadOwned(data.ownedVehicles.Contains(vehicle.SaveId));
        }

        Debug.Log($"Partida cargada: día {data.day}, {data.money} €");
        return true;
    }

    // Atajos de pruebas: clic derecho sobre el componente en el Inspector (en Play)
    [ContextMenu("Guardar ahora (pruebas)")]
    private void SaveNowForTesting() => SaveGame();

    [ContextMenu("Borrar partida guardada (pruebas)")]
    private void DeleteSaveForTesting()
    {
        SaveSystem.Delete();
        Debug.Log("Partida guardada borrada.");
    }
}
