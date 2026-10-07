using UnityEngine;

// Algo que se puede comprar (GDD: ShopItemData con precio, efecto y nivel requerido).
// De momento solo nombre, precio y descripción; el efecto lo pone quien lo usa
// (por ejemplo, VehicleOwnership hace que el vehículo pase a ser tuyo).
[CreateAssetMenu(fileName = "NewShopItem", menuName = "Doble Turno/Shop Item Data")]
public class ShopItemData : ScriptableObject
{
    public string displayName = "Bicicleta";

    [Min(0)] public int price = 250;

    [TextArea] public string description;
}
