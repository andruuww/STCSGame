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

public class InteractManager : MonoBehaviour {
    [SerializeField] private LayerMask interactLayer;
    [SerializeField] private Transform interactorSource;
    [SerializeField] [Range(0, 20)] private int maxItems = 10;
    [SerializeField] [Range(0, 20)] private float interactDistance = 2f;
    [SerializeField] [Range(0, 1)] private float interactThreshold = 0.5f;
    [SerializeField] [Range(0, 2)] private float uiSize = 0.5f;
    [SerializeField] [Range(0, 1)] private float fadeTime = 0.5f;
    [SerializeField] [Range(0, 1)] private float uiOffset = 0.1f;
    [SerializeField] [Range(0, 50)] private float handSpeed = 50;

    public InteractHandUI ui;
    private bool _isInteracting;
    private bool _isVisible = true;

    private PlayerStateManager _playerStateManager;

    private Collider closestCollider;
    private IInteractable closetInteractable;

    private float minDistance = float.MinValue;

    private void Start() {
        _playerStateManager = GetComponent<PlayerStateManager>();
        ui.transform.localScale = new Vector3(uiSize, uiSize, uiSize);
    }

    private void Update() {
        if (!_isInteracting) {
            minDistance = float.MinValue;
            closetInteractable = null;
            closestCollider = null;

            Collider[] colliders = new Collider[maxItems];
            Physics.OverlapSphereNonAlloc(interactorSource.position, interactDistance, colliders, interactLayer);

            foreach (Collider collider in colliders)
                if (collider is not null &&
                    !Physics.Linecast(interactorSource.position, collider.bounds.center, 1 << interactLayer.value)) {
                    Vector3 direction = (collider.bounds.center - interactorSource.position).normalized;
                    float dot = Vector3.Dot(direction, interactorSource.forward);
                    if (dot > interactThreshold)
                        if (dot > minDistance)
                            if (collider.gameObject.TryGetComponent(out IInteractable interactable) &&
                                IsInteractableCone(interactable)) {
                                minDistance = dot;
                                closetInteractable = interactable;
                                closestCollider = collider;
                            }
                }


            if (closetInteractable is not null && closestCollider is not null) {
                ShowUI();
                HandleInteract(closestCollider);
            }
            else {
                HideUI();
            }
        }
        else {
            ShowUINoLerp();
            HandleInteract(closestCollider);
        }

        AlignCanvas();
    }

    private void AlignCanvas() {
        ui.transform.LookAt(interactorSource);
    }

    private void HandleInteract(Collider collider) {
        if (collider.gameObject.TryGetComponent(out DoorInteractable door)) {
            if (_playerStateManager.IsInteractDown()) {
                ui.CloseHand();
                _isInteracting = true;
                _playerStateManager.LockCamera();
                _playerStateManager.LockMovement();
                door.Interact(_playerStateManager);
            }
            else {
                _isInteracting = false;
                _playerStateManager.UnlockCamera();
                _playerStateManager.UnlockMovement();
                ui.OpenHand();
            }
        }
    }

    private void ShowUI() {
        Vector3 uiPos = CalculateUIPosition(closestCollider.bounds.center);
        if (!_isVisible) {
            ui.fadeIn(fadeTime);
            ui.transform.position = uiPos;
            _isVisible = true;
        }

        ui.transform.position =
            Vector3.Lerp(ui.transform.position, uiPos,
                Time.deltaTime * handSpeed);
    }

    private void ShowUINoLerp() {
        ui.transform.position = CalculateUIPosition(closestCollider.bounds.center);
    }

    private void HideUI() {
        if (_isVisible)
            ui.fadeOut(fadeTime);
        _isVisible = false;
    }

    // --------------------------------------------------------------------------------

    public static bool IsInteractableCone(IInteractable interactable) {
        if (interactable.GetConeInteractableSettings(out Vector3 objPos, out Vector3 axis, out float angle))
            return Vector3.Angle(Camera.main.transform.position - objPos, axis) < angle;
        return true;
    }

    public Vector3 CalculateUIPosition(Vector3 obj) {
        Physics.Raycast(interactorSource.position, obj - interactorSource.position, out RaycastHit hit, interactLayer);
        // Debug.DrawRay(interactorSource.position, obj - interactorSource.position, Color.red);
        Vector3 pos = hit.point;
        pos = Vector3.MoveTowards(pos, interactorSource.position, uiOffset);
        return pos;
    }
}