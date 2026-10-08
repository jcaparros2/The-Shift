using System;
using UnityEngine;

// Un vehículo que solo se puede usar alquilado (el taxi de CityCab).
// Al acabar el alquiler vuelve solo a su sitio: al momento si nadie lo conduce,
// o en cuanto el conductor baje si lo está conduciendo.
[RequireComponent(typeof(VehicleController))]
public class VehicleRental : MonoBehaviour
{
    [Tooltip("Sitio de la parada al que vuelve el vehículo al devolverlo.")]
    [SerializeField] private Transform parkingSpot;

    // Avisan al alquilarlo y al devolverlo (para la UI, los viajes de taxi...)
    public event Action OnRented;
    public event Action OnReturned;

    public bool IsRented { get; private set; }

    // Hasta cuándo dura el alquiler, en horas totales (TimeManager.TotalHours)
    public float RentedUntil { get; private set; }

    private VehicleController vehicle;
    private Rigidbody rb;
    private bool returnWarningShown;

    private void Awake()
    {
        vehicle = GetComponent<VehicleController>();
        rb = GetComponent<Rigidbody>();
    }

    public void Rent(float untilTotalHours)
    {
        IsRented = true;
        RentedUntil = untilTotalHours;
        returnWarningShown = false;
        OnRented?.Invoke();
    }

    private void Update()
    {
        // Se compara con horas totales: funciona aunque el reloj salte (por ejemplo, al dormir)
        if (!IsRented || TimeManager.Instance.TotalHours < RentedUntil) return;

        if (vehicle.IsDriven)
        {
            if (!returnWarningShown)
            {
                PlayerMessages.Show("Se acabó el alquiler: baja del taxi y volverá a la parada.");
                returnWarningShown = true;
            }
            return;
        }

        ReturnToParking();
    }

    // Lo deja en su sitio de la parada, parado, y sin alquilar
    public void ReturnToParking()
    {
        if (!IsRented) return;

        IsRented = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.position = parkingSpot.position;
        rb.rotation = parkingSpot.rotation;
        transform.SetPositionAndRotation(parkingSpot.position, parkingSpot.rotation);

        PlayerMessages.Show("El taxi ha vuelto a la parada de CityCab.");
        OnReturned?.Invoke();
    }
}
