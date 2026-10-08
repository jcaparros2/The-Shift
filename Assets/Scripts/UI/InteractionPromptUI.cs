using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// Muestra en pantalla qué pasará al pulsar Interactuar ("[E] Coger: Paquete pequeño")
// y un punto de mira que se resalta cuando apuntas a algo usable.
// El texto se consulta cada frame porque puede cambiar sin cambiar de objetivo
// (por ejemplo, la carga de la bici pasa de "Cargar" a "Carga llena").
public class InteractionPromptUI : MonoBehaviour
{
    [SerializeField] private PlayerInteractor interactor;

    [Tooltip("Caja con el texto; se oculta cuando no miras a nada.")]
    [SerializeField] private GameObject promptPanel;
    [SerializeField] private TMP_Text promptLabel;

    [Header("Punto de mira")]
    [SerializeField] private Image crosshair;
    [SerializeField] private Color crosshairIdleColor = new Color(1f, 1f, 1f, 0.5f);
    [SerializeField] private Color crosshairActiveColor = new Color(1f, 0.6f, 0.15f, 1f);
    [SerializeField] private float crosshairActiveScale = 1.8f;

    // Nombre de la tecla de Interactuar ("E"), leído de los controles para no escribirlo a mano
    private string interactKey;
    private string shownText;

    private void Start()
    {
        InputAction interact = InputSystem.actions.FindAction("Player/Interact", throwIfNotFound: true);
        // Sin las "interactions" del binding: la acción trae un "Hold" que no usamos y saldría "Hold E"
        interactKey = interact.GetBindingDisplayString(InputBinding.MaskByGroup("Keyboard&Mouse"),
                                                       InputBinding.DisplayStringOptions.DontIncludeInteractions);

        // Empieza oculto (en el editor la caja tiene un texto de ejemplo)
        promptPanel.SetActive(false);
        shownText = null;
    }

    private void Update()
    {
        // Sin interactor activo (vamos en un vehículo) no hay punto de mira ni texto
        bool onFoot = interactor.isActiveAndEnabled;
        crosshair.enabled = onFoot;

        IInteractable target = onFoot ? interactor.CurrentTarget : null;

        // Un objeto destruido sigue sin ser "null" para C# a través de la interfaz; Unity sí lo sabe
        if (target is Object unityObject && unityObject == null) target = null;

        ShowPrompt(target == null ? null : $"[{interactKey}] {target.GetInteractionPrompt(interactor.gameObject)}");

        bool active = target != null;
        crosshair.color = active ? crosshairActiveColor : crosshairIdleColor;
        crosshair.rectTransform.localScale = Vector3.one * (active ? crosshairActiveScale : 1f);
    }

    private void ShowPrompt(string text)
    {
        // Solo toca la UI cuando el texto cambia
        if (text == shownText) return;
        shownText = text;

        promptPanel.SetActive(text != null);
        if (text != null) promptLabel.text = text;
    }
}
