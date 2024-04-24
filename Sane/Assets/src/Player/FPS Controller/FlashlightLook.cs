using UnityEngine;

public class FlashlightLook : MonoBehaviour {
    [SerializeField] private PlayerStateManager playerStateManager;
    [SerializeField] private float mouseSensitivity = 10f;

    [SerializeField] private float flashlightSpeed = 10f;

    [SerializeField] private float minVerticalAngle = -90;
    [SerializeField] private float maxVerticalAngle = 90;

    public Transform cameraRotation;
    public Transform body;

    public Light flashlight;
    public AudioSource flashlightAudioSource;

    private Quaternion _hTarget;
    private Quaternion _vTarget;

    private float _xRotation;
    private float _yRotation;

    private bool locked;

    private void Start() {
        Cursor.lockState = CursorLockMode.Locked;

        _vTarget = cameraRotation.localRotation;
        _hTarget = cameraRotation.localRotation;
    }

    private void Update() {
        if (locked) return;
        HandleVertical();
        HandleHorizontal();

        LerpToTarget();

        if (playerStateManager.IsFlashlightPress()) {
            flashlight.enabled = !flashlight.enabled;
            flashlightAudioSource.Play();
        }
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
            Quaternion.Lerp(cameraRotation.localRotation, _vTarget, flashlightSpeed * Time.deltaTime);

        body.localRotation =
            Quaternion.Lerp(body.localRotation, _hTarget, flashlightSpeed * Time.deltaTime);
    }

    public void Lock() {
        locked = true;
    }

    public void Unlock() {
        locked = false;
    }
}