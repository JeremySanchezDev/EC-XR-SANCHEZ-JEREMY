using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

// Reto libre: contador de agarres mostrado en un texto 3D dentro de la sala.
public class GrabCounter : MonoBehaviour
{
    [SerializeField] XRGrabInteractable[] grabbables;
    [SerializeField] TextMesh label;

    int count;

    void OnEnable()
    {
        foreach (var g in grabbables) if (g != null) g.selectEntered.AddListener(OnGrab);
        UpdateLabel();
    }

    void OnDisable()
    {
        foreach (var g in grabbables) if (g != null) g.selectEntered.RemoveListener(OnGrab);
    }

    void OnGrab(UnityEngine.XR.Interaction.Toolkit.SelectEnterEventArgs _)
    {
        count++;
        UpdateLabel();
    }

    void UpdateLabel()
    {
        if (label != null) label.text = "Objetos agarrados: " + count;
    }
}
