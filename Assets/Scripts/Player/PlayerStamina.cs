using UnityEngine;

// Stamina para correr: se gasta mientras corres y se recupera sola al parar.
// Si la vacías del todo quedas "agotado" y no puedes volver a correr hasta recuperar una parte,
// así no se puede correr a tirones con la stamina a cero.
// No es la "energía" del GDD (la del día, que se recupera comiendo y durmiendo): esta dura segundos.
public class PlayerStamina : MonoBehaviour
{
    [SerializeField] private float maxStamina = 100f;

    [Tooltip("Stamina que se gasta por segundo corriendo.")]
    [SerializeField] private float drainPerSecond = 20f;

    [Tooltip("Stamina que se recupera por segundo al no correr.")]
    [SerializeField] private float regenPerSecond = 25f;

    [Tooltip("Segundos sin correr antes de empezar a recuperar.")]
    [SerializeField] private float regenDelay = 1f;

    [Tooltip("Parte (0 a 1) que hay que recuperar tras agotarte para poder correr otra vez.")]
    [Range(0f, 1f)] [SerializeField] private float recoverToSprint = 0.3f;

    public float Current { get; private set; }
    public float Normalized => Current / maxStamina;
    public bool IsExhausted { get; private set; }
    public bool CanSprint => !IsExhausted && Current > 0f;

    // Momento (Time.time) en que se gastó stamina por última vez
    private float lastUseTime = float.NegativeInfinity;

    private void Awake()
    {
        Current = maxStamina;
    }

    // Lo llama PlayerController cada frame que el jugador corre
    public void UseForSprint(float deltaTime)
    {
        if (!CanSprint) return;

        Current = Mathf.Max(Current - drainPerSecond * deltaTime, 0f);
        lastUseTime = Time.time;
        if (Current <= 0f) IsExhausted = true;
    }

    private void Update()
    {
        // Se recupera solo si hace un rato que no corres (funciona aunque vayas en un vehículo)
        if (Time.time - lastUseTime < regenDelay) return;

        Current = Mathf.Min(Current + regenPerSecond * Time.deltaTime, maxStamina);
        if (IsExhausted && Normalized >= recoverToSprint) IsExhausted = false;
    }
}
