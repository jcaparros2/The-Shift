using System.Collections;
using TMPro;
using UnityEngine;

// Pantalla negra que aparece y desaparece poco a poco, con un texto encima (el resumen del día).
// Las funciones que tardan (Fade) son corrutinas: quien las usa espera con "yield return".
public class ScreenFader : MonoBehaviour
{
    [SerializeField] private CanvasGroup group;
    [SerializeField] private TMP_Text label;

    private void Awake()
    {
        group.alpha = 0f;
        label.text = "";
    }

    public void SetText(string text) => label.text = text;

    // Cambia la opacidad hasta "target" (0 = invisible, 1 = negro) en "duration" segundos reales
    public IEnumerator Fade(float target, float duration)
    {
        float start = group.alpha;
        for (float t = 0f; t < duration; t += Time.unscaledDeltaTime)
        {
            group.alpha = Mathf.Lerp(start, target, t / duration);
            yield return null;   // espera al siguiente frame
        }
        group.alpha = target;
    }
}
