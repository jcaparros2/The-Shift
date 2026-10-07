using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Un destino de entrega: una puerta con su dirección.
// Se apunta solo a una lista común (All) al activarse, así el generador de pedidos
// encuentra todos los destinos de la escena sin tener que arrastrarlos uno a uno.
public class DeliveryPoint : MonoBehaviour
{
    [SerializeField] private string address = "Calle Mayor 1";

    [Tooltip("Cartel con la dirección (opcional). Se rellena solo con el campo Address.")]
    [SerializeField] private TMP_Text label;

    public string Address => address;

    private static readonly List<DeliveryPoint> all = new List<DeliveryPoint>();
    public static IReadOnlyList<DeliveryPoint> All => all;

    private void OnEnable() => all.Add(this);
    private void OnDisable() => all.Remove(this);

    // OnValidate se llama en el editor al cambiar un valor: así el cartel siempre coincide con la dirección
    private void OnValidate()
    {
        if (label != null) label.text = address;
    }
}
