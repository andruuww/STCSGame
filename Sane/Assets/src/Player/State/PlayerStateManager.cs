using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class PlayerStateManager : MonoBehaviour {
    [Header("Ground Detection")]
    [SerializeField] private float groundedDistance = 0.5f;
    [SerializeField] private LayerMask groundLayer;

    public PlayerMovement playerMovement;
    public SmoothLook smoothLook;

    private PlayerInput _playerInput;
    private bool lockMovement;

    private void Awake() {
        playerMovement = GetComponent<PlayerMovement>();
        smoothLook = GetComponent<SmoothLook>();

        _playerInput = new PlayerInput();
        Application.targetFrameRate = 60;
    }

    private void OnEnable() {
        _playerInput.Enable();
    }

    private void OnDisable() {
        _playerInput.Disable();
    }

    public void LockMovement() {
        lockMovement = true;
        playerMovement.Lock();
    }

    public void UnlockMovement() {
        lockMovement = false;
        playerMovement.Unlock();
    }

    public void LockCamera() {
        smoothLook.Lock();
    }

    public void UnlockCamera() {
        smoothLook.Unlock();
    }


    // player interact getters --------------------------------------------------------------------------------------------

    public bool IsInteractPress() {
        return _playerInput.Interactions.InteractPress.triggered;
    }

    public bool IsInteractDown() {
        return _playerInput.Interactions.InteractDown.ReadValue<float>() != 0;
    }


    // player input getters ------------------------------------------------------------------------------------------------
    public float GetPlayerMovementForward() {
        return _playerInput.Movement.Forward.ReadValue<float>();
    }

    public float GetPlayerMovementRight() {
        return _playerInput.Movement.Right.ReadValue<float>();
    }

    public float GetMouseX() {
        return _playerInput.CameraLook.MouseX.ReadValue<float>();
    }

    public float GetMouseY() {
        return _playerInput.CameraLook.MouseY.ReadValue<float>();
    }

    public bool IsRunningButtonDown() {
        return _playerInput.Movement.Run.ReadValue<float>() != 0;
    }

    public bool IsCrouchingButtonDown() {
        return _playerInput.Movement.Crouch.ReadValue<float>() != 0;
    }


    // player state getters ------------------------------------------------------------------------------------------------

    public bool ShouldPlayerCrouch() {
        return !IsPlayerRunning() && IsCrouchingButtonDown();
    }

    public bool ShouldPlayerRun() {
        return !IsCrouchingButtonDown() && IsRunningButtonDown();
    }

    public bool IsPlayerMoving() {
        return GetPlayerMovementForward() != 0 ||
               GetPlayerMovementRight() != 0;
    }

    public bool IsPlayerCrouching() {
        return ShouldPlayerCrouch() && IsPlayerMoving();
    }

    public bool IsPlayerWalking() {
        return IsPlayerMoving() && !ShouldPlayerRun();
    }

    public bool IsPlayerRunning() {
        return ShouldPlayerRun() && IsPlayerMoving();
    }

    public bool IsGrounded() {
        return Physics.CheckSphere(transform.position - new Vector3(0, 1, 0), groundedDistance, groundLayer);
    }

    public MovementState GetMovementState() {
        if (lockMovement) return MovementState.Idle;
        if (IsPlayerCrouching()) return MovementState.Crouching;
        if (IsPlayerRunning()) return MovementState.Running;
        if (IsPlayerWalking()) return MovementState.Walking;
        return MovementState.Idle;
    }

    public PlayerState GetPlayerState() {
        return new PlayerState {
            Movement = GetMovementState(),
            IsGrounded = IsGrounded()
        };
    }
}

public enum MovementState {
    Idle,
    Walking,
    Running,
    Crouching
}

public struct PlayerState {
    public MovementState Movement;
    public bool IsGrounded;
}