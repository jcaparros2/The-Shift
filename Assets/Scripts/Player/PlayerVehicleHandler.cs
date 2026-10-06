using System;
using UnityEngine;
using UnityEngine.InputSystem;

// Gestiona si el jugador va a pie o en un vehículo.
// Al subir: apaga caminar e interactuar, sienta al jugador en el vehículo (la cámara va con él)
// y cambia los controles del mapa "Player" al mapa "Vehicle". Al bajar, lo contrario.
[RequireComponent(typeof(CharacterController))]
public class PlayerVehicleHandler : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerInteractor playerInteractor;

    // Avisan al subir y bajar (para la UI, el sonido, la cámara en tercera persona...)
    public event Action<VehicleEntry> OnVehicleEntered;
    public event Action<VehicleEntry> OnVehicleExited;

    public VehicleEntry CurrentVehicle { get; private set; }
    public bool IsInVehicle => CurrentVehicle != null;

    private CharacterController characterController;
    private InputActionMap playerMap;
    private InputActionMap vehicleMap;
    private InputAction exitAction;

    // Frame en que subimos: la misma pulsación de E no debe bajarnos en ese mismo frame
    private int enteredFrame = -1;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerMap = InputSystem.actions.FindActionMap("Player", throwIfNotFound: true);
        vehicleMap = InputSystem.actions.FindActionMap("Vehicle", throwIfNotFound: true);
        exitAction = vehicleMap.FindAction("Exit", throwIfNotFound: true);
    }

    private void Start()
    {
        // Empezamos a pie: los controles del vehículo, apagados
        vehicleMap.Disable();
        playerMap.Enable();
    }

    private void Update()
    {
        if (IsInVehicle && Time.frameCount != enteredFrame && exitAction.WasPressedThisFrame())
        {
            ExitVehicle();
        }
    }

    public void EnterVehicle(VehicleEntry entry)
    {
        if (IsInVehicle) return;

        CurrentVehicle = entry;
        enteredFrame = Time.frameCount;

        SetOnFootEnabled(false);

        // Hijo del asiento: el jugador (y su cámara) se mueve y gira con el vehículo
        transform.SetParent(entry.Seat, false);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        playerController.ResetLook();

        playerMap.Disable();
        vehicleMap.Enable();
        entry.Vehicle.IsDriven = true;

        OnVehicleEntered?.Invoke(entry);
    }

    public void ExitVehicle()
    {
        if (!IsInVehicle) return;

        VehicleEntry entry = CurrentVehicle;
        CurrentVehicle = null;
        entry.Vehicle.IsDriven = false;

        // De pie junto al vehículo, mirando hacia donde miraba el vehículo
        transform.SetParent(null);
        transform.SetPositionAndRotation(entry.GetExitPosition(),
                                         Quaternion.Euler(0f, entry.transform.eulerAngles.y, 0f));

        vehicleMap.Disable();
        playerMap.Enable();

        // Se activa después de colocarlo: con el CharacterController activo, mover el transform no siempre funciona
        SetOnFootEnabled(true);

        OnVehicleExited?.Invoke(entry);
    }

    private void SetOnFootEnabled(bool enabled)
    {
        characterController.enabled = enabled;
        playerController.enabled = enabled;
        playerInteractor.enabled = enabled;

        // PlayerController suelta el ratón al desactivarse; en el vehículo lo queremos igualmente bloqueado
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
