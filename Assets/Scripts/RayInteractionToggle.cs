using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

// Interaccion a distancia: al apuntar (rayo en VR / clic en PC) enciende o apaga la lampara.
// Con la lampara apagada la sala queda realmente a oscuras: sin luces de apoyo, sin luz ambiental ni reflejos del cielo.
[RequireComponent(typeof(XRSimpleInteractable))]
public class RayInteractionToggle : MonoBehaviour
{
    [SerializeField] Light targetLight;
    [SerializeField] Renderer targetRenderer;
    [SerializeField] Renderer bulb;
    [SerializeField] Light[] extraLights;
    [SerializeField] Color onColor = Color.yellow;
    [SerializeField] Color offColor = Color.gray;
    [SerializeField] Color ambientOn = new Color(0.12f, 0.13f, 0.17f);
    [SerializeField] Color ambientOff = new Color(0.005f, 0.005f, 0.008f);
    [SerializeField] float reflectionOn = 0.15f;
    [SerializeField] Color bulbEmission = new Color(3f, 2.7f, 2f);

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
        foreach (var l in extraLights) if (l != null) l.enabled = isOn;
        if (targetRenderer != null) targetRenderer.material.color = isOn ? onColor : offColor;
        if (bulb != null)
        {
            var mat = bulb.material;
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", isOn ? bulbEmission : Color.black);
            mat.SetColor("_BaseColor", isOn ? Color.white : new Color(0.15f, 0.15f, 0.15f));
        }
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = isOn ? ambientOn : ambientOff;
        RenderSettings.reflectionIntensity = isOn ? reflectionOn : 0f;
    }
}
