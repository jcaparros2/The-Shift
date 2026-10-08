using UnityEngine;
using UnityEngine.UI;

// Barra de stamina: se llena de izquierda a derecha según la stamina que queda.
// Solo se ve cuando no está llena (así no estorba al caminar) y se pone roja al agotarte.
public class StaminaBarUI : MonoBehaviour
{
    [SerializeField] private PlayerStamina stamina;

    [Tooltip("Controla la transparencia de toda la barra.")]
    [SerializeField] private CanvasGroup group;

    [Tooltip("La parte de color. Su pivote debe estar a la izquierda para encoger hacia allí.")]
    [SerializeField] private Image fill;

    [SerializeField] private Color normalColor = new Color(1f, 0.82f, 0.25f);
    [SerializeField] private Color exhaustedColor = new Color(1f, 0.35f, 0.3f);

    [Tooltip("Lo rápido que aparece y desaparece la barra.")]
    [SerializeField] private float fadeSpeed = 4f;

    private void Update()
    {
        float value = stamina.Normalized;

        // Escalar en X la parte de color es más sencillo que una imagen "Filled", que necesita un sprite
        fill.rectTransform.localScale = new Vector3(value, 1f, 1f);
        fill.color = stamina.IsExhausted ? exhaustedColor : normalColor;

        // Visible mientras no esté llena del todo
        float targetAlpha = value < 0.999f ? 1f : 0f;
        group.alpha = Mathf.MoveTowards(group.alpha, targetAlpha, fadeSpeed * Time.unscaledDeltaTime);
    }
}
