using UnityEngine;

// Datos de un tipo de vehículo (coche, taxi, moto, bici...).
// Es un ScriptableObject: se crea como archivo en el proyecto y se ajusta desde el Inspector,
// así podemos afinar cómo se conduce cada vehículo sin tocar código.
[CreateAssetMenu(fileName = "NewVehicleData", menuName = "Doble Turno/Vehicle Data")]
public class VehicleData : ScriptableObject
{
    [Header("Velocidad")]
    [Tooltip("Velocidad máxima hacia delante, en km/h.")]
    public float maxSpeedKmh = 90f;

    [Tooltip("Velocidad máxima marcha atrás, en km/h.")]
    public float maxReverseSpeedKmh = 25f;

    [Tooltip("Cuánto gana de velocidad por segundo al acelerar, en km/h.")]
    public float accelerationKmh = 35f;

    [Tooltip("Cuánto pierde de velocidad por segundo al frenar, en km/h.")]
    public float brakeKmh = 80f;

    [Tooltip("Cuánto pierde de velocidad por segundo sin pisar nada, en km/h.")]
    public float coastDecelerationKmh = 12f;

    [Header("Giro")]
    [Tooltip("Grados por segundo que gira a buena velocidad.")]
    public float turnSpeed = 110f;

    [Tooltip("Velocidad (km/h) a partir de la cual el giro es completo. Por debajo gira menos, como un coche real parado.")]
    public float fullTurnSpeedKmh = 25f;

    [Header("Agarre")]
    [Tooltip("Lo rápido que se corrige el deslizamiento lateral. Alto = va sobre raíles, bajo = derrapa.")]
    public float grip = 8f;

    [Tooltip("Agarre con el freno de mano puesto. Bajo para poder derrapar.")]
    public float handbrakeGrip = 1.5f;
}
