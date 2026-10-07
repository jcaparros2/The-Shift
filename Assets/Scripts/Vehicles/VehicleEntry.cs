using Unity.Cinemachine;
using UnityEngine;

// Hace que un vehículo se pueda usar con el botón de interactuar ("Subir a la bici").
// Solo sabe dónde se sienta el conductor y por dónde se baja; el cambio de estado
// del jugador (dejar de caminar, controles del vehículo...) lo hace PlayerVehicleHandler.
public class VehicleEntry : MonoBehaviour, IInteractable
{
    [SerializeField] private VehicleController vehicle;

    [Tooltip("Punto donde se colocan los pies del conductor. Hijo del vehículo, mirando hacia delante.")]
    [SerializeField] private Transform seat;

    [Tooltip("Cámara de Cinemachine que sigue a este vehículo mientras se conduce (ya apuntando a él).")]
    [SerializeField] private CinemachineCamera driverCamera;

    [SerializeField] private string prompt = "Subir";

    [Tooltip("A qué distancia del centro del vehículo se baja el conductor (por la izquierda).")]
    [SerializeField] private float exitSideDistance = 1.2f;

    public VehicleController Vehicle => vehicle;
    public Transform Seat => seat;
    public CinemachineCamera DriverCamera => driverCamera;
    public string GetInteractionPrompt(GameObject interactor) => prompt;

    public void Interact(GameObject interactor)
    {
        // Solo el jugador sabe subirse a vehículos; cualquier otro objeto no hace nada
        if (interactor.TryGetComponent(out PlayerVehicleHandler handler))
        {
            handler.EnterVehicle(this);
        }
    }

    // Punto del suelo junto al vehículo donde dejar al conductor al bajar
    public Vector3 GetExitPosition()
    {
        Vector3 side = transform.position - transform.right * exitSideDistance;

        // Busca el suelo bajando desde un poco más arriba, por si el vehículo está en una cuesta
        if (Physics.Raycast(side + Vector3.up * 2f, Vector3.down, out RaycastHit hit, 5f,
                            ~0, QueryTriggerInteraction.Ignore))
        {
            return hit.point;
        }
        return side;
    }
}
