# XR Interaction Challenge

| | |
|---|---|
| **Apellidos y nombres** | Sánchez Galán Jeremy Antonio |
| **Código del estudiante** | 2221899925 |
| **Curso** | Laboratorio de Realidad Extendida (XR) para Videojuegos |
| **Docente** | Victor Alejandro Arroyo Castro |

## Descripción
Sala de entrenamiento XR hecha con primitivas de Unity. Integra la configuración del entorno XR (URP + OpenXR) y la interacción con XR Interaction Toolkit: agarrar objetos con física, interactuar a distancia con un rayo y moverse por teletransporte.

Escena principal: `Assets/Scenes/EC_XR_SanchezJeremy.unity`.

## Funcionalidades implementadas
- **Escenario:** piso, luz direccional, 4 paredes como límites visuales, mesa, pilar y luz puntual (más de cinco objetos 3D).
- **Objetos manipulables:** todos los objetos sueltos (cubo, esfera, cilindro, cajas, barril, cajas y libros del estante; 17 en total) con `Rigidbody` y `XR Grab Interactable`.
- **Interacción a distancia:** botón amarillo en la pared norte; con el rayo enciende/apaga la luz de la sala y cambia su color (`RayInteractionToggle.cs`).
- **Detalles:** piso con baldosas, alfombra, mesa con patas, estante, cajas, barril, marco de puerta, lámpara, rótulos y post-procesado (tonemapping, bloom, viñeta).
- **Modo PC:** `PCMouseInteraction.cs` da una cámara en primera persona (correr, agacharse, cursor bloqueado) y usa el XR Interaction Toolkit desde la mira del ratón. Si hay un visor VR activo, se desactiva solo.
- **Puerta interactiva:** se abre y cierra con clic (PC) o con el rayo (VR) (`DoorController.cs`).
- **Reto libre:** teletransporte sobre el piso (`Teleportation Area`) y contador de objetos agarrados en un texto 3D (`GrabCounter.cs`).

## Controles
**Con visor VR (OpenXR):** gatillo lateral (grip) para agarrar, apuntar con el rayo y gatillo para pulsar el botón, joystick hacia adelante para apuntar el teletransporte.

**En PC sin visor (ratón y teclado):** el cursor queda bloqueado dentro de la ventana de juego.
- `W A S D`: moverse. `Shift izquierdo` (mantener): correr. `C` o `Ctrl izquierdo` (mantener): agacharse.
- `Mouse`: mirar. `Esc`: liberar el cursor (un clic vuelve a controlar la cámara).
- **Clic izquierdo** sobre el punto central (mira): agarrar el objeto apuntado; otro clic lo suelta o lo lanza. Sobre el botón amarillo enciende/apaga la luz; sobre la puerta la abre/cierra.
- `F`: teletransportarse al punto del piso apuntado.
- `H`: mostrar/ocultar el panel de ayuda. Al apuntar a un objeto aparece un texto con la acción disponible.

## Capturas
![Sala general](Screenshots/01_sala_general.png)
![Objetos en la mesa](Screenshots/02_objetos_mesa.png)
![Botón de rayo y contador](Screenshots/03_boton_rayo_y_contador.png)
![Estante y cajas](Screenshots/04_estante_y_cajas.png)
![Puerta abierta](Screenshots/05_puerta_abierta.png)

## Video demostrativo
_(agregar enlace, máximo 1 minuto)_

## Tecnologías y paquetes
- Unity 6000.3.10f1
- Universal Render Pipeline 17.3.0
- XR Interaction Toolkit 3.6.1 (sample: Starter Assets)
- OpenXR 1.18.0 (XR Plug-in Management)
- Input System 1.18.0
- TextMesh Pro
