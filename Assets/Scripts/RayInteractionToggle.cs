using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

// Interaccion a distancia: al apuntar con el rayo y presionar, enciende/apaga una luz y cambia el color del objeto.
[RequireComponent(typeof(XRSimpleInteractable))]
public class RayInteractionToggle : MonoBehaviour
{
    [SerializeField] Light targetLight;
    [SerializeField] Renderer targetRenderer;
    [SerializeField] Color onColor = Color.yellow;
    [SerializeField] Color offColor = Color.gray;

    XRSimpleInteractable interactable;
    bool isOn = true;

    void Awake()
    {
        interactable = GetComponent<XRSimpleInteractable>();
        interactable.selectEntered.AddListener(_ => Toggle());
        Apply();
    }

    void OnDestroy()
    {
        if (interactable != null) interactable.selectEntered.RemoveAllListeners();
    }

    void Toggle()
    {
        isOn = !isOn;
        Apply();
    }

    void Apply()
    {
        if (targetLight != null) targetLight.enabled = isOn;
        if (targetRenderer != null) targetRenderer.material.color = isOn ? onColor : offColor;
    }
}
