using System;
using System.Collections.Generic;

// Lo que se guarda de una partida. JsonUtility convierte esta clase a texto JSON y al revés,
// por eso es [Serializable] y usa campos públicos simples (números, textos y listas).
// Se guarda al dormir, así que no hace falta guardar pedidos ni posiciones: al cargar
// despiertas en tu habitación por la mañana.
[Serializable]
public class SaveData
{
    // Sube este número si cambias lo que se guarda, para poder adaptar partidas antiguas
    public int version = 1;

    public int day;
    public float timeOfDay;
    public int money;

    // Vehículos comprados, por su identificador (VehicleOwnership.SaveId)
    public List<string> ownedVehicles = new List<string>();
}
