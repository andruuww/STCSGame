using UnityEngine;

public class DoorHandleManager : MonoBehaviour, DoorInteractable {
    [SerializeField] [Range(0, 360)] private float interactAngle = 90;
    [SerializeField] private bool reverse;
    private Collider _collider;

    private DoorManager _doorManager;

    public void Awake() {
        _doorManager = GetComponentInParent<DoorManager>();
        _collider = GetComponent<Collider>();
    }

    public void Interact(PlayerStateManager playerStateManager) {
        float mouseY = playerStateManager.GetMouseY();
        if (reverse) mouseY *= -1;
        _doorManager.RotateDoor(-mouseY);
    }

    public bool GetConeInteractableSettings(out Vector3 objPos, out Vector3 axis, out float angle) {
        objPos = _collider.bounds.center;
        axis = transform.forward;
        angle = interactAngle;
        return true;
    }
}