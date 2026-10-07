# Doble Turno — Game Design Document

Última actualización: 6 de octubre de 2026

## Visión general

Doble Turno es un simulador de vida en una ciudad pequeña: de día repartes paquetes, de noche conduces un taxi, y la misma gente aparece en los dos turnos.

**Pitch:** llegas a una ciudad nueva con 150 € y una bicicleta. Trabaja, mejora tu vida y descubre las historias de tus vecinos.

### Pilares de diseño

- **Una sola ciudad, dos caras.** Cada NPC, local y calle existe de día y de noche. Lo que haces en un turno cambia el otro.
- **Rutina con sorpresas.** El ciclo diario es predecible; las historias surgen dentro de él, no como misiones aparte.
- **Progreso que se nota.** Cada mejora (vehículo, casa, móvil) cambia cómo juegas, no solo un número.
- **Pequeño y denso.** Un mapa que se cruza en pocos minutos, lleno de cosas, mejor que uno grande y vacío.

### Ficha

| Campo | Valor |
| --- | --- |
| Género | Simulación de vida en mundo abierto pequeño |
| Referencia | Schedule 1 (estructura de bucle y progresión) |
| Motor | Unity 6, C# |
| Plataforma | PC (Steam) |
| Cámara | Primera persona (por confirmar), mas adelante podrás canviar vista. |
| Estilo visual | Low-poly |
| Equipo | 1 persona |

## Bucle de juego principal

Un día de juego dura unos 24 minutos reales (1 minuto real = 1 hora de juego, a ajustar). Los dos turnos generan dinero y relaciones, y el tiempo libre es donde los gastas.

```text
Despertar (07:00) → Turno reparto (08:00–18:00) → Tiempo libre (18:00–21:00)
  → Turno taxi (21:00–03:00) → Dormir (03:00–07:00) → nuevo día

Turno reparto y turno taxi → generan dinero y relaciones
Dinero y relaciones → se gastan en el tiempo libre (mejoras, compras, charlas)
```

El jugador no está obligado a hacer los dos turnos. Saltarse uno es una decisión: menos dinero, pero más tiempo para explorar, descansar o seguir una historia.

### Energía y descanso

- Trabajar y conducir gastan energía; comer y dormir la recuperan.
- Dormir poco reduce la energía máxima del día siguiente.
- Con energía muy baja, la conducción se vuelve torpe y bajan las propinas.

## Mecánicas: repartidor y taxista

El reparto premia planificar ruta y carga; el taxi premia conducir bien y hablar con la gente. Así los dos turnos no se sienten iguales.

| Aspecto | Repartidor (QuickDrop) | Taxista (CityCab) |
| --- | --- | --- |
| Horario | 08:00–18:00 | 21:00–03:00 |
| Habilidad clave | Planificar ruta y carga | Conducción suave y conversación |
| Ingreso | Pago por entrega + propina por puntualidad | Tarifa por viaje + propina por valoración |
| Penalización | Retrasos, paquetes dañados | Golpes, frenazos, rutas largas |
| Contacto con NPCs | Breve, en la puerta | Largo, dentro del coche |

### Repartidor: un pedido paso a paso

1. Abres la app QuickDrop o vas al almacén.
2. Eliges pedidos de una lista: destino, tamaño, pago y hora límite.
3. Cargas solo lo que cabe en tu vehículo.
4. Llegas, llamas y entregas en mano o en el buzón.
5. Cobras el pago base más una propina según puntualidad y estado del paquete.

Variables de cada pedido: distancia, tamaño (algunos no caben en bici), fragilidad, urgencia y cliente.

Eventos posibles: cliente ausente, dirección equivocada, paquete sospechoso (gancho de historia) o entregas encadenadas en el mismo edificio.

### Taxista: un viaje paso a paso

1. Te conectas en la app CityCab o esperas en una parada.
2. Llega una solicitud con pasajero, origen, destino y pago estimado; aceptas o rechazas.
3. Recoges al pasajero.
4. Conduces: cada pasajero valora cosas distintas (rapidez, suavidad, respetar semáforos).
5. Cobras la tarifa, la propina y una valoración de 1 a 5 estrellas.

Tipos de pasajero: con prisa, tranquilo, borracho, turista, habitual (NPC con historia).

Durante el viaje aparecen opciones de diálogo. Las respuestas cambian la relación con ese NPC.

## Vehículos y conducción

Al principio no tienes coche, así que CityCab te alquila un taxi de la empresa por noche. Comprar tu propio coche elimina ese gasto y es el primer gran objetivo.

