using UnityEngine;

// Cámara cenital del minimapa: sigue al jugador desde arriba, con el norte siempre hacia arriba.
// Dibuja en una RenderTexture que se muestra en el HUD; la flecha del centro gira según hacia dónde miras.
// Sigue al Player también en la bici, porque al subir el Player pasa a ser hijo del vehículo.
[RequireComponent(typeof(Camera))]
public class MinimapCamera : MonoBehaviour
{
    [SerializeField] private Transform target;

    [Tooltip("Altura sobre el jugador. Tiene que quedar por encima del edificio más alto.")]
    [SerializeField] private float height = 120f;

    [Tooltip("Flecha del centro del minimapa (en el HUD).")]
    [SerializeField] private RectTransform playerArrow;

    // LateUpdate: después de que el jugador (y la bici) se hayan movido este frame
    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 p = target.position;
        transform.SetPositionAndRotation(new Vector3(p.x, p.y + height, p.z), Quaternion.Euler(90f, 0f, 0f));

        // En la UI, girar en Z negativo = girar a la derecha, como el jugador al aumentar su ángulo Y
        if (playerArrow != null) playerArrow.localEulerAngles = new Vector3(0f, 0f, -target.eulerAngles.y);
    }
}
