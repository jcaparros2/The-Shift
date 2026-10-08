using UnityEngine;

// Efecto visual: inclina el modelo hacia dentro de la curva, como una bici o una moto.
// Solo gira el modelo (hijo), no el objeto con la física, así la inclinación no afecta a la conducción.
public class VehicleLean : MonoBehaviour
{
    [SerializeField] private VehicleController vehicle;

    [Tooltip("El modelo que se inclina. Su origen debe estar a ras de suelo para que gire sobre las ruedas.")]
    [SerializeField] private Transform model;

    [Tooltip("Inclinación máxima en las curvas, en grados.")]
    [SerializeField] private float maxLeanAngle = 20f;

    [Tooltip("Velocidad (km/h) a partir de la cual se inclina del todo. Parado no se inclina.")]
    [SerializeField] private float fullLeanSpeedKmh = 15f;

    [Tooltip("Lo rápido que llega a la inclinación. Más alto = más brusco.")]
    [SerializeField] private float leanSmoothing = 6f;

    private float currentLean;

    // LateUpdate va después de Update y de la física del frame: buen sitio para efectos visuales
    private void LateUpdate()
    {
        float speedFactor = Mathf.Clamp01(Mathf.Abs(vehicle.CurrentSpeedKmh) / fullLeanSpeedKmh);

        // Girar a la derecha (+1) inclina a la derecha, que es un giro negativo sobre el eje Z
        float targetLean = -vehicle.SteerInput * maxLeanAngle * speedFactor;

        // Suavizado que no depende de los FPS
        currentLean = Mathf.Lerp(currentLean, targetLean, 1f - Mathf.Exp(-leanSmoothing * Time.deltaTime));
        model.localRotation = Quaternion.Euler(0f, 0f, currentLean);
    }
}
