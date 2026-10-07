using UnityEngine;

// Cuenta lo que haces en una jornada (de despertar a dormir): lo ganado con entregas y cuántas.
// Escucha el evento OnJobDelivered de PlayerJobs. SleepController lo lee para el resumen y lo reinicia.
// La jornada no termina a medianoche sino al dormir, por eso no se reinicia con OnDayChanged.
public class DayStats : MonoBehaviour
{
    [SerializeField] private PlayerJobs playerJobs;

    public int Earned { get; private set; }
    public int Deliveries { get; private set; }
    public int LateDeliveries { get; private set; }

    private void OnEnable() => playerJobs.OnJobDelivered += HandleJobDelivered;
    private void OnDisable() => playerJobs.OnJobDelivered -= HandleJobDelivered;

    private void HandleJobDelivered(DeliveryJob job, int total)
    {
        Earned += total;
        Deliveries++;
        if (TimeManager.Instance.TotalHours > job.Deadline) LateDeliveries++;
    }

    public void ResetDay()
    {
        Earned = 0;
        Deliveries = 0;
        LateDeliveries = 0;
    }
}
