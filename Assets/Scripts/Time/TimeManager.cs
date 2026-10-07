using System;
using UnityEngine;

// Reloj del juego: lleva el día y la hora y avisa con eventos cuando cambian.
// Hay uno solo en la escena y se accede desde cualquier script con TimeManager.Instance.
// DefaultExecutionOrder(-100) hace que se inicialice antes que los scripts que lo usan.
[DefaultExecutionOrder(-100)]
public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    [SerializeField] private TimeSettings settings;

    [Tooltip("Solo para pruebas: multiplica la velocidad del tiempo. 60 = un día en 24 segundos.")]
    [Range(0f, 120f)]
    [SerializeField] private float timeSpeed = 1f;

    [Tooltip("Para el reloj (por ejemplo, en menús o diálogos).")]
    [SerializeField] private bool isPaused;

    // Avisa al empezar cada hora (0-23). Lo usarán los turnos, los pedidos, los NPCs...
    public event Action<int> OnHourChanged;

    // Avisa al empezar un día nuevo (a medianoche)
    public event Action<int> OnDayChanged;

    public int Day { get; private set; }

    // Hora actual con decimales, de 0 a 24 (13.5 = 13:30)
    public float TimeOfDay { get; private set; }

    public int Hour => Mathf.FloorToInt(TimeOfDay);
    public int Minute => Mathf.FloorToInt((TimeOfDay - Hour) * 60f);

    // Horas totales desde el inicio del día 0. Sirve para comparar momentos de días distintos
    // (por ejemplo, una hora límite a las 00:30 del día siguiente).
    public float TotalHours => Day * 24f + TimeOfDay;

    // Segundos reales que dura una hora de juego (sin contar timeSpeed)
    public float RealSecondsPerGameHour => settings.realSecondsPerGameHour;

    public bool IsPaused
    {
        get => isPaused;
        set => isPaused = value;
    }

    private void Awake()
    {
        // Si ya hay otro TimeManager, este sobra
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Hay más de un TimeManager en la escena; se elimina el sobrante.", this);
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (settings == null)
        {
            Debug.LogError($"{name}: falta asignar un TimeSettings en el TimeManager.", this);
            enabled = false;
            return;
        }

        Day = settings.startDay;
        TimeOfDay = settings.startHour;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    private void Update()
    {
        if (isPaused) return;

        // Cuántas horas de juego pasan en este frame
        AdvanceTime(Time.deltaTime / settings.realSecondsPerGameHour * timeSpeed);
    }

    private void AdvanceTime(float gameHours)
    {
        int previousHour = Hour;
        TimeOfDay += gameHours;

        // Al pasar de las 24:00 empieza un día nuevo
        while (TimeOfDay >= 24f)
        {
            TimeOfDay -= 24f;
            Day++;
            OnDayChanged?.Invoke(Day);
        }

        if (Hour != previousHour)
        {
            OnHourChanged?.Invoke(Hour);
        }
    }

    // Pone el día y la hora de una partida guardada, sin lanzar eventos (no "pasa" el tiempo)
    public void LoadState(int day, float timeOfDay)
    {
        Day = day;
        TimeOfDay = Mathf.Repeat(timeOfDay, 24f);
    }

    // Salta directamente a una hora del día actual (pruebas, y más adelante dormir).
    // Si la hora es menor que la actual, se entiende que es del día siguiente.
    public void SetTime(float hour)
    {
        hour = Mathf.Repeat(hour, 24f);
        float hoursToAdvance = hour >= TimeOfDay ? hour - TimeOfDay : hour + 24f - TimeOfDay;
        AdvanceTime(hoursToAdvance);
    }

    // Texto "HH:MM" de la hora actual
    public string GetTimeText() => FormatTime(TimeOfDay);

    // Convierte horas (13.5, o en horas totales) en texto "HH:MM" (13:30)
    public static string FormatTime(float hours)
    {
        int totalMinutes = Mathf.FloorToInt(Mathf.Repeat(hours, 24f) * 60f);
        return $"{totalMinutes / 60:00}:{totalMinutes % 60:00}";
    }
}
