using UnityEngine;
using UnityEngine.InputSystem;

// Controla al jugador a pie: caminar con WASD/stick y mirar con el ratón.
// El cuerpo gira en horizontal (yaw) y solo el "cameraTarget" gira en vertical (pitch),
// así más adelante una cámara de Cinemachine (primera o tercera persona) puede seguir
// a ese mismo punto sin rehacer el jugador.
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    [Tooltip("Velocidad al caminar, en metros por segundo.")]
    [SerializeField] private float walkSpeed = 4f;

    [Tooltip("Gravedad aplicada al jugador (negativa = hacia abajo).")]
    [SerializeField] private float gravity = -20f;

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
        verticalVelocity += gravity * Time.deltaTime;

        Vector3 velocity = direction * walkSpeed + Vector3.up * verticalVelocity;
        controller.Move(velocity * Time.deltaTime);
    }
}
