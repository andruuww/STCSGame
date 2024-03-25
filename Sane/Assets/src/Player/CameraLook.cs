using UnityEngine;

public class CameraLook : MonoBehaviour {
    public InputManager inputManager;
    public float mouseSensitivity = 100;

    public Transform body;
    public new Camera camera;

    private float _xRotation;

    private void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        _xRotation = 0;
    }

    private void Update() {
        HandleVertical();
        HandleHorizontal();
    }

    private void HandleVertical() {
        float mouseY = inputManager.GetMouseY() * Time.deltaTime * mouseSensitivity;

        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);
        camera.transform.localRotation = Quaternion.Euler(_xRotation, 0, 0);
    }

    private void HandleHorizontal() {
        float mouseX = inputManager.GetMouseX() * Time.deltaTime * mouseSensitivity;
        transform.Rotate(Vector3.up * mouseX);
    }
}