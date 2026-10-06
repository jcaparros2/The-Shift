using UnityEngine;

// Objeto de prueba: cambia a un color aleatorio cada vez que interactúas con él.
// Sirve para comprobar que el sistema de interacción funciona; se puede borrar más adelante.
public class TestInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private string prompt = "Cambiar color";

    public string InteractionPrompt => prompt;

    public void Interact(GameObject interactor)
    {
        // .material crea una copia del material solo para este objeto, así no se pintan los demás
        GetComponent<Renderer>().material.color = Random.ColorHSV(0f, 1f, 0.6f, 1f, 0.8f, 1f);
        Debug.Log($"{interactor.name} ha interactuado con {name}");
    }
}
