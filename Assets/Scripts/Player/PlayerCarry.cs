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

    // Avisa cuando cambia lo que llevas en la mano: el paquete nuevo, o null si ya no llevas nada
    public event Action<Package> OnHeldPackageChanged;

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
            PlayerMessages.Show("Ya llevas un paquete en la mano. Suéltalo con G.");
            return false;
        }
        if (package.Data.size > maxSize)
        {
            PlayerMessages.Show($"{package.Data.displayName}: es demasiado grande para llevarlo en la mano.");
            return false;
        }

        HeldPackage = package;
        package.AttachTo(holdPoint);
        PlayerMessages.Show($"Has cogido: {package.Data.displayName} ({package.Data.basePay} €)");
        OnHeldPackageChanged?.Invoke(package);
        return true;
    }

    // Suelta el paquete al suelo, delante del jugador
    public void Drop()
    {
        Package package = ReleaseHeldPackage();
        if (package == null) return;

        // En horizontal y un poco en alto para que caiga al suelo
        Vector3 forward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
        Vector3 position = transform.position + forward * dropDistance + Vector3.up * 0.5f;
        package.Detach(position, Quaternion.LookRotation(forward));

        PlayerMessages.Show($"Has soltado: {package.Data.displayName}");
    }

    // Deja de llevar el paquete y lo devuelve, para que otro lo coloque (la carga de un vehículo...)
    public Package ReleaseHeldPackage()
    {
        Package package = HeldPackage;
        if (package == null) return null;

        HeldPackage = null;
        OnHeldPackageChanged?.Invoke(null);
        return package;
    }
}
