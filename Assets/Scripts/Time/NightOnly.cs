using UnityEngine;

// Enciende unos objetos solo de noche (luces y neones de bares, farolas...) y los apaga de día.
// Escucha el evento OnHourChanged del TimeManager, así no tiene que mirar la hora cada frame.
public class NightOnly : MonoBehaviour
{
    [Tooltip("Lo que se enciende de noche.")]
    [SerializeField] private GameObject[] targets;

    [Tooltip("Hora a la que se encienden.")]
    [Range(0, 23)] [SerializeField] private int onHour = 20;

    [Tooltip("Hora a la que se apagan (de madrugada).")]
    [Range(0, 23)] [SerializeField] private int offHour = 7;

    private void Start()
    {
        // En Start: TimeManager ya existe y, si se cargó partida, ya tiene la hora buena
        TimeManager.Instance.OnHourChanged += Apply;
        Apply(TimeManager.Instance.Hour);
    }

    private void OnDestroy()
    {
        if (TimeManager.Instance != null) TimeManager.Instance.OnHourChanged -= Apply;
    }

    private void Apply(int hour)
    {
        // La noche cruza la medianoche: de onHour a 24 y de 0 a offHour
        bool night = hour >= onHour || hour < offHour;
        foreach (GameObject target in targets)
        {
            if (target != null) target.SetActive(night);
        }
    }
}
