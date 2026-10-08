using System;
using UnityEngine;
using UnityEngine.InputSystem;

// Lanza un rayo desde la cámara hacia donde mira el jugador. Si da con un IInteractable
// cercano lo marca como "objetivo", y al pulsar Interactuar (E / botón norte) lo usa.
// Va en el objeto Player, separado de PlayerController: mover y interactuar son sistemas distintos.
public class PlayerInteractor : MonoBehaviour
{
    [Tooltip("Desde dónde sale el rayo. Normalmente la cámara (o el CameraTarget).")]
    [SerializeField] private Transform rayOrigin;

    [Tooltip("Distancia máxima a la que se puede interactuar, en metros.")]
    [SerializeField] private float interactDistance = 2.5f;

    [Tooltip("Capas que el rayo tiene en cuenta.")]
    [SerializeField] private LayerMask interactLayers = ~0;

    // Avisa cuando cambia el objeto al que miras (null = ninguno). Lo usará la UI para el texto de ayuda.
    public event Action<IInteractable> OnTargetChanged;

    public IInteractable CurrentTarget { get; private set; }

    private InputAction interactAction;

    private void Awake()
    {
        interactAction = InputSystem.actions.FindAction("Player/Interact", throwIfNotFound: true);
    }

    // Al desactivarse (por ejemplo, al subir a un vehículo) ya no mira a nada
    private void OnDisable()
    {
        SetTarget(null);
    }

    private void Update()
    {
        UpdateTarget();

        // WasPressedThisFrame ignora la interacción "Hold" que trae la acción por defecto,
        // así que basta con un toque de la tecla.
        if (CurrentTarget != null && interactAction.WasPressedThisFrame())
        {
            CurrentTarget.Interact(gameObject);
        }
    }

    private void UpdateTarget()
    {
        IInteractable newTarget = null;

        // QueryTriggerInteraction.Ignore: los triggers (zonas invisibles) no tapan el rayo
        if (Physics.Raycast(rayOrigin.position, rayOrigin.forward, out RaycastHit hit,
                            interactDistance, interactLayers, QueryTriggerInteraction.Ignore))
        {
            // GetComponentInParent permite que el collider esté en un hijo del objeto interactuable
            newTarget = hit.collider.GetComponentInParent<IInteractable>();
        }

        SetTarget(newTarget);
    }

    private void SetTarget(IInteractable newTarget)
    {
        if (newTarget != CurrentTarget)
        {
            CurrentTarget = newTarget;
            OnTargetChanged?.Invoke(CurrentTarget);
        }
    }

    // Dibuja el rayo en la vista Scene para ver su alcance
    private void OnDrawGizmosSelected()
    {
        if (rayOrigin == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(rayOrigin.position, rayOrigin.forward * interactDistance);
    }
}
