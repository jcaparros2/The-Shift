using UnityEngine;

// Ajustes del reloj del juego. ScriptableObject para cambiar el ritmo del día sin tocar código.
[CreateAssetMenu(fileName = "TimeSettings", menuName = "Doble Turno/Time Settings")]
public class TimeSettings : ScriptableObject
{
    [Tooltip("Segundos reales que dura una hora de juego. 60 = un día de 24 minutos (GDD).")]
    [Min(0.1f)] public float realSecondsPerGameHour = 60f;

    [Tooltip("Día con el que empieza la partida.")]
    [Min(1)] public int startDay = 1;

    [Tooltip("Hora con la que empieza la partida (7 = despertar, según el GDD).")]
    [Range(0f, 24f)] public float startHour = 7f;
}
