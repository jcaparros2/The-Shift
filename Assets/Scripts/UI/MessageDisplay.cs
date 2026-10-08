using TMPro;
using UnityEngine;

// Muestra en pantalla los avisos de PlayerMessages durante unos segundos y luego los desvanece.
// Si llega un aviso nuevo, sustituye al anterior.
public class MessageDisplay : MonoBehaviour
{
    [Tooltip("Controla la transparencia de la caja del aviso.")]
    [SerializeField] private CanvasGroup group;
    [SerializeField] private TMP_Text label;

    [Tooltip("Segundos que se ve el aviso, contando el desvanecido.")]
    [SerializeField] private float duration = 2.5f;

    [Tooltip("Segundos finales en los que se va desvaneciendo.")]
    [SerializeField] private float fadeTime = 0.5f;

    private float timeLeft;

    private void OnEnable() => PlayerMessages.OnMessage += HandleMessage;
    private void OnDisable() => PlayerMessages.OnMessage -= HandleMessage;

    private void Start()
    {
        group.alpha = 0f;
    }

    private void HandleMessage(string message)
    {
        label.text = message;
        timeLeft = duration;
        group.alpha = 1f;
    }

    private void Update()
    {
        if (timeLeft <= 0f) return;

        // Tiempo real: la UI no debe pararse si algún día pausamos el juego con timeScale
        timeLeft -= Time.unscaledDeltaTime;
        group.alpha = Mathf.Clamp01(timeLeft / fadeTime);
    }
}
