using UnityEngine;

// Contrato para cualquier cosa con la que el jugador puede interactuar
// (puertas, paquetes, NPCs, vehículos...). Cada objeto decide qué hace al usarlo.
public interface IInteractable
{
    // Texto corto para mostrar en pantalla, por ejemplo "Abrir puerta"
    string InteractionPrompt { get; }

    // Se llama cuando el jugador pulsa el botón de interactuar mirando a este objeto
    void Interact(GameObject interactor);
}
