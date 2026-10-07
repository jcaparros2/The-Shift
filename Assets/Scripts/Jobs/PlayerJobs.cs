using System;
using System.Collections.Generic;
using UnityEngine;

// Los pedidos que ha aceptado el jugador.
// Escucha a PlayerCarry: cuando coges un paquete cuyo pedido aún está disponible, lo acepta.
// Así PlayerCarry solo se ocupa de la mano y este script, de los pedidos.
public class PlayerJobs : MonoBehaviour
{
    [SerializeField] private PlayerCarry carry;

    // Avisa cuando la lista cambia (para la UI y, más adelante, para entregar)
    public event Action OnJobsChanged;

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
}
