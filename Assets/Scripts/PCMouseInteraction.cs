using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Management;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation;

// Modo PC (sin visor): camara en primera persona con mouse + teclado que usa el XR Interaction Toolkit.
// Si hay un visor VR activo, este script no toca nada y el rig funciona con los mandos.
public class PCMouseInteraction : MonoBehaviour
{
    public enum PcMode { XRDeviceSimulator, FirstPerson }

    [Header("Modo PC")]
    [Tooltip("XRDeviceSimulator: mandos XR simulados con su panel de comandos. FirstPerson: camara simple con clic para agarrar.")]
    [SerializeField] PcMode mode = PcMode.XRDeviceSimulator;

    [Header("Referencias XR")]
    [SerializeField] XRInteractionManager manager;
    [SerializeField] NearFarInteractor interactor;
    [SerializeField] Transform originTransform;
    [SerializeField] Transform cameraOffset;
    [SerializeField] Transform leftController;
    [SerializeField] Transform rightController;
    [SerializeField] GameObject deviceSimulator;

    [Header("Ajustes PC")]
    [SerializeField] float lookSensitivity = 0.08f;
    [SerializeField] float walkSpeed = 2.5f;
    [SerializeField] float runMultiplier = 2f;
    [SerializeField] float crouchMultiplier = 0.5f;
    [SerializeField] float standEyeHeight = 1.65f;
    [SerializeField] float crouchEyeHeight = 0.95f;
    [SerializeField] float reach = 6f;

    Camera cam;
    CharacterController controller;
    IXRSelectInteractable held;
    bool pcMode;
    bool simMode;
    XRDeviceSimulator sim;
    float simBodyMultiplier, crouchOffset;
    float baseOffsetY;
    float yaw, pitch, eyeHeight, verticalSpeed;
    string hint = "";
    bool showHelp = true;
    bool wantLocked;
    GUIStyle boxStyle, hintStyle, titleStyle;

    void Start()
    {
        var xr = XRGeneralSettings.Instance;
        bool headsetActive = xr != null && xr.Manager != null && xr.Manager.activeLoader != null;
        if (headsetActive)
        {
            if (deviceSimulator != null) deviceSimulator.SetActive(false);
            enabled = false;
            return;
        }
        if (mode == PcMode.XRDeviceSimulator && deviceSimulator != null)
        {
            SetupSimulator();
            return;
        }
        if (deviceSimulator != null) deviceSimulator.SetActive(false);

        pcMode = true;
        cam = Camera.main;
        eyeHeight = standEyeHeight;

        foreach (var d in originTransform.GetComponentsInChildren<TrackedPoseDriver>(true)) d.enabled = false;
        // Sin mandos reales el modality manager apagaria los controladores; los dejamos activos para el modo PC.
        foreach (var m in originTransform.GetComponentsInChildren<UnityEngine.XR.Interaction.Toolkit.Inputs.XRInputModalityManager>(true)) m.enabled = false;
        if (leftController != null) leftController.gameObject.SetActive(true);
        if (rightController != null) rightController.gameObject.SetActive(true);
        cam.transform.localPosition = Vector3.zero;
        cam.transform.localRotation = Quaternion.identity;
        cameraOffset.localPosition = new Vector3(0f, eyeHeight, 0f);

        foreach (var c in new[] { leftController, rightController })
        {
            if (c == null) continue;
            c.SetParent(cam.transform, false);
            c.localRotation = Quaternion.identity;
        }
        if (leftController != null) leftController.localPosition = new Vector3(-0.25f, -0.25f, 0.5f);
        if (rightController != null) rightController.localPosition = new Vector3(0.25f, -0.25f, 0.6f);

        controller = originTransform.GetComponent<CharacterController>();
        if (controller != null)
        {
            controller.height = 1.7f;
            controller.radius = 0.25f;
            controller.center = new Vector3(0f, 0.85f, 0f);
        }
        yaw = originTransform.eulerAngles.y;
        LockCursor(true);
    }

    void SetupSimulator()
    {
        simMode = true;
        cam = Camera.main;
        deviceSimulator.SetActive(true);
        sim = deviceSimulator.GetComponent<XRDeviceSimulator>();
        baseOffsetY = cameraOffset != null ? cameraOffset.localPosition.y : 0f;
        if (sim != null)
        {
            simBodyMultiplier = sim.keyboardBodyTranslateMultiplier;
            // Shift ya no controla la mano izquierda: se usa para correr. La mano izquierda pasa a Alt izquierdo.
            var left = sim.manipulateLeftAction != null ? sim.manipulateLeftAction.action : null;
            if (left != null) left.ApplyBindingOverride(0, "<Keyboard>/leftAlt");
        }
        LockCursor(true);
    }

