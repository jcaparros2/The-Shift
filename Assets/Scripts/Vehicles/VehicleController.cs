using UnityEngine;
using UnityEngine.InputSystem;

// Conducción arcade: en vez de simular ruedas, controlamos directamente la velocidad del Rigidbody.
// La velocidad se separa en dos partes: "hacia delante" (la que da el motor) y "de lado"
// (el derrape). El motor cambia la primera; el agarre va quitando la segunda poco a poco.
// La física de Unity sigue encargándose de la gravedad, los choques y los saltos.
[RequireComponent(typeof(Rigidbody))]
public class VehicleController : MonoBehaviour
{
    // Para convertir km/h (más fáciles de ajustar) a m/s (lo que usa Unity)
    private const float KmhToMs = 1f / 3.6f;

    [Tooltip("Valores de conducción de este vehículo.")]
    [SerializeField] private VehicleData data;

    [Tooltip("Distancia hacia abajo, desde el centro del coche, para saber si toca el suelo.")]
    [SerializeField] private float groundCheckDistance = 0.8f;

    [Tooltip("Centro de masas respecto al coche. Bajo = vuelca menos.")]
    [SerializeField] private Vector3 centerOfMass = new Vector3(0f, -0.4f, 0f);

    [Tooltip("Segundos boca abajo antes de volver a ponerlo derecho automáticamente.")]
    [SerializeField] private float flipResetTime = 2f;

    [Tooltip("Si alguien lo conduce. Sin conductor ignora los controles y queda frenado (aparcado).")]
    [SerializeField] private bool isDriven = true;

    // Velocidad actual en km/h (positiva hacia delante). Útil para un velocímetro más adelante.
    public float CurrentSpeedKmh { get; private set; }
    public bool IsGrounded { get; private set; }

    // Giro pedido por el conductor (-1 izquierda, 1 derecha). Lo usan los efectos visuales, como inclinar la bici.
    public float SteerInput { get; private set; }

    public bool IsDriven
    {
        get => isDriven;
        set => isDriven = value;
    }

    private Rigidbody rb;
    private InputAction moveAction;
    private InputAction handbrakeAction;
    private float flippedTimer;

    private void Awake()
    {
        // Sin datos no sabe cómo conducir: avisa una vez y se apaga, en vez de dar un error cada frame
        if (data == null)
        {
            Debug.LogError($"{name}: falta asignar un VehicleData en el VehicleController.", this);
            enabled = false;
            return;
        }

        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = centerOfMass;

        // Mapa "Vehicle": W/S acelerar/frenar, A/D girar, Espacio freno de mano.
        // PlayerVehicleHandler lo activa al subir y lo apaga al bajar.
        moveAction = InputSystem.actions.FindAction("Vehicle/Drive", throwIfNotFound: true);
        handbrakeAction = InputSystem.actions.FindAction("Vehicle/Handbrake", throwIfNotFound: true);
    }

    // La física se hace en FixedUpdate, que va a ritmo fijo y sincronizado con el motor de física
    private void FixedUpdate()
    {
        // Sin conductor: ni acelera ni gira, y el freno de mano queda puesto
        Vector2 input = isDriven ? moveAction.ReadValue<Vector2>() : Vector2.zero;
        bool handbrake = !isDriven || handbrakeAction.IsPressed();
        SteerInput = input.x;

        IsGrounded = Physics.Raycast(transform.position, -transform.up, groundCheckDistance,
                                     ~0, QueryTriggerInteraction.Ignore);

        // Descompone la velocidad actual en los ejes del coche
        Vector3 velocity = rb.linearVelocity;
        float forwardSpeed = Vector3.Dot(velocity, transform.forward);
        float sideSpeed = Vector3.Dot(velocity, transform.right);
        float upSpeed = Vector3.Dot(velocity, transform.up);

        // En el aire no hay control: solo actúan la gravedad y la inercia
        if (IsGrounded)
        {
            forwardSpeed = ApplyThrottle(forwardSpeed, input.y, handbrake);
            sideSpeed = ApplyGrip(sideSpeed, handbrake);
            ApplySteering(forwardSpeed, input.x);

            rb.linearVelocity = transform.forward * forwardSpeed
                              + transform.right * sideSpeed
                              + transform.up * upSpeed;
        }

        if (data.keepUpright)
        {
            KeepUpright();
        }

        CurrentSpeedKmh = forwardSpeed / KmhToMs;
        ResetIfFlipped();
    }

