using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

// Terminar el día durmiendo: fundido a negro, el reloj salta a la mañana siguiente,
// resumen de la jornada y vuelta a jugar. Los pedidos sin entregar se pierden.
public class SleepController : MonoBehaviour
{
    [Header("Horario")]
    [Tooltip("Desde esta hora se puede dormir (GDD: el turno de reparto acaba a las 18:00).")]
    [Range(0, 23)] [SerializeField] private int earliestSleepHour = 18;

    [Tooltip("Hasta esta hora de la madrugada se puede seguir durmiendo.")]
    [Range(0, 23)] [SerializeField] private int latestSleepHour = 6;

    [Tooltip("Hora de despertar (GDD: 07:00).")]
    [Range(0, 23)] [SerializeField] private int wakeUpHour = 7;

    [Header("Referencias")]
    [SerializeField] private PlayerJobs playerJobs;
    [SerializeField] private DayStats dayStats;
    [SerializeField] private ScreenFader fader;

    [Header("Tiempos (segundos reales)")]
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float summaryDuration = 3.5f;

    public bool IsSleeping { get; private set; }

    // ¿Es buena hora para dormir? De earliestSleepHour a medianoche, o de medianoche a latestSleepHour
    public bool CanSleepNow
    {
        get
        {
            int hour = TimeManager.Instance.Hour;
            return hour >= earliestSleepHour || hour < latestSleepHour;
        }
    }

    public string EarliestSleepText => TimeManager.FormatTime(earliestSleepHour);
    public string WakeUpText => TimeManager.FormatTime(wakeUpHour);

    public bool TrySleep()
    {
        if (IsSleeping) return false;
        if (!CanSleepNow)
        {
            PlayerMessages.Show($"Aún es pronto para dormir. Podrás a partir de las {EarliestSleepText}.");
            return false;
        }

        StartCoroutine(SleepRoutine());
        return true;
    }

    // Una corrutina puede "esperar" (yield) sin congelar el juego: aquí, a los fundidos y al resumen
    private IEnumerator SleepRoutine()
    {
        IsSleeping = true;
        TimeManager time = TimeManager.Instance;
        InputActionMap playerMap = InputSystem.actions.FindActionMap("Player", throwIfNotFound: true);

        // Mientras duermes no te mueves y el reloj no corre
        playerMap.Disable();
        time.IsPaused = true;

        yield return fader.Fade(1f, fadeDuration);

        int finishedDay = time.Day;
        int lostJobs = playerJobs.CancelAllJobs();
        fader.SetText(BuildSummary(finishedDay, lostJobs));
        dayStats.ResetDay();
        time.SetTime(wakeUpHour);

        yield return new WaitForSecondsRealtime(summaryDuration);

        fader.SetText("");
        time.IsPaused = false;
        yield return fader.Fade(0f, fadeDuration);

        playerMap.Enable();
        PlayerMessages.Show($"Día {time.Day} · {time.GetTimeText()}. ¡A trabajar!");
        IsSleeping = false;
    }

    private string BuildSummary(int day, int lostJobs)
    {
        string text = $"<size=140%><b>Día {day} terminado</b></size>\n\n" +
                      $"Ganado con entregas: {dayStats.Earned} €\n" +
                      $"Entregas: {dayStats.Deliveries}";
        if (dayStats.LateDeliveries > 0) text += $" ({dayStats.LateDeliveries} tarde)";
        if (lostJobs > 0) text += $"\nPedidos sin entregar: {lostJobs} (vuelven al almacén)";
        return text;
    }
}
