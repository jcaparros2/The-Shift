using System;
using UnityEngine;

// El dinero del jugador (GDD: servicio central con eventos OnMoneyChanged).
// Hay uno solo en la escena y se accede con EconomyManager.Instance, igual que TimeManager.
[DefaultExecutionOrder(-100)]
public class EconomyManager : MonoBehaviour
{
    public static EconomyManager Instance { get; private set; }

    [Tooltip("Dinero al empezar la partida (GDD: llegas con 150 €).")]
    [SerializeField] private int startingMoney = 150;

    // Avisa con el saldo nuevo y cuánto ha cambiado (+ ingreso, - gasto)
    public event Action<int, int> OnMoneyChanged;

    public int Money { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Hay más de un EconomyManager en la escena; se elimina el sobrante.", this);
            Destroy(gameObject);
            return;
        }
        Instance = this;
        Money = startingMoney;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void AddMoney(int amount)
    {
        if (amount <= 0) return;
        Money += amount;
        OnMoneyChanged?.Invoke(Money, amount);
    }

    // Gasta si hay saldo suficiente; devuelve si se ha podido pagar (para la tienda, alquileres...)
    public bool TrySpend(int amount)
    {
        if (amount <= 0) return true;
        if (Money < amount) return false;

        Money -= amount;
        OnMoneyChanged?.Invoke(Money, -amount);
        return true;
    }
}
