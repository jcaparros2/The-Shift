# Doble Turno — instrucciones para Claude Code

## El proyecto

Juego en Unity 6 (C#): simulador de vida en una ciudad pequeña. De día el jugador es repartidor (QuickDrop) y de noche taxista (CityCab). La misma gente aparece en los dos turnos.

El diseño completo está en `Docs/GDD.md`. Léelo antes de proponer o crear sistemas nuevos, y sigue sus decisiones (arquitectura, nombres de sistemas, fases).

## Sobre mí

- Estoy aprendiendo Unity y C#. Este es mi primer proyecto grande.
- Háblame siempre en español.
- Cuando crees o cambies un script, explícame brevemente qué hace cada parte y por qué lo haces así.
- Si hay varias formas razonables de hacer algo, dímelo y recomiéndame una antes de empezar.

## Cómo trabajamos

- **Una tarea pequeña cada vez**, siguiendo las fases y tareas del GDD. No empieces la siguiente fase sin que yo lo diga.
- Al terminar una tarea, dime cómo probarla en el editor (qué pulsar, qué debería pasar) y para ahí.
- No hagas cambios grandes que no he pedido. Si ves algo que mejorar fuera de la tarea, coméntamelo en lugar de hacerlo.
- Después de cambiar código, revisa la consola de Unity y corrige los errores de compilación antes de darme la tarea por terminada.
- No instales paquetes nuevos sin preguntarme primero.

## Decisiones técnicas

- Unity 6, Input System (no el Input Manager antiguo), Cinemachine para cámaras.
- Cámara: primera persona por defecto, pero el código debe permitir añadir más adelante una vista en tercera persona sin rehacer el jugador.
- Estilo arcade en la conducción, no simulación.
- Sistemas pequeños e independientes que se comunican por eventos de C#.
- Datos del juego (paquetes, vehículos, NPCs, tienda) en ScriptableObjects, para ajustar valores sin tocar código.
- Estilo de código: nombres en inglés (clases, métodos, variables), comentarios en español. Un script por clase, organizados en `Assets/Scripts/<Sistema>/`.

## Estructura de carpetas

```
Assets/
  Scripts/      # Código, una subcarpeta por sistema (Player, Time, Jobs, Economy...)
  Data/         # ScriptableObjects con los datos del juego
  Prefabs/
  Scenes/
  Art/          # Modelos, materiales, texturas
  Audio/
Docs/
  GDD.md        # Documento de diseño
```

## Estado actual

- Fase actual: **1 · MVP: Reparto**.
- Fase 0 terminada: jugador en primera persona (caminar, mirar, interactuar con `IInteractable`) en `Game`, y prototipo de conducción arcade (`VehicleController` + `VehicleData`, cámara con Cinemachine) en `DrivingPrototype`.
- Hecho en fase 1: reloj del juego (`TimeManager` + `TimeSettings`, eventos `OnHourChanged`/`OnDayChanged`), ciclo día/noche (`DayNightLighting`) y hora en pantalla (`ClockDisplay`) en `Game`.
- Hecho en fase 1: bicicleta conducible (prefab `Bike`, mismo `VehicleController` que el coche). Se sube y se baja con E (`VehicleEntry` + `PlayerVehicleHandler`, mapa de controles "Vehicle"). Cámaras con Cinemachine: primera persona a pie y cámara propia de cada vehículo al conducir (`PlayerCameraSwitcher`). El jugador salta con Espacio.
- Hecho en fase 1: almacén QuickDrop provisional en `Game` con paquetes (`PackageData` + `Package`, prefabs por tamaño). El jugador lleva 1 paquete pequeño en la mano (`PlayerCarry`, G para soltar) y la bici carga 2 en su caja trasera (`VehicleCargo`, capacidad en `VehicleData`).
- Hecho en fase 1: textos de interacción, punto de mira y avisos en pantalla (`InteractionPromptUI`, `PlayerMessages` + `MessageDisplay`). Pedidos: `DeliveryJobGenerator` llena el almacén durante el turno con paquetes-pedido (`DeliveryJob`, `DeliveryJobSettings`) para 5 destinos (`DeliveryPoint`); `PlayerJobs` guarda los aceptados y `JobListUI` los muestra.
- Hecho en fase 1: entregar y cobrar. La puerta del destino (`DeliveryPoint`) es interactuable; `PlayerJobs.TryDeliver` paga con propina a tiempo o penalización con retraso, y `EconomyManager` guarda el dinero (empieza con 150 €).
- Hecho en fase 1: saldo en pantalla (`MoneyDisplay`) y tienda: la bici está en venta (`ShopItemData` + `VehicleOwnership`, 250 €). A pie solo salen pedidos cercanos (`onFootRange`); cada vehículo comprado amplía el alcance (`VehicleData.deliveryRange`). Correr con Shift gasta stamina (`PlayerStamina` + `StaminaBarUI`).
- Hecho en fase 1: no se corre con un paquete en la mano. Vivienda: el jugador empieza en su habitación; la cama (`Bed` + `SleepController`) deja dormir de 18:00 a 06:00, funde a negro, muestra el resumen del día (`DayStats`, `ScreenFader`), cancela los pedidos sin entregar y despierta a las 07:00.
- Hecho en fase 1: guardar y cargar. Se guarda solo al dormir (`SaveManager` + `SaveSystem`, JSON en persistentDataPath: día, hora, dinero y vehículos). Escenas: `MainMenu` (Continuar / Nueva partida / Salir, arranca primero) y `Game` (el juego; antes `SampleScene`). Esc abre el menú de pausa (`PauseMenu`).
- Todas las tareas de la fase 1 están hechas. Falta comprobar su condición del GDD ("un día entero de reparto se juega de principio a fin y engancha") antes de pasar a la fase 2.
