using UnityEngine;

// La cama de la vivienda: con Interactuar se duerme y termina el día (si ya es hora).
public class Bed : MonoBehaviour, IInteractable
{
    [SerializeField] private SleepController sleepController;

    public string GetInteractionPrompt(GameObject interactor)
    {
        return sleepController.CanSleepNow
            ? $"Dormir hasta las {sleepController.WakeUpText}"
            : $"Cama (se puede dormir desde las {sleepController.EarliestSleepText})";
    }

    public void Interact(GameObject interactor)
    {
        sleepController.TrySleep();
    }
}
