// En qué punto está un pedido
public enum DeliveryJobStatus
{
    Available,  // En la estantería, nadie lo ha cogido
    Accepted,   // El jugador lo ha cogido: ahora es suyo
    Expired     // Caducó sin que nadie lo cogiera
}
