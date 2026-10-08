using UnityEngine;

// Cómo debe empezar la partida al cargar la escena del juego.
// El menú principal lo rellena antes de cambiar de escena, y SaveManager lo lee al empezar.
// Los campos "static" sobreviven al cambio de escena (no pertenecen a ningún objeto).
public static class GameSession
{
    // true = cargar la partida guardada ("Continuar"); false = partida nueva
    public static bool ContinueRequested;

    // true cuando la escena se ha abierto desde el menú (no directamente en el editor)
    public static bool StartedFromMenu;

    // El proyecto no recarga el código al darle a Play (Enter Play Mode Options), así que los
    // valores estáticos de un Play anterior seguirían ahí. Unity llama a esto al empezar cada Play.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetOnPlay()
    {
        ContinueRequested = false;
        StartedFromMenu = false;
    }
}
