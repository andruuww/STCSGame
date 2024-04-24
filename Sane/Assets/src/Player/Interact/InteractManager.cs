using UnityEngine;

public interface IInteractable {
    bool GetConeInteractableSettings(out Vector3 objPos, out Vector3 axis, out float angle);
}

public interface DoorInteractable : IInteractable {
    void Interact(PlayerStateManager playerStateManager);
}

public interface InspectionInteractable : IInteractable {
}

public interface PickupInteractable : IInteractable {
}

public interface UseInteractable : IInteractable {
}

public interface LightswitchInteractable : IInteractable {
    void Interact();
}

public class InteractManager : MonoBehaviour {
    [SerializeField] private LayerMask interactLayer;
    [SerializeField] private Transform interactorSource;
    [SerializeField] [Range(0, 20)] private int maxItems = 10;
    [SerializeField] [Range(0, 20)] private float interactDistance = 2f;
    [SerializeField] [Range(0, 1)] private float interactLookThreshold = 0.5f;
    [SerializeField] [Range(0, 2)] private float uiSize = 0.5f;
    [SerializeField] [Range(0, 1)] private float fadeTime = 0.5f;
    [SerializeField] [Range(0, 1)] private float uiOffset = 0.1f;
    [SerializeField] [Range(0, 50)] private float handSpeed = 50;
    [SerializeField] [Range(0, 1)] private float shakeIntensity = 0.05f;
    [SerializeField] [Range(0, 10)] private float shakeFrequency = 1f;

    public InteractHandUI ui;
    private bool _isInteracting;
    private bool _isLerping;

    private PlayerStateManager _playerStateManager;

    private Collider lastCollider;

    private void Start() {
        _playerStateManager = GetComponent<PlayerStateManager>();
        ui.transform.localScale = new Vector3(uiSize, uiSize, uiSize);
    }

    private void Update() {
        if (_isInteracting &&
            Vector3.Distance(interactorSource.position, lastCollider.bounds.center) <= interactDistance) {
            HandleInteract(lastCollider);
            SetUIPos(lastCollider);
            _isLerping = false;
        }
        else {
            CalculateClosestInteractable(out Collider closestCollider);
            HandleInteract(closestCollider);
            AlignCanvas();
            if (closestCollider is null) {
                ui.fadeOut(fadeTime);
                SetUIPos(lastCollider);
                _isLerping = false;
                lastCollider = null;
                return;
            }

            if (lastCollider is not null && closestCollider != lastCollider)
                _isLerping = true;
            else if (Vector3.Distance(ui.transform.position, CalculateUIPosition(closestCollider.bounds.center)) <
                     0.01)
                _isLerping = false;

            ui.fadeIn(fadeTime);
            if (!_isLerping)
                SetUIPos(closestCollider);
            else
                LerpUIPos(closestCollider);

            lastCollider = closestCollider;
        }
    }

    private void AlignCanvas() {
        ui.transform.LookAt(Camera.main.transform);
    }

    private void HandleInteract(Collider collider) {
        if (collider is not null) {
            if (collider.gameObject.TryGetComponent(out DoorInteractable door) &&
                _playerStateManager.IsInteractDown()) {
                ui.CloseHand();
                _isInteracting = true;
                _playerStateManager.LockCamera();
                door.Interact(_playerStateManager);
                return;
            }

            if (collider.gameObject.TryGetComponent(out LightswitchInteractable lightswitch) &&
                _playerStateManager.IsInteractDown()) {
                ui.CloseHand();
                if (!_isInteracting) {
                    _isInteracting = true;
                    lightswitch.Interact();
                }

                return;
            }
        }

        _isInteracting = false;
        _playerStateManager.UnlockCamera();
        _playerStateManager.UnlockMovement();
        ui.OpenHand();
    }

    private void SetUIPos(Collider closestCollider) {
        if (closestCollider is null) return;
        ui.transform.position = CalculateUIPosition(closestCollider.bounds.center);
    }

    private void LerpUIPos(Collider closestCollider) {
        ui.transform.position =
            Vector3.Lerp(ui.transform.position, CalculateUIPosition(closestCollider.bounds.center),
                Time.deltaTime * handSpeed);
    }

    private void CalculateClosestInteractable(
        out Collider closestCollider) {
        float minDistance = float.MinValue;
        closestCollider = null;

        Collider[] colliders = new Collider[maxItems];
        Physics.OverlapSphereNonAlloc(interactorSource.position, interactDistance, colliders, interactLayer);

        foreach (Collider collider in colliders)
            if (collider is not null &&
                !Physics.Linecast(interactorSource.position, collider.bounds.center, 1 << interactLayer.value)) {
                Vector3 direction = (collider.bounds.center - interactorSource.position).normalized;
                float dot = Vector3.Dot(direction, interactorSource.forward);
                if (dot > interactLookThreshold)
                    if (dot > minDistance)
                        if (collider.gameObject.TryGetComponent(out IInteractable interactable) &&
                            IsInteractableCone(interactable)) {
                            minDistance = dot;
                            closestCollider = collider;
                        }
            }
    }

    // --------------------------------------------------------------------------------

    public static bool IsInteractableCone(IInteractable interactable) {
        if (interactable.GetConeInteractableSettings(out Vector3 objPos, out Vector3 axis, out float angle))
            return Vector3.Angle(Camera.main.transform.position - objPos, axis) < angle;
        return true;
    }

    public Vector3 CalculateUIPosition(Vector3 obj) {
        Physics.Raycast(Camera.main.transform.position, obj - Camera.main.transform.position, out RaycastHit hit,
            interactLayer);
        Debug.DrawRay(Camera.main.transform.position, obj - Camera.main.transform.position, Color.red);
        Vector3 pos = hit.point;
        pos = Vector3.MoveTowards(pos, Camera.main.transform.position, uiOffset);

        float noiseZ = Mathf.PerlinNoise(Time.time * shakeFrequency, 0) * shakeIntensity;
        float noiseY = Mathf.PerlinNoise(0, Time.time * shakeFrequency) * shakeIntensity;
        pos += new Vector3(0, noiseY, noiseZ);

        return pos;
    }
}