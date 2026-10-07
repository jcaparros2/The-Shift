using UnityEngine;
using UnityEngine.InputSystem;

// Controla al jugador a pie: caminar con WASD/stick, correr con Shift, mirar con el ratón y saltar con Espacio.
// El cuerpo gira en horizontal (yaw) y solo el "cameraTarget" gira en vertical (pitch),
// así más adelante una cámara de Cinemachine (primera o tercera persona) puede seguir
// a ese mismo punto sin rehacer el jugador.
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    [Tooltip("Velocidad al caminar, en metros por segundo.")]
    [SerializeField] private float walkSpeed = 4f;

    [Tooltip("Velocidad al correr (Shift), en metros por segundo.")]
    [SerializeField] private float sprintSpeed = 7f;

    [Tooltip("Opcional: sin stamina se puede correr sin límite.")]
    [SerializeField] private PlayerStamina stamina;

    [Tooltip("Opcional: con un paquete en la mano no se puede correr.")]
    [SerializeField] private PlayerCarry carry;

    [Tooltip("Gravedad aplicada al jugador (negativa = hacia abajo).")]
    [SerializeField] private float gravity = -20f;

    [Tooltip("Altura del salto, en metros.")]
    [SerializeField] private float jumpHeight = 1.1f;

    [Header("Cámara")]
    [Tooltip("Punto a la altura de los ojos que gira arriba/abajo. La cámara cuelga de aquí.")]
    [SerializeField] private Transform cameraTarget;

    [Tooltip("Grados que gira la vista por cada píxel que se mueve el ratón.")]
    [SerializeField] private float lookSensitivity = 0.1f;

    [Tooltip("Límites para mirar arriba y abajo, en grados.")]
    [SerializeField] private float minPitch = -80f;
    [SerializeField] private float maxPitch = 80f;

    private CharacterController controller;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction sprintAction;

    // Ángulo vertical actual de la vista
    private float pitch;

    // Velocidad vertical acumulada por la gravedad
    private float verticalVelocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        // Acciones del asset de acciones del proyecto (InputSystem_Actions, mapa "Player")
        moveAction = InputSystem.actions.FindAction("Player/Move", throwIfNotFound: true);
        lookAction = InputSystem.actions.FindAction("Player/Look", throwIfNotFound: true);
        jumpAction = InputSystem.actions.FindAction("Player/Jump", throwIfNotFound: true);
        sprintAction = InputSystem.actions.FindAction("Player/Sprint", throwIfNotFound: true);
    }

    private void OnEnable()
    {
        // Oculta el cursor y lo deja fijo en el centro de la pantalla
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Update()
    {
        Look();
        Move();
    }

    private void Look()
    {
        // El ratón da cuánto se ha movido desde el último frame, así que no se multiplica por deltaTime
        Vector2 look = lookAction.ReadValue<Vector2>() * lookSensitivity;

        // Horizontal: gira todo el cuerpo, para que "adelante" sea hacia donde miras
        transform.Rotate(Vector3.up * look.x);

        // Vertical: solo gira el punto de la cámara, con límite para no dar la vuelta
        pitch = Mathf.Clamp(pitch - look.y, minPitch, maxPitch);
        cameraTarget.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    // Vuelve a mirar al frente (por ejemplo, al subir a un vehículo)
    public void ResetLook()
    {
        pitch = 0f;
        cameraTarget.localRotation = Quaternion.identity;
    }

    private void Move()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();

        // Convierte la entrada (x = lados, y = adelante) en una dirección relativa al jugador
        Vector3 direction = transform.right * input.x + transform.forward * input.y;

        // En el suelo, una pequeña fuerza hacia abajo mantiene al jugador pegado al terreno
        if (controller.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        // Saltar solo desde el suelo. La velocidad inicial sale de la física: v = √(2·g·altura)
        if (controller.isGrounded && jumpAction.WasPressedThisFrame())
        {
            verticalVelocity = Mathf.Sqrt(2f * -gravity * jumpHeight);
        }

        verticalVelocity += gravity * Time.deltaTime;

        // Correr: Shift pulsado, yendo hacia delante, con las manos libres y con stamina
        bool handsFull = carry != null && carry.IsCarrying;
        if (handsFull && sprintAction.WasPressedThisFrame())
        {
            PlayerMessages.Show("Con un paquete en la mano no puedes correr.");
        }

        bool wantsSprint = sprintAction.IsPressed() && input.y > 0.1f && !handsFull;
        bool sprinting = wantsSprint && (stamina == null || stamina.CanSprint);
        if (sprinting && stamina != null) stamina.UseForSprint(Time.deltaTime);

        float speed = sprinting ? sprintSpeed : walkSpeed;
        Vector3 velocity = direction * speed + Vector3.up * verticalVelocity;
        controller.Move(velocity * Time.deltaTime);
    }
}
