using UnityEngine;

// Escribe en la consola a qué estás mirando, escuchando el evento de PlayerInteractor.
// Es temporal, hasta que haya UI con el texto de ayuda; muestra cómo otro sistema se suscribe a un evento.
[RequireComponent(typeof(PlayerInteractor))]
public class InteractionDebugLogger : MonoBehaviour
{
    private PlayerInteractor interactor;

    private void Awake()
    {
        interactor = GetComponent<PlayerInteractor>();
    }

    // Suscribirse al activarse y desuscribirse al desactivarse evita llamadas a objetos destruidos
    private void OnEnable() => interactor.OnTargetChanged += HandleTargetChanged;
    private void OnDisable() => interactor.OnTargetChanged -= HandleTargetChanged;

    private void HandleTargetChanged(IInteractable target)
    {
        Debug.Log(target != null ? $"Mirando: [E] {target.InteractionPrompt}" : "Mirando: nada");
    }
}
