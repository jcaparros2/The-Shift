using TMPro;
using UnityEngine;

// Muestra el saldo ("172 €") y, cada vez que cambia, un "+11 €" verde o "-25 €" rojo que se desvanece.
// Escucha el evento OnMoneyChanged de EconomyManager: no pregunta el saldo cada frame.
public class MoneyDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text moneyLabel;

    [Tooltip("Texto pequeño con el último cambio (+11 € / -25 €).")]
    [SerializeField] private TMP_Text changeLabel;

    [SerializeField] private Color gainColor = new Color(0.45f, 1f, 0.45f);
    [SerializeField] private Color lossColor = new Color(1f, 0.4f, 0.35f);

    [Tooltip("Segundos que se ve el último cambio antes de desaparecer.")]
    [SerializeField] private float changeDuration = 2f;

    private float changeTimeLeft;

    private void Start()
    {
        // En Start y no en OnEnable: así EconomyManager ya ha hecho su Awake
        EconomyManager.Instance.OnMoneyChanged += HandleMoneyChanged;
        moneyLabel.text = $"{EconomyManager.Instance.Money} €";
        changeLabel.alpha = 0f;
    }

    private void OnDestroy()
    {
        if (EconomyManager.Instance != null) EconomyManager.Instance.OnMoneyChanged -= HandleMoneyChanged;
    }

    private void HandleMoneyChanged(int money, int change)
    {
        moneyLabel.text = $"{money} €";

        changeLabel.text = change > 0 ? $"+{change} €" : $"{change} €";
        changeLabel.color = change > 0 ? gainColor : lossColor;
        changeLabel.alpha = 1f;
        changeTimeLeft = changeDuration;
    }

    private void Update()
    {
        if (changeTimeLeft <= 0f) return;

        // Se desvanece durante el último medio segundo
        changeTimeLeft -= Time.unscaledDeltaTime;
        changeLabel.alpha = Mathf.Clamp01(changeTimeLeft / 0.5f);
    }
}
