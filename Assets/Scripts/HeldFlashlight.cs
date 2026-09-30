using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

// Convierte un objeto agarrable en linterna: al sostenerlo, el gatillo (VR) o clic derecho (PC) enciende/apaga el foco.
[RequireComponent(typeof(XRGrabInteractable))]
public class HeldFlashlight : MonoBehaviour
{
    [SerializeField] Light beam;
    [SerializeField] Renderer bulb;
    [SerializeField] Color onEmission = new Color(2.5f, 2.2f, 1.2f);

    XRGrabInteractable grab;
    bool isOn;

    public bool IsOn => isOn;

    void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();
        grab.activated.AddListener(_ => Toggle());
        Apply();
    }

    void OnDestroy()
    {
        if (grab != null) grab.activated.RemoveAllListeners();
    }

    public void Toggle()
    {
        isOn = !isOn;
        Apply();
    }

    void Apply()
    {
        if (beam != null) beam.enabled = isOn;
        if (bulb != null)
        {
            var mat = bulb.material;
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", isOn ? onEmission : Color.black);
        }
    }
}