    void UpdateSimulator()
    {
        var kb = Keyboard.current;
        var mouse = Mouse.current;
        if (kb == null || mouse == null) return;

        if (kb.escapeKey.wasPressedThisFrame) LockCursor(false);
        if (wantLocked && Cursor.lockState != CursorLockMode.Locked) Cursor.lockState = CursorLockMode.Locked;
        if (wantLocked) Cursor.visible = false;
        if (!wantLocked && mouse.leftButton.wasPressedThisFrame) LockCursor(true);

        if (sim != null) sim.keyboardBodyTranslateMultiplier = simBodyMultiplier * (kb.leftShiftKey.isPressed ? runMultiplier : 1f);

        // Agacharse: se baja la altura de la camara mientras se mantiene C
        bool crouching = kb.cKey.isPressed;
        crouchOffset = Mathf.MoveTowards(crouchOffset, crouching ? standEyeHeight - crouchEyeHeight : 0f, 3f * Time.deltaTime);
        if (cameraOffset != null) cameraOffset.localPosition = new Vector3(0f, baseOffsetY - crouchOffset, 0f);
    }

    void LockCursor(bool locked)
    {
        wantLocked = locked;
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }

    void Update()
    {
        if (simMode) { UpdateSimulator(); return; }
        if (!pcMode) return;
        var mouse = Mouse.current;
        var kb = Keyboard.current;
        if (mouse == null || kb == null) return;

        if (kb.escapeKey.wasPressedThisFrame) LockCursor(false);
        if (kb.hKey.wasPressedThisFrame) showHelp = !showHelp;

        // Si Unity solto el cursor sin que el jugador lo pidiera (foco, alt-tab, recarga), se vuelve a bloquear.
        if (wantLocked && Cursor.lockState != CursorLockMode.Locked) Cursor.lockState = CursorLockMode.Locked;
        if (wantLocked) Cursor.visible = false;

        bool locked = wantLocked;
        if (!locked)
        {
            if (mouse.leftButton.wasPressedThisFrame) LockCursor(true);
            return;
        }

        Look(mouse);
        Move(kb);
        if (mouse.leftButton.wasPressedThisFrame) OnClick();
        if (mouse.rightButton.wasPressedThisFrame) ActivateHeld();
        if (mouse.leftButton.wasReleasedThisFrame && held != null && ((Component)held).GetComponent<HingedDoor>() != null) ReleaseHeld();
        if (kb.fKey.wasPressedThisFrame) TryTeleport();
        UpdateHint();
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (pcMode && hasFocus && wantLocked) LockCursor(true);
    }

