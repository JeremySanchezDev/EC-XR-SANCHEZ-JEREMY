using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

// Puerta que se abre y cierra con la interaccion (rayo en VR o clic en PC). Gira sobre su pivote.
[RequireComponent(typeof(XRSimpleInteractable))]
public class DoorController : MonoBehaviour
{
    [SerializeField] Transform pivot;
    [SerializeField] float openAngle = -100f;
    [SerializeField] float speed = 4f;

    XRSimpleInteractable interactable;
    bool isOpen;
    Quaternion closedRot;

    public bool IsOpen => isOpen;

    void Awake()
    {
        if (pivot == null) pivot = transform.parent;
        closedRot = pivot.localRotation;
        interactable = GetComponent<XRSimpleInteractable>();
        interactable.selectEntered.AddListener(_ => isOpen = !isOpen);
    }

    void OnDestroy()
    {
        if (interactable != null) interactable.selectEntered.RemoveAllListeners();
    }

    void Update()
    {
        var target = isOpen ? closedRot * Quaternion.Euler(0f, openAngle, 0f) : closedRot;
        pivot.localRotation = Quaternion.Slerp(pivot.localRotation, target, Time.deltaTime * speed);
    }
}
