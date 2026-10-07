using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

// Genera los pedidos de reparto del almacén QuickDrop.
// Al empezar el turno y cada hora del turno, rellena huecos libres de las estanterías
// con paquetes-pedido (destino, pago y hora límite al azar). Los que caducan sin que
// nadie los coja desaparecen. Escucha a TimeManager por su evento OnHourChanged.
public class DeliveryJobGenerator : MonoBehaviour
{
    [SerializeField] private DeliveryJobSettings settings;

    [Tooltip("Desde dónde se mide la distancia a los destinos (el almacén).")]
    [SerializeField] private Transform origin;

    [Tooltip("Huecos de las estanterías donde aparecen los paquetes. Su origen debe estar a ras de la balda.")]
    [SerializeField] private Transform[] slots;

    // Avisan cuando aparece un pedido nuevo o caduca uno (para la UI, el móvil...)
    public event Action<DeliveryJob> OnJobCreated;
    public event Action<DeliveryJob> OnJobExpired;

    private void Start()
    {
        TimeManager.Instance.OnHourChanged += HandleHourChanged;

        // Si la partida empieza ya dentro del turno, que haya pedidos desde el principio
        if (IsShiftHour(TimeManager.Instance.Hour)) GenerateJobs(settings.jobsPerHour);
    }

    private void OnDestroy()
    {
        if (TimeManager.Instance != null) TimeManager.Instance.OnHourChanged -= HandleHourChanged;
    }

    private void Update()
    {
        ExpireOldJobs();
    }

    private void HandleHourChanged(int hour)
    {
        if (IsShiftHour(hour)) GenerateJobs(settings.jobsPerHour);
    }

    private bool IsShiftHour(int hour) => hour >= settings.shiftStartHour && hour < settings.shiftEndHour;

    private void GenerateJobs(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Transform slot = PickFreeSlot();
            if (slot == null) return;   // Estanterías llenas
            CreateJob(slot);
        }
    }

    private void CreateJob(Transform slot)
    {
        DeliveryPoint destination = PickDestinationInRange();
        if (destination == null || settings.packageTypes.Length == 0)
        {
            Debug.LogWarning("No hay destinos a tu alcance o tipos de paquete: no se pueden crear pedidos.", this);
            return;
        }

        PackageData data = settings.packageTypes[Random.Range(0, settings.packageTypes.Length)];
        float distance = Vector3.Distance(origin.position, destination.transform.position);
        bool urgent = Random.value < settings.urgentChance;

        // Pago: base del paquete + extra por distancia, y más si es urgente
        float pay = data.basePay + distance * settings.payPerMeter;
        if (urgent) pay *= settings.urgentPayMultiplier;

        // Hora límite: ahora + lo que se tarda en llegar + un margen; redondeada al cuarto de hora siguiente
        float travelHours = distance / settings.estimatedSpeed / TimeManager.Instance.RealSecondsPerGameHour;
        float margin = urgent ? settings.urgentMarginHours : settings.normalMarginHours;
        float deadline = Mathf.Ceil((TimeManager.Instance.TotalHours + travelHours + margin) * 4f) / 4f;

        var job = new DeliveryJob(data, destination, Mathf.RoundToInt(pay), deadline, urgent);

        // El paquete físico, apoyado en la balda (su centro, medio paquete por encima del hueco)
        Package package = Instantiate(data.prefab, slot);
        package.transform.localPosition = Vector3.up * package.transform.localScale.y * 0.5f;
        package.transform.localRotation = Quaternion.identity;
        package.AssignJob(job);

        OnJobCreated?.Invoke(job);
    }

    // Destino al azar entre los que están a tu alcance: a pie, cerca; con vehículo, según su deliveryRange
    private DeliveryPoint PickDestinationInRange()
    {
        float range = Mathf.Max(settings.onFootRange, VehicleOwnership.BestOwnedDeliveryRange());

        var inRange = new List<DeliveryPoint>();
        foreach (DeliveryPoint point in DeliveryPoint.All)
        {
            if (Vector3.Distance(origin.position, point.transform.position) <= range) inRange.Add(point);
        }
        return inRange.Count > 0 ? inRange[Random.Range(0, inRange.Count)] : null;
    }

    // Un hueco está libre si no tiene ningún paquete encima (al cogerlo, el paquete deja de ser su hijo)
    private Transform PickFreeSlot()
    {
        var free = new List<Transform>();
        foreach (Transform slot in slots)
        {
            if (slot.GetComponentInChildren<Package>() == null) free.Add(slot);
        }
        return free.Count > 0 ? free[Random.Range(0, free.Count)] : null;
    }

    private void ExpireOldJobs()
    {
        float now = TimeManager.Instance.TotalHours;
        foreach (Transform slot in slots)
        {
            Package package = slot.GetComponentInChildren<Package>();
            if (package == null || package.Job == null) continue;
            if (package.Job.Status != DeliveryJobStatus.Available || now <= package.Job.Deadline) continue;

            package.Job.Expire();
            OnJobExpired?.Invoke(package.Job);
            Destroy(package.gameObject);
        }
    }
}
