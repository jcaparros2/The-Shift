using TMPro;
using UnityEngine;

// Muestra el día y la hora del TimeManager en un texto de la pantalla.
// Solo reescribe el texto cuando cambia el minuto, para no generar basura cada frame.
public class ClockDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text label;

    private int lastShownMinute = -1;

    private void Update()
    {
        TimeManager time = TimeManager.Instance;
        if (time == null || label == null) return;

        // Un número único por minuto del juego (día, hora y minuto juntos)
        int currentMinute = (time.Day * 24 + time.Hour) * 60 + time.Minute;
        if (currentMinute == lastShownMinute) return;

        lastShownMinute = currentMinute;
        label.text = $"Día {time.Day}  {time.GetTimeText()}";
    }
}
