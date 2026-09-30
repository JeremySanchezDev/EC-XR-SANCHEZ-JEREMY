using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

// Modo PC (sin visor): la mira del centro de la pantalla usa el XR Interaction Toolkit.
// Clic izquierdo: agarrar / soltar objetos o pulsar el boton. F: teletransporte al punto apuntado.
public class PCMouseInteraction : MonoBehaviour
{
    [SerializeField] XRInteractionManager manager;
    [SerializeField] NearFarInteractor interactor;
    [SerializeField] TeleportationProvider teleportProvider;
    [SerializeField] float reach = 6f;

    Camera cam;
    IXRSelectInteractable held;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (cam == null) { cam = Camera.main; return; }
        var mouse = Mouse.current;
        var kb = Keyboard.current;
        if (mouse == null) return;

        if (mouse.leftButton.wasPressedThisFrame) OnClick();
        if (kb != null && kb.fKey.wasPressedThisFrame) TryTeleport();
    }

    void OnClick()
    {
        if (held != null)
        {
            manager.SelectExit(interactor, held);
            held = null;
            return;
        }

        if (!Physics.Raycast(cam.transform.position, cam.transform.forward, out var hit, reach)) return;
        var target = hit.collider.GetComponentInParent<XRBaseInteractable>();
        if (target == null) return;

        if (target is XRGrabInteractable)
        {
            manager.SelectEnter((IXRSelectInteractor)interactor, (IXRSelectInteractable)target);
            if (interactor.hasSelection) held = target;
        }
        else
        {
            manager.SelectEnter((IXRSelectInteractor)interactor, (IXRSelectInteractable)target);
            manager.SelectExit((IXRSelectInteractor)interactor, (IXRSelectInteractable)target);
        }
    }

    void TryTeleport()
    {
        if (teleportProvider == null) return;
        if (!Physics.Raycast(cam.transform.position, cam.transform.forward, out var hit, 30f)) return;
        if (hit.collider.GetComponentInParent<TeleportationArea>() == null) return;
        teleportProvider.QueueTeleportRequest(new TeleportRequest { destinationPosition = hit.point });
    }

    void OnGUI()
    {
        float cx = Screen.width * 0.5f, cy = Screen.height * 0.5f;
        GUI.Box(new Rect(cx - 3, cy - 3, 6, 6), GUIContent.none);
        GUI.Label(new Rect(10, Screen.height - 28, 900, 24),
            "PC: clic izquierdo = agarrar / soltar / pulsar boton   |   F = teletransportarse al piso apuntado");
    }
}
