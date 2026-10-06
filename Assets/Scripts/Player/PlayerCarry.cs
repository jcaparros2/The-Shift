using System;
using UnityEngine;
using UnityEngine.InputSystem;

// Lo que el jugador lleva en la mano: como mucho un paquete (GDD: a pie, 1 pequeño).
// Los paquetes se cogen con Interactuar (los avisa Package) y se sueltan con Drop (G).
public class PlayerCarry : MonoBehaviour
{
    [Tooltip("Punto delante de la cámara donde se ve el paquete que llevas.")]
    [SerializeField] private Transform holdPoint;

    [Tooltip("Tamaño máximo que se puede llevar en la mano.")]
    [SerializeField] private PackageSize maxSize = PackageSize.Small;

    [Tooltip("A qué distancia delante del jugador cae el paquete al soltarlo.")]
    [SerializeField] private float dropDistance = 0.8f;

    // Avisan al coger y soltar (para la UI, la bici, los pedidos...)
    public event Action<Package> OnPackagePickedUp;
    public event Action<Package> OnPackageDropped;

    public Package HeldPackage { get; private set; }
    public bool IsCarrying => HeldPackage != null;

    private InputAction dropAction;

    private void Awake()
    {
        dropAction = InputSystem.actions.FindAction("Player/Drop", throwIfNotFound: true);
    }

    private void Update()
    {
        if (IsCarrying && dropAction.WasPressedThisFrame())
        {
            Drop();
        }
    }

    public bool TryPickUp(Package package)
    {
        if (IsCarrying)
        {
            Debug.Log("Ya llevas un paquete en la mano. Suéltalo con G.");
            return false;
        }
        if (package.Data.size > maxSize)
        {
            Debug.Log($"{package.Data.displayName}: es demasiado grande para llevarlo en la mano.");
            return false;
        }

        HeldPackage = package;
        package.AttachTo(holdPoint);
        Debug.Log($"Has cogido: {package.Data.displayName} ({package.Data.basePay} €)");
        OnPackagePickedUp?.Invoke(package);
        return true;
    }

    public void Drop()
    {
        if (!IsCarrying) return;

        Package package = HeldPackage;
        HeldPackage = null;

        // Delante del jugador, en horizontal, y un poco en alto para que caiga al suelo
        Vector3 forward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
        Vector3 position = transform.position + forward * dropDistance + Vector3.up * 0.5f;
        package.Detach(position, Quaternion.LookRotation(forward));

        Debug.Log($"Has soltado: {package.Data.displayName}");
        OnPackageDropped?.Invoke(package);
    }
}
