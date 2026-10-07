using System;
using System.IO;
using UnityEngine;

// Lee y escribe la partida en un archivo JSON (GDD: Application.persistentDataPath).
// Es estático porque no necesita estar en la escena: el menú principal también lo usa.
public static class SaveSystem
{
    private const string FileName = "partida.json";

    // Carpeta de datos del juego en cada sistema (en Windows, dentro de AppData/LocalLow)
    public static string FilePath => Path.Combine(Application.persistentDataPath, FileName);

    public static bool HasSave => File.Exists(FilePath);

    public static void Save(SaveData data)
    {
        // "true" = JSON con saltos de línea, más fácil de leer si abres el archivo
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(FilePath, json);
    }

    public static bool TryLoad(out SaveData data)
    {
        data = null;
        if (!HasSave) return false;

        try
        {
            data = JsonUtility.FromJson<SaveData>(File.ReadAllText(FilePath));
            return data != null;
        }
        catch (Exception e)
        {
            // Un archivo roto no debe colgar el juego: avisamos y se empieza de cero
            Debug.LogWarning($"No se pudo leer la partida guardada ({FilePath}): {e.Message}");
            return false;
        }
    }

    public static void Delete()
    {
        if (HasSave) File.Delete(FilePath);
    }
}
