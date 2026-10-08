using UnityEngine;

// La parada de CityCab: aquí se alquila el taxi para el turno de noche y se devuelve.
// El alquiler dura hasta el final del turno (TaxiSettings.shiftEndHour); después el taxi vuelve solo.
public class TaxiStand : MonoBehaviour, IInteractable
{
    [SerializeField] private TaxiSettings settings;
    [SerializeField] private VehicleRental taxi;

    private string StartText => TimeManager.FormatTime(settings.shiftStartHour);
    private string EndText => TimeManager.FormatTime(settings.shiftEndHour);

    // ¿Es horario de alquiler? El turno cruza la medianoche: de shiftStartHour a 24 y de 0 a shiftEndHour
    private bool InShift
    {
        get
        {
            int hour = TimeManager.Instance.Hour;
            return hour >= settings.shiftStartHour || hour < settings.shiftEndHour;
        }
    }

    public string GetInteractionPrompt(GameObject interactor)
    {
        if (taxi.IsRented) return "Devolver el taxi";
        if (InShift) return $"Alquilar taxi esta noche · {settings.rentPrice} €";
        return $"CityCab · alquiler de {StartText} a {EndText}";
    }

    public void Interact(GameObject interactor)
    {
        if (taxi.IsRented)
        {
            taxi.ReturnToParking();
            return;
        }
        if (!InShift)
        {
            PlayerMessages.Show($"CityCab solo alquila taxis de {StartText} a {EndText}.");
            return;
        }
        if (!EconomyManager.Instance.TrySpend(settings.rentPrice))
        {
            PlayerMessages.Show($"No te llega: el alquiler cuesta {settings.rentPrice} €.");
            return;
        }

        // Fin del alquiler: el próximo shiftEndHour (hoy de madrugada o mañana de madrugada)
        TimeManager time = TimeManager.Instance;
        float until = time.Day * 24f + settings.shiftEndHour;
        if (time.Hour >= settings.shiftStartHour) until += 24f;

        taxi.Rent(until);
        PlayerMessages.Show($"Taxi alquilado hasta las {EndText}. ¡Buen turno!");
    }
}
