using UnityEngine;

// Ajustes de cómo se generan los pedidos de reparto (turno, cantidad, pago y tiempo).
// Todos los valores se pueden tocar en el Inspector para equilibrar el juego sin cambiar código.
[CreateAssetMenu(fileName = "DeliveryJobSettings", menuName = "Doble Turno/Delivery Job Settings")]
public class DeliveryJobSettings : ScriptableObject
{
    [Header("Turno (GDD: 08:00–18:00)")]
    [Range(0, 23)] public int shiftStartHour = 8;
    [Range(0, 24)] public int shiftEndHour = 18;

    [Header("Cantidad")]
    [Tooltip("Pedidos nuevos que llegan al empezar el turno y cada hora (si hay huecos libres).")]
    [Min(1)] public int jobsPerHour = 3;

    [Tooltip("Tipos de paquete que pueden salir. Repite uno para que salga más a menudo.")]
    public PackageData[] packageTypes;

    [Header("Pago")]
    [Tooltip("Euros extra por cada metro de distancia desde el almacén.")]
    [Min(0f)] public float payPerMeter = 0.05f;

    [Tooltip("Probabilidad (0 a 1) de que un pedido sea urgente.")]
    [Range(0f, 1f)] public float urgentChance = 0.25f;

    [Tooltip("Los urgentes pagan esto multiplicado.")]
    [Min(1f)] public float urgentPayMultiplier = 1.5f;

    [Header("Hora límite")]
    [Tooltip("Velocidad media (m/s reales) que se supone para calcular el viaje. 5 m/s = 18 km/h, en bici.")]
    [Min(0.5f)] public float estimatedSpeed = 5f;

    [Tooltip("Margen en horas de juego, además del viaje, para un pedido normal.")]
    [Min(0f)] public float normalMarginHours = 2f;

    [Tooltip("Margen en horas de juego para un pedido urgente.")]
    [Min(0f)] public float urgentMarginHours = 0.75f;
}
