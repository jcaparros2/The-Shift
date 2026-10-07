using System;
using System.Collections.Generic;
using UnityEngine;

// Los pedidos que ha aceptado el jugador, y su entrega.
// Escucha a PlayerCarry: cuando coges un paquete cuyo pedido aún está disponible, lo acepta.
// Al entregar en la puerta correcta calcula el pago (con propina o penalización) y cobra.
public class PlayerJobs : MonoBehaviour
{
    [SerializeField] private PlayerCarry carry;

    [Tooltip("Los mismos ajustes que usa el generador: aquí se leen la propina y la penalización.")]
    [SerializeField] private DeliveryJobSettings settings;

    // Avisa cuando la lista cambia (para la UI)
    public event Action OnJobsChanged;

    // Avisa de cada entrega con lo cobrado en total (para estadísticas, historias...)
    public event Action<DeliveryJob, int> OnJobDelivered;

    private readonly List<DeliveryJob> activeJobs = new List<DeliveryJob>();
    public IReadOnlyList<DeliveryJob> ActiveJobs => activeJobs;

    private void OnEnable() => carry.OnHeldPackageChanged += HandleHeldPackageChanged;
    private void OnDisable() => carry.OnHeldPackageChanged -= HandleHeldPackageChanged;

    private void HandleHeldPackageChanged(Package package)
    {
        DeliveryJob job = package != null ? package.Job : null;
        if (job == null || job.Status != DeliveryJobStatus.Available) return;

        job.Accept();
        activeJobs.Add(job);
        PlayerMessages.Show($"Pedido aceptado: {job.Destination.Address}, antes de {job.DeadlineText}");
        OnJobsChanged?.Invoke();
    }

    // ¿Tienes algún pedido para este destino? (para el texto de la puerta)
    public bool HasJobFor(DeliveryPoint point)
    {
        foreach (DeliveryJob job in activeJobs)
        {
            if (job.Destination == point) return true;
        }
        return false;
    }

    // Entrega el paquete que llevas en la mano en esta puerta, si es su destino
    public bool TryDeliver(DeliveryPoint point)
    {
        Package package = carry.HeldPackage;
        DeliveryJob job = package != null ? package.Job : null;

        if (job == null || !activeJobs.Contains(job))
        {
            PlayerMessages.Show(HasJobFor(point) ? "Trae en la mano el paquete para esta dirección."
                                                 : "No tienes ningún pedido para esta dirección.");
            return false;
        }
        if (job.Destination != point)
        {
            PlayerMessages.Show($"Este paquete va a {job.Destination.Address}.");
            return false;
        }

        // Pago: a tiempo, pago + propina; tarde, pago reducido según las horas de retraso
        float hoursLate = TimeManager.Instance.TotalHours - job.Deadline;
        int basePay;
        int tip;
        if (hoursLate <= 0f)
        {
            basePay = job.Pay;
            tip = Mathf.Min(Mathf.RoundToInt(job.Pay * settings.onTimeTipPercent), settings.maxTip);
        }
        else
        {
            float payPercent = Mathf.Max(1f - hoursLate * settings.latePenaltyPerHour, settings.minLatePayPercent);
            basePay = Mathf.RoundToInt(job.Pay * payPercent);
            tip = 0;
        }
        int total = basePay + tip;

        // El paquete pasa al cliente: deja de existir en el mundo
        carry.ReleaseHeldPackage();
        Destroy(package.gameObject);

        job.Complete();
        activeJobs.Remove(job);
        EconomyManager.Instance.AddMoney(total);

        PlayerMessages.Show(hoursLate <= 0f
            ? $"Entregado: +{total} € ({basePay} € + {tip} € de propina)"
            : $"Entregado tarde: +{total} € (de {job.Pay} €, sin propina)");

        OnJobDelivered?.Invoke(job, total);
        OnJobsChanged?.Invoke();
        return true;
    }
}
