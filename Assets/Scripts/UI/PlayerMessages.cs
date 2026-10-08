using System;
using UnityEngine;

// Canal de avisos para el jugador ("Ya llevas un paquete", "Cargado: ...").
// Cualquier sistema llama a PlayerMessages.Show(texto) y la UI que esté escuchando lo muestra.
// Es estático para que quien avisa no necesite una referencia a la UI: solo se comunican por el evento.
public static class PlayerMessages
{
    public static event Action<string> OnMessage;

    // Sin recarga de código al darle a Play, quedarían suscritos objetos de un Play anterior
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetOnPlay() => OnMessage = null;

    public static void Show(string message)
    {
        // También a la consola, útil para depurar
        Debug.Log(message);
        OnMessage?.Invoke(message);
    }
}
