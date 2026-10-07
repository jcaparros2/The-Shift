using System.Text;
using TMPro;
using UnityEngine;

// Panel con los pedidos aceptados: "Calle Mayor 3 · 14 € · 10:30".
// Se colorea según el tiempo que queda: normal, poco tiempo (naranja) o tarde (rojo).
// Se redibuja cuando cambia la lista (evento de PlayerJobs) y una vez por minuto de juego,
// porque los colores cambian con la hora aunque la lista no cambie.
public class JobListUI : MonoBehaviour
{
    [SerializeField] private PlayerJobs playerJobs;

    [Tooltip("Panel que se oculta cuando no hay pedidos.")]
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text label;

    [Tooltip("Horas de juego restantes por debajo de las cuales el pedido se marca en naranja.")]
    [SerializeField] private float warningHours = 0.5f;

    private int lastDrawnMinute = -1;

    private void OnEnable() => playerJobs.OnJobsChanged += Redraw;
    private void OnDisable() => playerJobs.OnJobsChanged -= Redraw;

    private void Start() => Redraw();

    private void Update()
    {
        // Un número distinto por cada minuto de juego: si cambia, toca redibujar los colores
        int minute = Mathf.FloorToInt(TimeManager.Instance.TotalHours * 60f);
        if (minute != lastDrawnMinute) Redraw();
    }

    private void Redraw()
    {
        float now = TimeManager.Instance.TotalHours;
        lastDrawnMinute = Mathf.FloorToInt(now * 60f);

        var jobs = playerJobs.ActiveJobs;
        panel.SetActive(jobs.Count > 0);
        if (jobs.Count == 0) return;

        // StringBuilder: para montar texto en varias partes sin crear muchas cadenas intermedias
        var text = new StringBuilder("<b>Pedidos</b>");
        foreach (DeliveryJob job in jobs)
        {
            float hoursLeft = job.Deadline - now;
            string color = hoursLeft < 0f ? "#FF5A4A" : hoursLeft < warningHours ? "#FFB340" : "#FFFFFF";
            string late = hoursLeft < 0f ? " (tarde)" : "";
            text.Append($"\n<color={color}>{job.Destination.Address} · {job.Pay} € · {job.DeadlineText}{late}</color>");
        }
        label.text = text.ToString();
    }
}
