using UnityEngine;

// Cambia la luz de la escena según la hora del TimeManager: posición del sol, color,
// intensidad, luz ambiente y brillo del cielo. De noche la misma luz hace de luna.
// Todo se ajusta con curvas y degradados en el Inspector, sin tocar código.
public class DayNightLighting : MonoBehaviour
{
    [Tooltip("La luz direccional que hace de sol (y de luna por la noche).")]
    [SerializeField] private Light sun;

    [Header("Horario del sol")]
    [SerializeField, Range(0f, 24f)] private float sunriseHour = 6.5f;
    [SerializeField, Range(0f, 24f)] private float sunsetHour = 20.5f;

    [Tooltip("Altura máxima del sol a mediodía, en grados.")]
    [SerializeField, Range(10f, 90f)] private float maxSunElevation = 60f;

    [Tooltip("Altura máxima de la luna a medianoche, en grados.")]
    [SerializeField, Range(10f, 90f)] private float maxMoonElevation = 40f;

    [Tooltip("Hacia dónde mira el sol a mediodía (giro horizontal, en grados).")]
    [SerializeField] private float noonDirection = -30f;

    [Header("Aspecto a lo largo del día (de 00:00 a 24:00)")]
    [Tooltip("Color de la luz. El eje va de las 00:00 (izquierda) a las 24:00 (derecha).")]
    [SerializeField] private Gradient lightColor;

    [Tooltip("Intensidad de la luz según la hora (eje X en horas, de 0 a 24).")]
    [SerializeField] private AnimationCurve lightIntensity;

    [Tooltip("Color de la luz ambiente (la que ilumina las sombras).")]
    [SerializeField] private Gradient ambientColor;

    [Tooltip("Brillo del cielo según la hora (eje X en horas, de 0 a 24).")]
    [SerializeField] private AnimationCurve skyExposure;

    private Material skyboxInstance;

    private void Start()
    {
        // La luz ambiente pasa a ser un color plano que controlamos nosotros
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;

        // Copia del material del cielo para cambiar su brillo sin modificar el original del proyecto
        if (RenderSettings.skybox != null)
        {
            skyboxInstance = new Material(RenderSettings.skybox);
            RenderSettings.skybox = skyboxInstance;
        }
    }

    private void OnDestroy()
    {
        if (skyboxInstance != null) Destroy(skyboxInstance);
    }

    private void Update()
    {
        TimeManager time = TimeManager.Instance;
        if (time == null || sun == null) return;

        float hour = time.TimeOfDay;
        float normalized = hour / 24f;

        sun.transform.rotation = CalculateLightRotation(hour);
        sun.color = lightColor.Evaluate(normalized);
        sun.intensity = lightIntensity.Evaluate(hour);
        RenderSettings.ambientLight = ambientColor.Evaluate(normalized);

        if (skyboxInstance != null && skyboxInstance.HasProperty("_Exposure"))
        {
            skyboxInstance.SetFloat("_Exposure", skyExposure.Evaluate(hour));
        }
    }

    private Quaternion CalculateLightRotation(float hour)
    {
        bool isDay = hour >= sunriseHour && hour < sunsetHour;
        float progress;
        float maxElevation;

        if (isDay)
        {
            // 0 al amanecer, 1 al anochecer
            progress = Mathf.InverseLerp(sunriseHour, sunsetHour, hour);
            maxElevation = maxSunElevation;
        }
        else
        {
            // La noche cruza la medianoche: contamos las horas desde el anochecer
            float nightLength = 24f - (sunsetHour - sunriseHour);
            float hoursSinceSunset = (hour - sunsetHour + 24f) % 24f;
            progress = hoursSinceSunset / nightLength;
            maxElevation = maxMoonElevation;
        }

        // Sube y baja en arco (seno), y cruza el cielo de este a oeste.
        // Mínimo de 5 grados para que nunca ilumine desde debajo del suelo.
        float elevation = Mathf.Max(Mathf.Sin(progress * Mathf.PI) * maxElevation, 5f);
        float direction = noonDirection + Mathf.Lerp(-90f, 90f, progress);
        return Quaternion.Euler(elevation, direction, 0f);
    }

    // Reset se llama al añadir el componente en el editor: rellena valores iniciales razonables
    private void Reset()
    {
        sun = RenderSettings.sun;

        Color moon = new Color(0.55f, 0.65f, 1f);
        Color dawn = new Color(1f, 0.6f, 0.35f);
        Color day = new Color(1f, 0.96f, 0.88f);
        Color dusk = new Color(1f, 0.5f, 0.3f);
        lightColor = MakeGradient(
            (moon, 0f), (moon, 0.25f), (dawn, 0.29f), (day, 0.375f),
            (day, 0.7f), (dusk, 0.84f), (moon, 0.9f), (moon, 1f));

        lightIntensity = new AnimationCurve(
            new Keyframe(0f, 0.25f), new Keyframe(5f, 0.25f), new Keyframe(6.5f, 0.05f),
            new Keyframe(8f, 1.2f), new Keyframe(12f, 2f), new Keyframe(17f, 1.6f),
            new Keyframe(20.5f, 0.05f), new Keyframe(22f, 0.25f), new Keyframe(24f, 0.25f));

        Color nightAmbient = new Color(0.08f, 0.1f, 0.18f);
        Color dawnAmbient = new Color(0.45f, 0.4f, 0.4f);
        Color dayAmbient = new Color(0.55f, 0.6f, 0.7f);
        Color duskAmbient = new Color(0.4f, 0.3f, 0.3f);
        ambientColor = MakeGradient(
            (nightAmbient, 0f), (nightAmbient, 0.25f), (dawnAmbient, 0.3f), (dayAmbient, 0.42f),
            (dayAmbient, 0.75f), (duskAmbient, 0.85f), (nightAmbient, 0.9f), (nightAmbient, 1f));

        skyExposure = new AnimationCurve(
            new Keyframe(0f, 0.12f), new Keyframe(5.5f, 0.12f), new Keyframe(8f, 1.3f),
            new Keyframe(19f, 1.3f), new Keyframe(21.5f, 0.12f), new Keyframe(24f, 0.12f));
    }

    private static Gradient MakeGradient(params (Color color, float time)[] keys)
    {
        var colorKeys = new GradientColorKey[keys.Length];
        for (int i = 0; i < keys.Length; i++)
        {
            colorKeys[i] = new GradientColorKey(keys[i].color, keys[i].time);
        }
        var gradient = new Gradient();
        gradient.SetKeys(colorKeys, new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) });
        return gradient;
    }
}
