using UnityEngine;

public class InputManager : MonoBehaviour {
    private PlayerInput playerInput;

    private void Awake() {
        playerInput = new PlayerInput();
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
        return ShouldPlayerCrouch();
    }

    public bool IsPlayerWalking() {
        return IsPlayerMoving() && !ShouldPlayerRun();
    }

    public bool IsPlayerRunning() {
        return ShouldPlayerRun() && IsPlayerMoving();
    }


    public PlayerState GetPlayerState() {
        if (IsPlayerCrouching()) return PlayerState.Crouching;
        if (IsPlayerRunning()) return PlayerState.Running;
        if (IsPlayerWalking()) return PlayerState.Walking;
        return PlayerState.Idle;
    }
}

public enum PlayerState {
    Idle,
    Walking,
    Running,
    Crouching
}