    private float ApplyThrottle(float forwardSpeed, float throttle, bool handbrake)
    {
        float dt = Time.fixedDeltaTime;

        if (handbrake)
        {
            // El freno de mano frena algo menos que el freno normal
            return Mathf.MoveTowards(forwardSpeed, 0f, data.brakeKmh * 0.5f * KmhToMs * dt);
        }

        if (throttle > 0.01f)
        {
            // Si íbamos marcha atrás, primero frena; si no, acelera hasta la máxima
            float rate = forwardSpeed < 0f ? data.brakeKmh : data.accelerationKmh;
            float maxSpeed = data.maxSpeedKmh * UphillFactor();
            return Mathf.MoveTowards(forwardSpeed, maxSpeed * throttle * KmhToMs, rate * KmhToMs * dt);
        }

        if (throttle < -0.01f)
        {
            // Si vamos hacia delante, S frena; una vez parados, va marcha atrás
            if (forwardSpeed > 0.1f)
            {
                return Mathf.MoveTowards(forwardSpeed, 0f, data.brakeKmh * KmhToMs * dt);
            }
            return Mathf.MoveTowards(forwardSpeed, data.maxReverseSpeedKmh * throttle * KmhToMs,
                                     data.accelerationKmh * KmhToMs * dt);
        }

        // Sin pisar nada, el coche va perdiendo velocidad poco a poco
        return Mathf.MoveTowards(forwardSpeed, 0f, data.coastDecelerationKmh * KmhToMs * dt);
    }

    private float ApplyGrip(float sideSpeed, bool handbrake)
    {
        // Quita una parte del deslizamiento lateral cada frame. Con poco agarre, el coche derrapa.
        float grip = handbrake ? data.handbrakeGrip : data.grip;
        return Mathf.Lerp(sideSpeed, 0f, grip * Time.fixedDeltaTime);
    }

    private void ApplySteering(float forwardSpeed, float steer)
    {
        // Parado no gira; el giro crece con la velocidad hasta fullTurnSpeedKmh
        float speedFactor = Mathf.Clamp01(Mathf.Abs(forwardSpeed) / (data.fullTurnSpeedKmh * KmhToMs));

        // Marcha atrás el volante se invierte, como en un coche real
        float direction = Mathf.Sign(forwardSpeed);
        float yawRate = steer * data.turnSpeed * speedFactor * direction * Mathf.Deg2Rad;

        // Sustituye solo el giro alrededor del eje "arriba" del coche (sirve también en rampas)
        Vector3 angular = rb.angularVelocity;
        angular -= transform.up * Vector3.Dot(angular, transform.up);
        rb.angularVelocity = angular + transform.up * yawRate;
    }

    // 1 en llano o cuesta abajo; menos de 1 cuesta arriba, según uphillSlowdown
    private float UphillFactor()
    {
        // Seno del ángulo de subida: 0 en llano, 0.26 en una cuesta de 15°
        float slope = Mathf.Max(0f, Vector3.Dot(transform.forward, Vector3.up));
        return Mathf.Clamp01(1f - slope * data.uphillSlowdown);
    }

    private void KeepUpright()
    {
        // Ángulo que le falta para estar derecho, girando solo sobre su eje "adelante" (alabeo).
        // Así puede seguir subiendo rampas (cabeceo) sin caerse de lado.
        Vector3 forward = transform.forward;
        Vector3 uprightUp = Vector3.ProjectOnPlane(Vector3.up, forward).normalized;
        float rollError = Vector3.SignedAngle(transform.up, uprightUp, forward);

        // Sustituye el giro de alabeo por uno que lo endereza
        Vector3 angular = rb.angularVelocity;
        angular -= forward * Vector3.Dot(angular, forward);
        rb.angularVelocity = angular + forward * (rollError * Mathf.Deg2Rad * data.uprightStrength);
    }

    private void ResetIfFlipped()
    {
        // Si el techo apunta hacia abajo durante un rato, lo levantamos y lo ponemos derecho
        bool flipped = Vector3.Dot(transform.up, Vector3.up) < 0.2f;
        flippedTimer = flipped ? flippedTimer + Time.fixedDeltaTime : 0f;

        if (flippedTimer > flipResetTime)
        {
            flippedTimer = 0f;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.position += Vector3.up * 1.5f;
            rb.rotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
        }
    }

    // Dibuja el rayo de "¿toco el suelo?" en la vista Scene
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, -transform.up * groundCheckDistance);
    }
}
