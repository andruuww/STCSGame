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

    public float GetPlayerMovementForward() {
        return playerInput.Movement.Forward.ReadValue<float>();
    }

    public float GetPlayerMovementRight() {
        return playerInput.Movement.Right.ReadValue<float>();
    }

    public bool IsPlayerRunning() {
        return playerInput.Movement.Run.ReadValue<float>() != 0;
    }

    public bool IsPlayerCrouching() {
        return playerInput.Movement.Crouch.ReadValue<float>() != 0;
    }

    public float GetMouseX() {
        return playerInput.CameraLook.MouseX.ReadValue<float>();
    }

    public float GetMouseY() {
        return playerInput.CameraLook.MouseY.ReadValue<float>();
    }
}