| Vehículo | Precio (propuesta) | Paquetes | Taxi | Notas |
| --- | --- | --- | --- | --- |
| A pie | 0 € | 1 pequeño | No | Solo el primer día |
| Bicicleta | Inicial | 2 pequeños | No | Sin gasolina, lenta en cuestas |
| Moto | 1.500 € | 4 pequeños | No | Rápida, poca carga |
| Taxi de empresa | 25 € por noche | — | Sí | Alquiler; si lo dañas, pagas |
| Coche propio | 6.000 € | 8 medianos | Sí | Sirve para los dos turnos |
| Furgoneta | 15.000 € | 25, incluidos grandes | No | Pedidos grandes y mejor pagados |

### Conducción

- Estilo arcade, no simulación: fácil de controlar y divertido desde el primer minuto.
- Daño simple: golpes bajan el estado del vehículo y la propina; el taller lo repara.
- Gasolina como gasto pequeño pero constante.
- Multas por saltarse semáforos o exceso de velocidad cerca de la policía (opcional, fase posterior).
- Mejoras en el taller: motor, neumáticos, maletero, pintura.

## Economía y progresión

El dinero solo importa si siempre hay en qué gastarlo. Al inicio, los gastos fijos deben comerse cerca de la mitad de lo que ganas, para que cada mejora se sienta merecida. Todas las cifras son una primera propuesta para ajustar al probar.

### Ingresos

| Fuente | Rango (propuesta) | Qué lo sube |
| --- | --- | --- |
| Entrega de paquete | 8–20 € | Distancia, tamaño, urgencia, puntualidad |
| Viaje de taxi | 10–30 € | Distancia, hora (madrugada paga más), valoración |
| Propinas | 0–10 € | Trato, rapidez, relación con el NPC |
| Encargos de NPCs | Variable | Historias y favores |

### Gastos

| Gasto | Frecuencia | Importe (propuesta) |
| --- | --- | --- |
| Alquiler de vivienda | Semanal | 150 € (habitación) a 600 € (casa) |
| Alquiler del taxi | Por noche | 25 € hasta tener coche propio |
| Gasolina | Por uso | 5–15 € por depósito parcial |
| Comida | Diaria | 10–20 € (recupera energía) |
| Reparaciones | Al dañar | Según el golpe |
| Multas | Ocasional | 50–200 € |

### Niveles de progresión

| Nivel | Dinero acumulado | Vehículo | Vivienda | Desbloquea |
| --- | --- | --- | --- | --- |
| Recién llegado | 0–1.000 € | Bicicleta | Habitación | QuickDrop, taxi alquilado |
| Asentado | 1.000–5.000 € | Moto | Apartamento | Pedidos urgentes, más apps |
| Establecido | 5.000–20.000 € | Coche propio | Piso grande | Clientes VIP, taller completo |
| Empresario | 20.000 € o más | Furgoneta | Casa | Contratar repartidores, negocio propio |

## El móvil

El móvil es el menú principal del juego: todo lo que no es caminar o conducir pasa por él. Así se evitan menús abstractos y todo queda dentro del mundo.

| App | Función | Se desbloquea |
| --- | --- | --- |
| QuickDrop | Ver y aceptar pedidos de reparto | Desde el inicio |
| CityCab | Conectarse y recibir viajes | Desde el inicio |
| Mensajes | Conversaciones y encargos de NPCs | Desde el inicio |
| Mapa | Zonas, locales, destinos activos | Desde el inicio |
| Banco | Saldo, ingresos, gastos, pagos de alquiler | Desde el inicio |
| Contactos | NPCs conocidos y su nivel de relación | Al conocer al primer NPC |
| Tienda online | Comprar mejoras y muebles a domicilio | Nivel Asentado |
| Agenda | Horarios de NPCs que conoces bien | Nivel Establecido |
| Gestión | Contratar y supervisar repartidores | Nivel Empresario |

Un móvil mejor (comprado en la tienda) puede añadir ventajas: mapa con tráfico, más pedidos visibles a la vez o batería que dura todo el día.

## La ciudad y sus locales

Cinco zonas pequeñas, cada una con un carácter de día distinto del de noche. La ciudad se abre por zonas a medida que progresas.

