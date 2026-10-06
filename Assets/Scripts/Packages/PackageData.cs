using UnityEngine;

// Datos de un tipo de paquete (GDD: tamaño, fragilidad, pago base).
// Cada tipo es un archivo en Assets/Data/Packages que se ajusta desde el Inspector.
[CreateAssetMenu(fileName = "NewPackageData", menuName = "Doble Turno/Package Data")]
public class PackageData : ScriptableObject
{
    [Tooltip("Nombre que se muestra al jugador.")]
    public string displayName = "Paquete pequeño";

    [Tooltip("Pequeño cabe en la mano y en la bici; mediano y grande necesitan vehículos más grandes.")]
    public PackageSize size = PackageSize.Small;

    [Tooltip("Si es frágil. Se usará al entregar: los golpes bajarán el pago (GDD: paquetes dañados).")]
    public bool fragile;

    [Tooltip("Pago por entregarlo, en euros, sin contar la propina.")]
    [Min(0)] public int basePay = 8;
}
