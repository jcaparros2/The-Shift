using UnityEngine;

// Ajustes de CityCab: cuánto cuesta alquilar el taxi y en qué horario (GDD: turno 21:00–03:00, 25 € por noche).
[CreateAssetMenu(fileName = "TaxiSettings", menuName = "Doble Turno/Taxi Settings")]
public class TaxiSettings : ScriptableObject
{
    [Tooltip("Precio del alquiler del taxi por noche, en euros.")]
    [Min(0)] public int rentPrice = 25;

    [Tooltip("Hora desde la que se puede alquilar.")]
    [Range(0, 23)] public int shiftStartHour = 21;

    [Tooltip("Hora (de madrugada) a la que termina el alquiler y el taxi vuelve a la parada.")]
    [Range(0, 23)] public int shiftEndHour = 3;
}