| Zona | De día | De noche | Locales clave |
| --- | --- | --- | --- |
| Centro | Oficinas, comercios, tráfico | Bares, hotel, mucha demanda de taxi | Hotel Central, bar, banco, Burger House |
| Residencial | Entregas a domicilio, vecinos | Calles tranquilas, regresos a casa | Tu vivienda, tienda de Juan |
| Industrial | Almacén QuickDrop, pedidos grandes | Turnos de noche, zona poco segura | Almacén, taller |
| Puerto y playa | Turistas, terrazas | Fiesta, pasajeros borrachos | Chiringuito, paseo marítimo |
| Afueras | Casas aisladas, rutas largas | Carreteras oscuras, viajes caros | Gasolinera, casa abandonada |

### Locales en los que se puede entrar

Cada interior es casi un nivel pequeño, así que se construyen pocos y bien hechos. El resto son fachadas.

| Local | Qué haces dentro | Fase |
| --- | --- | --- |
| Tu vivienda | Dormir, guardar, cambiar ropa, decorar | MVP |
| Almacén QuickDrop | Recoger paquetes, hablar con el jefe | MVP |
| Gasolinera | Repostar, comprar comida, hablar con el dependiente | MVP |
| Taller | Reparar y mejorar vehículos | Fase 2 |
| Bar | Conocer NPCs, recibir encargos | Fase 3 |
| Burger House | Comer, recoger pedidos, trabajo extra | Fase 3 |

## NPCs, relaciones e historias

Las historias son cadenas cortas de eventos que se activan por condiciones (hora, zona, relación, pedidos anteriores), no misiones con un menú propio.

### Cómo funciona un NPC

- **Horario:** dónde está cada hora del día (tienda a las 10:00, bar a las 23:00, casa a las 02:00).
- **Relación:** de 0 a 100, en cuatro niveles: desconocido, conocido, amigo, confidente.
- **Memoria:** marcas de cosas que han pasado ("le entregaste tarde", "le llevaste a casa borracho").
- **Historia:** una cadena de 3 a 6 eventos que avanza cuando se cumplen sus condiciones.

### NPCs iniciales

| NPC | Rol | Día | Noche | Gancho |
| --- | --- | --- | --- | --- |
| Juan | Tendero | Recibe paquetes en su tienda | Pasajero de vuelta a casa | Su tienda va mal y esconde por qué |
| Laura | Camarera | Apenas se la ve | Pasajera habitual desde el bar | Quiere irse de la ciudad |
| Dueño del restaurante | Cliente | Te pide entregas | Te pide recoger a su hermano | El hermano se mete en líos |
| Jefe de QuickDrop | Jefe | Te asigna pedidos | — | Te ofrece pedidos "especiales" mejor pagados |

### Ejemplo de cadena: Juan

1. Entregas varios paquetes en su tienda (relación sube a "conocido").
2. Una noche lo recoges en taxi: "Oye, tú eres el de los paquetes".
3. Te pide llevar un paquete fuera de la app, en mano.
4. Descubres qué hay detrás de los problemas de la tienda.
5. Decides ayudarle o no; el resultado cambia la tienda y lo que te ofrece después.

## Arte, audio y estilo

Low-poly con colores planos: es barato de producir en solitario, se lee bien y envejece mejor que el realismo.

- **Assets:** usar packs low-poly de la Asset Store para edificios, coches y personajes, y modelar solo lo que da identidad (logos de QuickDrop y CityCab, tu vivienda).
- **Día:** luz cálida, calles llenas, tonos claros.
- **Noche:** farolas, neones del centro, calles vacías en las afueras. El cambio de luz es la forma más barata de que la ciudad parezca otra.
- **Audio:** ambiente distinto por zona y hora, radio en el vehículo como música principal, sonidos claros para cobros y notificaciones del móvil.
- **Interfaz:** mínima en pantalla (hora, dinero, energía); el resto, en el móvil.

## Arquitectura técnica en Unity

Sistemas pequeños e independientes que se comunican por eventos de C#, y datos del juego definidos en ScriptableObjects. Así puedes añadir el taxi o los NPCs sin romper el reparto.

