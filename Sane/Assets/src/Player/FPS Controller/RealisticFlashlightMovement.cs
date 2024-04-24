using UnityEngine;

public class RealisticFlashlightMovement : MonoBehaviour {
    [SerializeField] private PlayerStateManager playerStateManager;
    [SerializeField] private GameObject flashlight;
    [SerializeField] private GameObject flashlightHolder;
    [SerializeField] private GameObject flashlightOffset;
    [SerializeField] private Transform body;
    [SerializeField] private new Transform camera;

    [SerializeField] [Range(0, 5)] private float cameraLerpSpeed = 1.0f;
    [SerializeField] [Range(0, 90)] private float runningFlashlightAngle = 90;
    [SerializeField] [Range(0, 30)] private float runningDuckSpeed = 30;
    [SerializeField] [Range(0, 30)] private float runningRecoversSpeed = 30;

    private void Update() {
        flashlightHolder.transform.position = body.transform.position;
        RotateBody();
        RotateCameraUpDown();

        if (playerStateManager.GetMovementState() == MovementState.Running)
            flashlightOffset.transform.localRotation = Quaternion.Slerp(flashlightOffset.transform.localRotation,
                Quaternion.Euler(runningFlashlightAngle, 0, 0), runningDuckSpeed * Time.deltaTime);
        else
            flashlightOffset.transform.localRotation = Quaternion.Slerp(flashlightOffset.transform.localRotation,
                Quaternion.Euler(0, 0, 0), runningRecoversSpeed * Time.deltaTime);
    }

    private void RotateBody() {
        Quaternion targetRotation = Quaternion.Euler(
            body.rotation.eulerAngles.x,
            flashlightHolder.transform.rotation.eulerAngles.y,
            body.rotation.eulerAngles.z
        );

        body.rotation = Quaternion.Slerp(body.rotation, targetRotation, cameraLerpSpeed * Time.deltaTime);
    }

    private void RotateCameraUpDown() {
        float angleX = flashlight.transform.localRotation.eulerAngles.x;
        Quaternion targetRotation = Quaternion.Euler(
            angleX,
            0f,
            0f
        );

        camera.localRotation = Quaternion.Slerp(camera.localRotation, targetRotation, cameraLerpSpeed * Time.deltaTime);
    }
}