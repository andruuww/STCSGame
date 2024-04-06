using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class InteractableTest : MonoBehaviour, DoorInteractable {
    [SerializeField] [Range(0, 360)] private float interactAngle = 90;

    private Collider _collider;

    public void Awake() {
        _collider = GetComponent<Collider>();
    }

    public void Interact(PlayerStateManager playerStateManager) {
    }

    public bool GetConeInteractableSettings(out Vector3 objPos, out Vector3 axis, out float angle) {
        objPos = _collider.bounds.center;
        axis = transform.forward;
        angle = interactAngle;
        return true;
    }
}