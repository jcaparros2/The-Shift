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

    [Tooltip("Opcional: si el vehículo puede estar en venta. Mientras lo esté, no se puede cargar.")]
    [SerializeField] private VehicleOwnership ownership;

    private bool IsForSale => ownership != null && !ownership.IsOwned;

    // Avisa cuando se carga o descarga algo (para la UI, los pedidos...)
    public event Action OnCargoChanged;

    private readonly List<Package> packages = new List<Package>();

    public IReadOnlyList<Package> Packages => packages;
    public int Capacity => Mathf.Min(slots.Length, vehicle.Data.cargoSlots);
    public bool IsFull => packages.Count >= Capacity;

    // El texto cambia según lo que haría E: cargar si llevas algo en la mano, descargar si no
    public string GetInteractionPrompt(GameObject interactor)
    {
        if (IsForSale) return ownership.BuyPrompt;

        string count = $"({packages.Count}/{Capacity})";
        bool carrying = interactor.TryGetComponent(out PlayerCarry carry) && carry.IsCarrying;

        if (carrying)
        {
            return IsFull ? $"Carga llena {count}" : $"Cargar paquete {count}";
        }
        return packages.Count > 0 ? $"Descargar paquete {count}" : $"Carga vacía {count}";
    }

    public void Interact(GameObject interactor)
    {
        // En venta: interactuar con la caja también es comprarlo
        if (IsForSale)
        {
            ownership.TryBuy();
            return;
        }

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
            PlayerMessages.Show($"{package.Data.displayName}: no cabe en este vehículo.");
            return false;
        }
        if (IsFull)
        {
            PlayerMessages.Show($"La carga está llena ({Capacity}/{Capacity}).");
            return false;
        }

        carry.ReleaseHeldPackage();
        packages.Add(package);
        package.AttachTo(slots[packages.Count - 1]);

        PlayerMessages.Show($"Cargado: {package.Data.displayName} ({packages.Count}/{Capacity})");
        OnCargoChanged?.Invoke();
        return true;
    }

    // Pasa el último paquete cargado a la mano del jugador
    public bool TryUnload(PlayerCarry carry)
    {
        if (packages.Count == 0)
        {
            PlayerMessages.Show("La carga está vacía.");
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