    void OnDisable()
    {
        if (!pcMode) return;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Look(Mouse mouse)
    {
        var delta = mouse.delta.ReadValue() * lookSensitivity;
        yaw += delta.x;
        pitch = Mathf.Clamp(pitch - delta.y, -89f, 89f);
        originTransform.rotation = Quaternion.Euler(0f, yaw, 0f);
        cam.transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    void Move(Keyboard kb)
    {
        bool crouching = kb.cKey.isPressed || kb.leftCtrlKey.isPressed;
        bool running = kb.leftShiftKey.isPressed && !crouching;

        float targetEye = crouching ? crouchEyeHeight : standEyeHeight;
        eyeHeight = Mathf.MoveTowards(eyeHeight, targetEye, 4f * Time.deltaTime);
        cameraOffset.localPosition = new Vector3(0f, eyeHeight, 0f);

        float x = (kb.dKey.isPressed ? 1f : 0f) - (kb.aKey.isPressed ? 1f : 0f);
        float z = (kb.wKey.isPressed ? 1f : 0f) - (kb.sKey.isPressed ? 1f : 0f);
        float speed = walkSpeed * (running ? runMultiplier : 1f) * (crouching ? crouchMultiplier : 1f);
        var move = (originTransform.right * x + originTransform.forward * z);
        if (move.sqrMagnitude > 1f) move.Normalize();

        if (controller == null) { originTransform.position += move * speed * Time.deltaTime; return; }
        verticalSpeed = controller.isGrounded ? -1f : verticalSpeed + Physics.gravity.y * Time.deltaTime;
        controller.Move((move * speed + Vector3.up * verticalSpeed) * Time.deltaTime);
    }

    void UpdateHint()
    {
        if (held != null)
        {
            var fl = (held as Component) != null ? ((Component)held).GetComponent<HeldFlashlight>() : null;
            hint = fl != null ? "[Clic derecho] Linterna " + (fl.IsOn ? "OFF" : "ON") + "   |   [Clic izquierdo] Soltar / lanzar"
                              : "[Clic izquierdo] Soltar / lanzar";
            return;
        }
        hint = "";
        if (!Physics.Raycast(cam.transform.position, cam.transform.forward, out var hit, reach)) return;
        var target = hit.collider.GetComponentInParent<XRBaseInteractable>();
        if (target is XRGrabInteractable)
        {
            var door = target.GetComponent<HingedDoor>();
            hint = door != null ? "[Mantener clic izquierdo + mover la vista] Empujar / jalar la puerta"
                                : "[Clic izquierdo] Agarrar: " + target.name;
        }
        else if (target is XRSimpleInteractable) hint = "[Clic izquierdo] Encender / apagar la luz";
        else if (hit.collider.GetComponentInParent<UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation.TeleportationArea>() != null)
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

    void ReleaseHeld()
    {
        manager.SelectExit(interactor, held);
        held = null;
    }

    void ActivateHeld()
    {
        if (held is XRGrabInteractable grab)
            grab.activated.Invoke(new ActivateEventArgs { interactorObject = interactor, interactableObject = grab });
    }

    void TryTeleport()
    {
        if (!Physics.Raycast(cam.transform.position, cam.transform.forward, out var hit, 30f)) return;
        if (hit.collider.GetComponentInParent<UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation.TeleportationArea>() == null) return;
        if (controller != null) controller.enabled = false;
        originTransform.position = hit.point;
        if (controller != null) controller.enabled = true;
    }

    void OnGUI()
    {
        if (simMode)
        {
            if (boxStyle == null) boxStyle = new GUIStyle(GUI.skin.box) { alignment = TextAnchor.UpperLeft, fontSize = 15, padding = new RectOffset(12, 12, 10, 10) };
            GUI.Box(new Rect(Screen.width - 452, 12, 440, 158),
                "EXTRAS (PC)\n" +
                "Shift izquierdo (mantener)  -  Correr\n" +
                "C (mantener)  -  Agacharse\n" +
                "Alt izquierdo (mantener)  -  Mover mano IZQUIERDA\n" +
                "Espacio (mantener)  -  Mover mano DERECHA\n" +
                "Esc  -  Liberar el cursor (clic para volver)\n" +
                "El panel de la izquierda lista el resto de comandos.", boxStyle);
            return;
        }
        if (!pcMode) return;
        if (boxStyle == null)
        {
            boxStyle = new GUIStyle(GUI.skin.box) { alignment = TextAnchor.UpperLeft, fontSize = 15, padding = new RectOffset(12, 12, 10, 10) };
            hintStyle = new GUIStyle(GUI.skin.box) { alignment = TextAnchor.MiddleCenter, fontSize = 18, fontStyle = FontStyle.Bold };
            titleStyle = new GUIStyle(GUI.skin.label) { fontSize = 16, fontStyle = FontStyle.Bold };
        }

        float cx = Screen.width * 0.5f, cy = Screen.height * 0.5f;
        GUI.Box(new Rect(cx - 3, cy - 3, 6, 6), GUIContent.none);

        if (!wantLocked)
            GUI.Box(new Rect(cx - 220, cy - 20, 440, 40), "Haz clic para volver a controlar la camara", hintStyle);
        else if (!string.IsNullOrEmpty(hint))
            GUI.Box(new Rect(cx - 320, cy + 30, 640, 36), hint, hintStyle);

        if (showHelp)
        {
            GUI.Box(new Rect(12, 12, 470, 328),
                "CONTROLES (PC)\n" +
                "W A S D  -  Moverse\n" +
                "Shift izquierdo (mantener)  -  Correr\n" +
                "C o Ctrl izquierdo (mantener)  -  Agacharse\n" +
                "Mouse  -  Mirar\n" +
                "Clic izquierdo  -  Agarrar / soltar objetos, pulsar el boton\n" +
                "Mantener clic izq. en la puerta  -  Moverla con la vista\n" +
                "Clic derecho  -  Usar el objeto en mano (linterna)\n" +
                "F  -  Teletransportarse al piso apuntado\n" +
                "H  -  Mostrar / ocultar esta ayuda\n" +
                "Esc  -  Liberar el cursor del mouse\n\n" +
                "Todos los objetos sueltos se pueden agarrar.", boxStyle);
        }
        else
        {
            GUI.Label(new Rect(14, 10, 300, 24), "H: ayuda", titleStyle);
        }
    }
}
