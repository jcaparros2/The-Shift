using System;
using UnityEngine;

// La carga de un vehículo (la caja trasera de la bici, el maletero...).
// Mirándola con el botón de interactuar: si llevas un paquete en la mano lo carga en el primer
// hueco libre; si no, descarga el del último hueco ocupado y te lo pone en la mano.
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

    public int Capacity => Mathf.Min(slots.Length, vehicle.Data.cargoSlots);
    public bool IsFull => LoadedCount >= Capacity;

    // No guardamos una lista aparte: lo cargado es lo que hay encima de cada hueco.
    // Así, si un paquete desaparece (por ejemplo, al cancelarse su pedido), su hueco queda libre solo.
    public int LoadedCount
    {
        get
        {
            int count = 0;
            for (int i = 0; i < Capacity; i++)
            {
                if (PackageIn(i) != null) count++;
            }
            return count;
        }
    }

    private Package PackageIn(int slotIndex) => slots[slotIndex].GetComponentInChildren<Package>();

    // El texto cambia según lo que haría E: cargar si llevas algo en la mano, descargar si no
    public string GetInteractionPrompt(GameObject interactor)
    {
        if (IsForSale) return ownership.BuyPrompt;

        int loaded = LoadedCount;
        string count = $"({loaded}/{Capacity})";
        bool carrying = interactor.TryGetComponent(out PlayerCarry carry) && carry.IsCarrying;

        if (carrying)
        {
            return IsFull ? $"Carga llena {count}" : $"Cargar paquete {count}";
        }
        return loaded > 0 ? $"Descargar paquete {count}" : $"Carga vacía {count}";
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

        // Primer hueco libre
        int freeSlot = 0;
        while (PackageIn(freeSlot) != null) freeSlot++;

        carry.ReleaseHeldPackage();
        package.AttachTo(slots[freeSlot]);

        PlayerMessages.Show($"Cargado: {package.Data.displayName} ({LoadedCount}/{Capacity})");
        OnCargoChanged?.Invoke();
        return true;
    }

    // Pasa a la mano del jugador el paquete del último hueco ocupado
    public bool TryUnload(PlayerCarry carry)
    {
        for (int i = Capacity - 1; i >= 0; i--)
        {
            Package package = PackageIn(i);
            if (package == null) continue;

            // Si no cabe en la mano (por ejemplo, uno mediano del coche), se queda donde estaba
            if (!carry.TryPickUp(package)) return false;

            OnCargoChanged?.Invoke();
            return true;
        }

        PlayerMessages.Show("La carga está vacía.");
        return false;
    }
}
