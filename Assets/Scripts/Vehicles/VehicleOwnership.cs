using System;
using System.Collections.Generic;
using UnityEngine;

// Si un vehículo es del jugador o está en venta.
// Mientras está en venta, VehicleEntry y VehicleCargo ofrecen comprarlo en vez de usarlo.
public class VehicleOwnership : MonoBehaviour
{
    [Tooltip("Qué es y cuánto cuesta.")]
    [SerializeField] private ShopItemData shopItem;

    [Tooltip("Si ya es tuyo al empezar la partida.")]
    [SerializeField] private bool ownedAtStart;

    // Avisa al comprarlo (para desbloquear pedidos más lejanos, la UI...)
    public event Action<VehicleOwnership> OnPurchased;

    public bool IsOwned { get; private set; }
    public ShopItemData ShopItem => shopItem;

    // Identificador para la partida guardada: el nombre del artículo de tienda ("Bike")
    public string SaveId => shopItem.name;

    // Marca el vehículo como propio al cargar una partida, sin cobrar ni avisar
    public void LoadOwned(bool owned)
    {
        IsOwned = owned;
    }

    // Todos los vehículos con propietario de la escena (como DeliveryPoint.All), para saber cuáles tienes
    private static readonly List<VehicleOwnership> all = new List<VehicleOwnership>();
    public static IReadOnlyList<VehicleOwnership> All => all;

    private void OnEnable() => all.Add(this);
    private void OnDisable() => all.Remove(this);

    // El mejor alcance de pedidos entre los vehículos que tienes (0 si no tienes ninguno)
    public static float BestOwnedDeliveryRange()
    {
        float best = 0f;
        foreach (VehicleOwnership vehicle in all)
        {
            if (vehicle.IsOwned && vehicle.TryGetComponent(out VehicleController controller))
            {
                best = Mathf.Max(best, controller.Data.deliveryRange);
            }
        }
        return best;
    }

    public string BuyPrompt => $"Comprar {shopItem.displayName} · {shopItem.price} €";

    private void Awake()
    {
        IsOwned = ownedAtStart;
    }

    public bool TryBuy()
    {
        if (IsOwned) return true;

        if (!EconomyManager.Instance.TrySpend(shopItem.price))
        {
            int missing = shopItem.price - EconomyManager.Instance.Money;
            PlayerMessages.Show($"No te llega: te faltan {missing} € para {shopItem.displayName}.");
            return false;
        }

        IsOwned = true;
        PlayerMessages.Show($"Has comprado: {shopItem.displayName}. Te llegarán pedidos más lejanos.");
        OnPurchased?.Invoke(this);
        return true;
    }
}
