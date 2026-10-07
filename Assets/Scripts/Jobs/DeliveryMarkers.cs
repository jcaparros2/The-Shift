using System.Collections.Generic;
using UnityEngine;

// Pone un haz de luz vertical sobre el destino de cada pedido aceptado, para verlo desde lejos
// (y en el minimapa). El del paquete que llevas en la mano se pinta de otro color.
// Se actualiza solo cuando cambian los pedidos o lo que llevas en la mano (eventos), no cada frame.
public class DeliveryMarkers : MonoBehaviour
{
    [SerializeField] private PlayerJobs playerJobs;
    [SerializeField] private PlayerCarry carry;

    [Tooltip("Prefab del marcador (sin colliders, para que no tape el rayo de interactuar).")]
    [SerializeField] private GameObject markerPrefab;

    [SerializeField] private Color jobColor = new Color(1f, 0.55f, 0.1f);
    [SerializeField] private Color heldColor = new Color(0.35f, 1f, 0.4f);

    // Un marcador por destino (varios pedidos al mismo sitio comparten marcador)
    private readonly Dictionary<DeliveryPoint, GameObject> markers = new Dictionary<DeliveryPoint, GameObject>();

    // Para cambiar el color de un objeto sin crear materiales nuevos
    private MaterialPropertyBlock colorBlock;
    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");

    private void Awake() => colorBlock = new MaterialPropertyBlock();

    private void OnEnable()
    {
        playerJobs.OnJobsChanged += Refresh;
        carry.OnHeldPackageChanged += HandleHeldPackageChanged;
    }

    private void OnDisable()
    {
        playerJobs.OnJobsChanged -= Refresh;
        carry.OnHeldPackageChanged -= HandleHeldPackageChanged;
    }

    private void Start() => Refresh();

    private void HandleHeldPackageChanged(Package package) => Refresh();

    private void Refresh()
    {
        // Destinos que necesitan marcador
        var needed = new HashSet<DeliveryPoint>();
        foreach (DeliveryJob job in playerJobs.ActiveJobs) needed.Add(job.Destination);

        // Quitar los que ya no hacen falta
        var toRemove = new List<DeliveryPoint>();
        foreach (var pair in markers)
        {
            if (!needed.Contains(pair.Key)) toRemove.Add(pair.Key);
        }
        foreach (DeliveryPoint point in toRemove)
        {
            Destroy(markers[point]);
            markers.Remove(point);
        }

        // Crear los que faltan
        foreach (DeliveryPoint point in needed)
        {
            if (!markers.ContainsKey(point))
            {
                markers[point] = Instantiate(markerPrefab, point.transform.position, Quaternion.identity, transform);
            }
        }

        // Colorear: verde el destino del paquete que llevas en la mano, naranja el resto
        DeliveryPoint heldDestination = null;
        if (carry.HeldPackage != null && carry.HeldPackage.Job != null) heldDestination = carry.HeldPackage.Job.Destination;

        foreach (var pair in markers)
        {
            colorBlock.SetColor(BaseColorId, pair.Key == heldDestination ? heldColor : jobColor);
            foreach (Renderer r in pair.Value.GetComponentsInChildren<Renderer>()) r.SetPropertyBlock(colorBlock);
        }
    }
}
