# XR Interaction Challenge

| | |
|---|---|
| **Apellidos y nombres** | Sánchez Jeremy |
| **Código del estudiante** | _(completar)_ |
| **Curso** | Laboratorio de Realidad Extendida (XR) para Videojuegos |
| **Docente** | Victor Alejandro Arroyo Castro |

## Descripción
Sala de entrenamiento XR hecha con primitivas de Unity. Integra la configuración del entorno XR (URP + OpenXR) y la interacción con XR Interaction Toolkit: agarrar objetos con física, interactuar a distancia con un rayo y moverse por teletransporte.

Escena principal: `Assets/Scenes/EC_XR_SanchezJeremy.unity`.

## Funcionalidades implementadas
- **Escenario:** piso, luz direccional, 4 paredes como límites visuales, mesa, pilar y luz puntual (más de cinco objetos 3D).
- **Objetos manipulables:** cubo, esfera y cilindro con `Rigidbody` y `XR Grab Interactable`.
- **Interacción a distancia:** botón amarillo en la pared norte; con el rayo enciende/apaga la luz de la sala y cambia su color (`RayInteractionToggle.cs`).
- **Reto libre:** teletransporte sobre el piso (`Teleportation Area`) y contador de objetos agarrados en un texto 3D (`GrabCounter.cs`).

## Controles
**Con visor VR (OpenXR):** gatillo lateral (grip) para agarrar, apuntar con el rayo y gatillo para pulsar el botón, joystick hacia adelante para apuntar el teletransporte.

**En PC sin visor (ratón y teclado, XR Device Simulator):**
- Mover: `W A S D`; subir/bajar: `Q` / `E`.
- Mirar: mantener `clic derecho` y mover el ratón (o `Tab` para alternar el bloqueo del cursor).
- Mano izquierda: mantener `Shift izquierdo`; mano derecha: mantener `Espacio`.
- Con una mano activa: `clic izquierdo` = gatillo, `G` = agarrar (grip), ratón = mover la mano.
- El panel del simulador muestra el resto de atajos al ejecutar.

## Capturas
![Sala general](Screenshots/01_sala_general.png)
![Objetos en la mesa](Screenshots/02_objetos_mesa.png)
![Botón de rayo y contador](Screenshots/03_boton_rayo_y_contador.png)

## Video demostrativo
_(agregar enlace, máximo 1 minuto)_

## Tecnologías y paquetes
- Unity 6000.3.10f1
- Universal Render Pipeline 17.3.0
- XR Interaction Toolkit 3.6.1 (samples: Starter Assets, XR Device Simulator)
- OpenXR 1.18.0 (XR Plug-in Management)
- Input System 1.18.0
- TextMesh Pro
