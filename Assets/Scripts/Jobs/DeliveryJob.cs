// Un pedido de reparto: qué paquete, a dónde, cuánto paga y hasta cuándo.
// Es una clase normal de C# (no un MonoBehaviour): solo guarda datos.
// El paquete físico que lo representa en el mundo es un Package con este pedido asignado.
public class DeliveryJob
{
    public PackageData PackageData { get; }
    public DeliveryPoint Destination { get; }
    public int Pay { get; }
    public bool IsUrgent { get; }

    // Hora límite en horas totales (TimeManager.TotalHours), para que funcione aunque pase de medianoche
    public float Deadline { get; }

    public DeliveryJobStatus Status { get; private set; } = DeliveryJobStatus.Available;

    public string DeadlineText => TimeManager.FormatTime(Deadline);

    public DeliveryJob(PackageData packageData, DeliveryPoint destination, int pay, float deadline, bool isUrgent)
    {
        PackageData = packageData;
        Destination = destination;
        Pay = pay;
        Deadline = deadline;
        IsUrgent = isUrgent;
    }

    public void Accept()
    {
        if (Status == DeliveryJobStatus.Available) Status = DeliveryJobStatus.Accepted;
    }

    public void Complete()
    {
        if (Status == DeliveryJobStatus.Accepted) Status = DeliveryJobStatus.Delivered;
    }

    public void Expire()
    {
        if (Status == DeliveryJobStatus.Available) Status = DeliveryJobStatus.Expired;
    }

    // Texto corto para la etiqueta del paquete: "Calle Mayor 3 · 14 € · antes de 10:30"
    public string Summary
    {
        get
        {
            string text = $"{Destination.DisplayName} · {Pay} € · antes de {DeadlineText}";
            if (IsUrgent) text += " · URGENTE";
            return text;
        }
    }
}
