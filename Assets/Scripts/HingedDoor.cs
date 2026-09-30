using UnityEngine;

// Puerta fisica: un Rigidbody con HingeJoint que se agarra (XR Grab Interactable) y se mueve como una puerta real.
// La velocidad con que la mueves define como se abre o se cierra; al soltarla conserva su impulso y choca con el tope.
[RequireComponent(typeof(Rigidbody), typeof(HingeJoint))]
public class HingedDoor : MonoBehaviour
{
    [SerializeField] float openThresholdDegrees = 12f;
    [Tooltip("Colliders de la pared y el marco por donde pasa la puerta.")]
    [SerializeField] Collider[] ignoredColliders;

    HingeJoint hinge;

    public bool IsOpen => hinge != null && hinge.angle > openThresholdDegrees;

    void Awake()
    {
        hinge = GetComponent<HingeJoint>();
        var own = GetComponent<Collider>();
        foreach (var c in ignoredColliders)
            if (c != null) Physics.IgnoreCollision(own, c);
    }
}
