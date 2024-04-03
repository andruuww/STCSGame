using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class PlayerStateManager : MonoBehaviour {
    [Header("Ground Detection")]
    [SerializeField] private float groundedDistance = 0.5f;
    [SerializeField] private LayerMask groundLayer;
    private PlayerInput playerInput;

    private void Awake() {
        playerInput = new PlayerInput();
        Application.targetFrameRate = 60;
    }

    private void OnEnable() {
        playerInput.Enable();
    }

    private void OnDisable() {
        playerInput.Disable();
    }

    // player input getters ------------------------------------------------------------------------------------------------
    public float GetPlayerMovementForward() {
        return playerInput.Movement.Forward.ReadValue<float>();
    }

    public float GetPlayerMovementRight() {
        return playerInput.Movement.Right.ReadValue<float>();
    }

    public float GetMouseX() {
        return playerInput.CameraLook.MouseX.ReadValue<float>();
    }

    public float GetMouseY() {
        return playerInput.CameraLook.MouseY.ReadValue<float>();
    }

    public bool IsRunningButtonDown() {
        return playerInput.Movement.Run.ReadValue<float>() != 0;
    }

    public bool IsCrouchingButtonDown() {
        return playerInput.Movement.Crouch.ReadValue<float>() != 0;
    }


    // player state getters ------------------------------------------------------------------------------------------------

    public bool ShouldPlayerCrouch() {
        return !IsPlayerRunning() && IsCrouchingButtonDown();
    }

    public bool ShouldPlayerRun() {
        return !IsCrouchingButtonDown() && IsRunningButtonDown();
    }

    public bool IsPlayerMoving() {
        return playerInput.Movement.Forward.ReadValue<float>() != 0 ||
               playerInput.Movement.Right.ReadValue<float>() != 0;
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