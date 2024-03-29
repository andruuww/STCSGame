using UnityEngine;

public class SmoothLook : MonoBehaviour {
    [SerializeField] private PlayerStateManager playerStateManager;
    [SerializeField] private float mouseSensitivity = 100;

    [SerializeField] private float cameraSpeed = 0.001f;

    [SerializeField] private float minVerticalAngle = -90;
    [SerializeField] private float maxVerticalAngle = 90;

    public Transform cameraRotation;

    private Quaternion _hTarget;
    private Quaternion _vTarget;

    private float _xRotation;
    private float _yRotation;

    private void Start() {
        Cursor.lockState = CursorLockMode.Locked;

        _vTarget = transform.localRotation;
        _hTarget = transform.localRotation;
    }

    private void Update() {
        HandleVertical();
        HandleHorizontal();

        LerpToTarget();
    }

    private void HandleVertical() {
        float mouseY = playerStateManager.GetMouseY() * Time.deltaTime * mouseSensitivity;

        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, minVerticalAngle, maxVerticalAngle);
        _vTarget.x = _xRotation * Mathf.Deg2Rad;
    }

    private void HandleHorizontal() {
        float mouseX = playerStateManager.GetMouseX() * Time.deltaTime * mouseSensitivity;
        _yRotation += mouseX * 2;
        _hTarget = Quaternion.Euler(0, _yRotation, 0);
    }

    private void LerpToTarget() {
        cameraRotation.localRotation =
            Quaternion.Slerp(cameraRotation.localRotation, _vTarget, cameraSpeed * Time.deltaTime);

        transform.localRotation =
            Quaternion.Slerp(transform.localRotation, _hTarget, cameraSpeed * Time.deltaTime);
    }
}