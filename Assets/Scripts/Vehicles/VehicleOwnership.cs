using System;
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
        PlayerMessages.Show($"Has comprado: {shopItem.displayName}");
        OnPurchased?.Invoke(this);
        return true;
    }
}