| Sistema | Responsabilidad | Implementación |
| --- | --- | --- |
| TimeManager | Hora del juego, día/noche, eventos "empieza turno" | MonoBehaviour único, evento OnHourChanged |
| PlayerController | Caminar, mirar, interactuar | CharacterController + Input System + Cinemachine |
| Interaction | Puertas, objetos, NPCs, subir al vehículo | Interfaz IInteractable + raycast desde la cámara |
| VehicleController | Conducción arcade, daño, gasolina | Rigidbody con controlador propio o WheelCollider |
| JobSystem | Generar y validar pedidos y viajes | Clases DeliveryJob y TaxiJob, datos en ScriptableObjects |
| EconomyManager | Saldo, ingresos, gastos, alquileres | Servicio central con eventos OnMoneyChanged |
| NPCScheduler | Mover NPCs según su horario | NavMesh (paquete AI Navigation) + horario en ScriptableObject |
| DialogueSystem | Conversaciones y elecciones | Datos en ScriptableObject o paquete tipo Yarn Spinner |
| StoryManager | Activar eventos de historia por condiciones | Lista de eventos con condiciones y marcas guardadas |
| PhoneUI | Apps del móvil | UI Toolkit o Canvas, una pantalla por app |
| SaveSystem | Guardar y cargar partida | Serializar a JSON en Application.persistentDataPath |

### Datos como ScriptableObjects

- `PackageData`: tamaño, fragilidad, pago base.
- `PassengerProfile`: tipo, qué valora, frases.
- `NPCData`: nombre, horario, relación inicial, historia asociada.
- `VehicleData`: precio, velocidad, capacidad, consumo.
- `ShopItemData`: precio, efecto, nivel requerido.

## Alcance: MVP y hoja de ruta

El MVP es un solo día de reparto que se juega de principio a fin. No se empieza una fase sin cumplir la condición de la anterior.

| Fase | Qué se construye | Pasa a la siguiente cuando… |
| --- | --- | --- |
| 0 · Aprender y prototipar | Bases de Unity y C#: jugador que camina e interactúa. Prototipo de conducción arcade aislado, sin ciudad. | Conducir un coche por un plano vacío ya es divertido |
| 1 · MVP: Reparto | Un barrio, bici, pedidos QuickDrop, dinero y día/noche. Interiores: solo vivienda, almacén y gasolinera. | Un día entero de reparto se juega de principio a fin y engancha |
| 2 · Taxi y vehículos | Turno de noche con CityCab, tipos de pasajero, valoraciones. Moto, coche propio, taller y gastos fijos. | Los dos turnos se sienten distintos y la economía aguanta una semana |
| 3 · NPCs e historias | 3 o 4 NPCs con horario, relación y su propia historia. Mensajes y contactos en el móvil, bar y Burger House. | Una historia se descubre jugando, sin que nada la señale |
| 4 · Ciudad y pulido | Resto de zonas, más NPCs, progresión hasta Empresario. Guardado, menús, sonido y pruebas con otras personas. | — |

No hay fechas: dependen de cuántas horas a la semana le dediques. Mejor medir el avance por puertas cumplidas.

### Tareas de la fase 1 (MVP)

- [x] Jugador en primera persona que camina e interactúa
- [x] Reloj del juego con ciclo día/noche e iluminación
- [x] Bicicleta conducible
- [x] Almacén QuickDrop: recoger paquetes
- [ ] Generar pedidos con destino, pago y hora límite
- [ ] Entregar y cobrar
- [ ] Saldo de dinero visible y una tienda donde gastarlo
- [ ] Vivienda con cama para terminar el día
- [ ] Guardar y cargar partida

## Riesgos y preguntas abiertas

El mayor riesgo es el alcance: un proyecto en solitario muere más por querer hacerlo todo que por falta de ideas.

| Riesgo | Por qué | Cómo reducirlo |
| --- | --- | --- |
| Alcance excesivo | 10 sistemas a la vez agotan a una persona | Seguir las fases; no empezar una sin cerrar la anterior |
| Conducción poco divertida | Es lo más difícil técnicamente | Prototipo de conducción aislado en la fase 0 |
| Interiores | Cada uno es casi un nivel | Solo 3 en el MVP, el resto fachadas |
| Contenido de NPCs | Diálogos y horarios llevan mucho tiempo | 3 o 4 NPCs bien hechos antes de añadir más |
| Economía rota | Demasiado dinero o demasiado poco | Valores en ScriptableObjects para ajustarlos sin tocar código |

### Preguntas abiertas

- [ ] ¿Cámara en primera o tercera persona?
- [ ] ¿El taxi empieza siendo de la empresa (alquiler por noche), como propone este documento?
- [ ] ¿Habrá trabajos ilegales u opcionales tipo "pedidos especiales", o todo es legal?
- [ ] ¿El juego tiene un final o es infinito?
- [ ] ¿Qué pasa si no duermes: pierdes energía, eficiencia o te quedas dormido al volante?
- [ ] ¿Nombre definitivo: Doble Turno?
