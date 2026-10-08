using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Un destino de entrega: una puerta con su dirección (y, si quieres, el nombre del sitio).
// Se apunta solo a una lista común (All) al activarse, así el generador de pedidos
// encuentra todos los destinos de la escena sin tener que arrastrarlos uno a uno.
// Mirando la puerta con Interactuar se entrega el paquete que llevas en la mano.
public class DeliveryPoint : MonoBehaviour, IInteractable
{
    [SerializeField] private string address = "Calle Mayor 1";

    [Tooltip("Opcional: nombre del sitio (\"Tienda de Juan\", \"Casa de los García\"...).")]
    [SerializeField] private string placeName = "";

    [Tooltip("Cartel con el nombre y la dirección (opcional). Se rellena solo.")]
    [SerializeField] private TMP_Text label;

    public string Address => address;
    public string PlaceName => placeName;

    // Para mostrar al jugador: "Tienda de Juan (Calle del Río 3)", o solo la dirección si no tiene nombre
    public string DisplayName => string.IsNullOrEmpty(placeName) ? address : $"{placeName} ({address})";

    private static readonly List<DeliveryPoint> all = new List<DeliveryPoint>();
    public static IReadOnlyList<DeliveryPoint> All => all;

    // Sin recarga de código al darle a Play, la lista podría conservar restos de un Play anterior
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetOnPlay() => all.Clear();

    private void OnEnable() => all.Add(this);
    private void OnDisable() => all.Remove(this);

    // OnValidate se llama en el editor al cambiar un valor: así el cartel siempre coincide con los datos
    private void OnValidate()
    {
        if (label == null) return;
        label.text = string.IsNullOrEmpty(placeName)
            ? address
            : $"<b>{placeName}</b>\n<size=70%>{address}</size>";
    }

    public string GetInteractionPrompt(GameObject interactor)
    {
        interactor.TryGetComponent(out PlayerCarry carry);
        interactor.TryGetComponent(out PlayerJobs jobs);
        DeliveryJob held = carry != null && carry.HeldPackage != null ? carry.HeldPackage.Job : null;

        if (held != null && held.Destination == this && held.Status == DeliveryJobStatus.Accepted)
        {
            return $"Entregar pedido · {DisplayName}";
        }
        if (jobs != null && jobs.HasJobFor(this))
        {
            return $"{DisplayName} · trae el paquete en la mano";
        }
        return DisplayName;
    }

    public void Interact(GameObject interactor)
    {
        if (interactor.TryGetComponent(out PlayerJobs jobs))
        {
            jobs.TryDeliver(this);
        }
    }
}
