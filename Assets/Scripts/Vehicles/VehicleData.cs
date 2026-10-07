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

    [Header("Terreno y estabilidad")]
    [Tooltip("Cuánta velocidad máxima pierde cuesta arriba. 0 = nada; 2 = en una cuesta de 15° va a la mitad.")]
    [Min(0f)] public float uphillSlowdown = 0.5f;

    [Tooltip("Se mantiene derecho solo, sin volcar de lado (bici, moto).")]
    public bool keepUpright = false;

    [Tooltip("Lo rápido que se endereza si keepUpright está activo.")]
    [Min(0f)] public float uprightStrength = 10f;

    [Header("Carga")]
    [Tooltip("Cuántos paquetes caben (GDD: bici 2, moto 4, coche 8...).")]
    [Min(0)] public int cargoSlots = 0;

    [Tooltip("Tamaño máximo de paquete que se puede cargar.")]
    public PackageSize maxPackageSize = PackageSize.Small;

    [Header("Pedidos")]
    [Tooltip("Distancia máxima (m desde el almacén) de los pedidos que te llegan si tienes este vehículo.")]
    [Min(0f)] public float deliveryRange = 100f;
}
