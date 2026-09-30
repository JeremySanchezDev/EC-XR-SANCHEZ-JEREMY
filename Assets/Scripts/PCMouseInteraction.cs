using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

// Modo PC (sin visor): la mira del centro de la pantalla usa el XR Interaction Toolkit.
// Clic izquierdo: agarrar / soltar objetos, pulsar el boton o abrir la puerta. F: teletransporte. H: mostrar/ocultar ayuda.
public class PCMouseInteraction : MonoBehaviour
{
    [SerializeField] XRInteractionManager manager;
    [SerializeField] NearFarInteractor interactor;
    [SerializeField] TeleportationProvider teleportProvider;
    [SerializeField] float reach = 6f;

    Camera cam;
    IXRSelectInteractable held;
    string hint = "";
    bool showHelp = true;
    GUIStyle boxStyle, hintStyle, titleStyle;

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

        if (kb != null && kb.hKey.wasPressedThisFrame) showHelp = !showHelp;
        if (mouse.leftButton.wasPressedThisFrame) OnClick();
        if (kb != null && kb.fKey.wasPressedThisFrame) TryTeleport();
        UpdateHint();
    }

    void UpdateHint()
    {
        if (held != null)
        {
            hint = "[Clic izquierdo] Soltar / lanzar";
            return;
        }
        hint = "";
        if (!Physics.Raycast(cam.transform.position, cam.transform.forward, out var hit, reach)) return;
        var target = hit.collider.GetComponentInParent<XRBaseInteractable>();
        if (target is XRGrabInteractable) hint = "[Clic izquierdo] Agarrar: " + target.name;
        else if (target is XRSimpleInteractable)
        {
            var door = target.GetComponent<DoorController>();
            hint = door != null ? (door.IsOpen ? "[Clic izquierdo] Cerrar puerta" : "[Clic izquierdo] Abrir puerta")
                                : "[Clic izquierdo] Encender / apagar la luz";
        }
        else if (hit.collider.GetComponentInParent<TeleportationArea>() != null)
            hint = "[F] Teletransportarse aqui";
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
        if (boxStyle == null)
        {
            boxStyle = new GUIStyle(GUI.skin.box) { alignment = TextAnchor.UpperLeft, fontSize = 15, padding = new RectOffset(12, 12, 10, 10) };
            hintStyle = new GUIStyle(GUI.skin.box) { alignment = TextAnchor.MiddleCenter, fontSize = 18, fontStyle = FontStyle.Bold };
            titleStyle = new GUIStyle(GUI.skin.label) { fontSize = 16, fontStyle = FontStyle.Bold };
        }

        float cx = Screen.width * 0.5f, cy = Screen.height * 0.5f;
        GUI.Box(new Rect(cx - 3, cy - 3, 6, 6), GUIContent.none);

        if (!string.IsNullOrEmpty(hint))
            GUI.Box(new Rect(cx - 220, cy + 30, 440, 36), hint, hintStyle);

        if (showHelp)
        {
            GUI.Box(new Rect(12, 12, 400, 232),
                "CONTROLES (PC)\n" +
                "W A S D  -  Moverse\n" +
                "Q / E  -  Bajar / subir\n" +
                "Mouse  -  Mirar\n" +
                "Clic izquierdo  -  Agarrar, soltar, pulsar, abrir puerta\n" +
                "F  -  Teletransportarse al piso apuntado\n" +
                "H  -  Mostrar / ocultar esta ayuda\n" +
                "Esc  -  Liberar el cursor\n\n" +
                "Todos los objetos sueltos se pueden agarrar.", boxStyle);
        }
        else
        {
            GUI.Label(new Rect(14, 10, 300, 24), "H: ayuda", titleStyle);
        }
    }
}
