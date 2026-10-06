using Unity.Cinemachine;
using UnityEngine;

// Elige qué cámara de Cinemachine se usa: primera persona a pie, y la del vehículo
// (por detrás) al conducir. Escucha los eventos de PlayerVehicleHandler, así ninguno
// de los dos sistemas necesita saber cómo funciona el otro.
// Cinemachine usa la cámara con más prioridad y hace la transición entre ellas sola.
public class PlayerCameraSwitcher : MonoBehaviour
{
    [SerializeField] private PlayerVehicleHandler vehicleHandler;

    [Tooltip("Cámara en los ojos del jugador.")]
    [SerializeField] private CinemachineCamera firstPersonCamera;

    // La cámara activa tiene más prioridad que el resto
    private const int ActivePriority = 20;
    private const int InactivePriority = 0;

    // Cámara del vehículo que conducimos ahora (null si vamos a pie)
    private CinemachineCamera currentVehicleCamera;

    private void OnEnable()
    {
        vehicleHandler.OnVehicleEntered += HandleVehicleEntered;
        vehicleHandler.OnVehicleExited += HandleVehicleExited;
    }

    private void OnDisable()
    {
        vehicleHandler.OnVehicleEntered -= HandleVehicleEntered;
        vehicleHandler.OnVehicleExited -= HandleVehicleExited;
    }

    private void Start()
    {
        // Empezamos a pie
        firstPersonCamera.Priority = ActivePriority;
    }

    private void HandleVehicleEntered(VehicleEntry entry)
    {
        // Cada vehículo trae su propia cámara, ya configurada para seguirlo
        currentVehicleCamera = entry.DriverCamera;
        if (currentVehicleCamera == null)
        {
            Debug.LogWarning($"{entry.name} no tiene Driver Camera; se queda la primera persona.", entry);
            return;
        }

        currentVehicleCamera.Priority = ActivePriority;
        firstPersonCamera.Priority = InactivePriority;
    }

    private void HandleVehicleExited(VehicleEntry entry)
    {
        if (currentVehicleCamera != null)
        {
            currentVehicleCamera.Priority = InactivePriority;
            currentVehicleCamera = null;
        }
        firstPersonCamera.Priority = ActivePriority;
    }
}
