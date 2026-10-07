using System;
using System.Collections.Generic;
using UnityEngine;

// La carga de un vehículo (la caja trasera de la bici, el maletero...).
// Mirándola con el botón de interactuar: si llevas un paquete en la mano lo carga;
// si no, descarga el último que se cargó y te lo pone en la mano.
// La capacidad sale de VehicleData (cargoSlots y maxPackageSize).
public class VehicleCargo : MonoBehaviour, IInteractable
{
    [SerializeField] private VehicleController vehicle;

    [Tooltip("Puntos donde se colocan los paquetes, en orden. Debe haber al menos tantos como cargoSlots.")]
    [SerializeField] private Transform[] slots;

    // Avisa cuando se carga o descarga algo (para la UI, los pedidos...)
    public event Action OnCargoChanged;

    private readonly List<Package> packages = new List<Package>();

    public IReadOnlyList<Package> Packages => packages;
    public int Capacity => Mathf.Min(slots.Length, vehicle.Data.cargoSlots);
    public bool IsFull => packages.Count >= Capacity;

    public string InteractionPrompt => $"Carga {packages.Count}/{Capacity} (cargar o descargar)";

    public void Interact(GameObject interactor)
    {
        if (!interactor.TryGetComponent(out PlayerCarry carry)) return;

        if (carry.IsCarrying)
        {
            TryLoad(carry);
        }
        else
        {
            TryUnload(carry);
        }
    }

    // Pasa el paquete de la mano del jugador a la carga
    public bool TryLoad(PlayerCarry carry)
    {
        Package package = carry.HeldPackage;

        if (package.Data.size > vehicle.Data.maxPackageSize)
        {
            Debug.Log($"{package.Data.displayName}: no cabe en este vehículo.");
            return false;
        }
        if (IsFull)
        {
            Debug.Log($"La carga está llena ({Capacity}/{Capacity}).");
            return false;
        }

        carry.ReleaseHeldPackage();
        packages.Add(package);
        package.AttachTo(slots[packages.Count - 1]);

        Debug.Log($"Cargado: {package.Data.displayName} ({packages.Count}/{Capacity})");
        OnCargoChanged?.Invoke();
        return true;
    }

    // Pasa el último paquete cargado a la mano del jugador
    public bool TryUnload(PlayerCarry carry)
    {
        if (packages.Count == 0)
        {
            Debug.Log("La carga está vacía.");
            return false;
        }

        Package package = packages[packages.Count - 1];
        if (!carry.TryPickUp(package))
        {
            // No cabe en la mano (por ejemplo, uno mediano del coche): se queda donde estaba
            return false;
        }

        packages.RemoveAt(packages.Count - 1);
        OnCargoChanged?.Invoke();
        return true;
    }
}
