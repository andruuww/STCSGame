using Unity.Mathematics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class RealisticCameraMovementController : MonoBehaviour {
    [SerializeField] private bool enable = true;


    [Header("Footstep")] [SerializeField] [Range(0, 5)]
    private float footstepHeaviness = 1.0f;

    [SerializeField] [Range(0, 1)] private float footstepThreshold = 0.7f;

    [SerializeField] [Range(0, 1)] private float noiseAmp = 0.5f;
    [SerializeField] [Range(0, 2)] private float noiseSpeed = 0.5f;

    [Header("Lerp Speed")] [SerializeField] [Range(0, 50f)]
    private float lerpSpeed = 0.8f;

    [SerializeField] [Range(0, 20f)] private float decaySpeed = 5f;

    [Header("Idle Motion")] [SerializeField] [Range(0, 10f)]
    private float idleAmpRot = 4f;

    [SerializeField] [Range(0, 2f)] private float idleFreq = 1f;

    [Header("Crouching Motion")] [SerializeField] [Range(0, 5f)]
    private float crouchingAmpTrans = 0.2f;

    [SerializeField] [Range(0, 50f)] private float crouchingAmpRot = 5f;
    [SerializeField] [Range(0, 30)] private float crouchingFreq = 8;

    [Header("Walking Motion")] [SerializeField] [Range(0, 5f)]
    private float walkingAmpTrans = 0.25f;

    [SerializeField] [Range(0, 50f)] private float walkingAmpRot = 8f;
    [SerializeField] [Range(0, 30f)] private float walkingFreq = 10.0f;

    [Header("Running Motion")] [SerializeField] [Range(0, 5f)]
    private float runningAmpTrans = 1f;

    [SerializeField] [Range(0, 50f)] private float runningAmpRot = 35f;
    [SerializeField] [Range(0, 30f)] private float runningFreq = 20.0f;

    [Header("Camera FOV")] [SerializeField] [Range(0, 120)]
    private float walkingFOV = 60.0f;

    [SerializeField] [Range(0, 120)] private float runningFOV = 75f;

    [SerializeField] private new Camera camera;
    public PlayerStateManager inputManager;
    private float _currentAmpRot;

    private float _currentAmpTrans;

    private float _currentFOV;
    private float _currentFreqRot;
    private float _currentFreqTrans;
    private Vector3 _startPos;

    private void Awake() {
        _startPos = camera.transform.localPosition;
        _currentFOV = walkingFOV;
    }

    private void Update() {
        if (!enable) return;

        CheckMotion();
        // camera.LookAt(FocusTarget());
    }


    private (Vector3 translation, Vector3 rotation) FootStepMotion(float ampTrans, float ampRot, float freq) {
        Vector3 translation = Vector3.zero;
        Vector3 rotation = Vector3.zero;
        float time = Time.time * freq;
        float sinValue = Mathf.Sin(time);
        float cosValue = Mathf.Cos(time / 2);

        // Calculate translation
        translation.y += sinValue * ampTrans;
        translation.x += cosValue * ampTrans * 1;

        // footstep animation code
        float direction = -Mathf.Sign(cosValue);

        if (sinValue >= footstepThreshold) {
            float footNoise = noise.snoise(new float2(time * noiseSpeed, 0)); // Use Simplex noise
            rotation.z += (footstepHeaviness * direction +
                           footNoise * noiseAmp) * ampRot;

            rotation.x += (footstepHeaviness +
                           footNoise * noiseAmp) * ampRot * 0.75f;
        }

        return (translation * Time.deltaTime, rotation * Time.deltaTime);
    }


    private (Vector3 translation, Vector3 rotation) BreathingMotion(float ampTrans, float ampRot, float freq) {
        Vector3 rotation = Vector3.zero;
        rotation.x = Mathf.Sin(Time.time * freq) * ampRot * 5;
        rotation.y = Mathf.Sin(Time.time * freq * 1.5f) * ampRot / 2;
        rotation.z = Mathf.Sin(Time.time * freq * 2.0f) * ampRot / 2;

        return (Vector3.zero, rotation * Time.deltaTime);
    }


    private void CheckMotion() {
        PlayerState currentState = inputManager.GetPlayerState();
        float freq = idleFreq; // Default frequency

        switch (currentState) {
            case PlayerState.Crouching:
                freq = crouchingFreq;
                LerpAmp(crouchingAmpTrans, crouchingAmpRot);
                break;
            case PlayerState.Walking:
                freq = walkingFreq;
                LerpAmp(walkingAmpTrans, walkingAmpRot);
                break;
            case PlayerState.Running:
                freq = runningFreq;
                LerpAmp(runningAmpTrans, runningAmpRot);
                break;
            case PlayerState.Idle:
                freq = idleFreq;
                LerpAmp(0, idleAmpRot);
                ResetPosition();
                break;
            default:
                LerpAmp(0, 0); // No motion
                break;
        }

        float targetFOV = currentState == PlayerState.Running ? runningFOV : walkingFOV;
        LerpFOV(targetFOV);

        (Vector3 translation, Vector3 rotation) = currentState == PlayerState.Idle
            ? BreathingMotion(_currentAmpTrans, _currentAmpRot, freq)
            : FootStepMotion(_currentAmpTrans, _currentAmpRot, freq);

        PlayMotion(translation, rotation);
    }


    private void ResetPosition() {
        if (camera.transform.localPosition == _startPos) return;
        camera.transform.localPosition = Vector3.Lerp(camera.transform.localPosition, _startPos, 1 * Time.deltaTime);
    }

    private void LerpAmp(float targetAmpTrans, float targetAmpRot) {
        float speed = _currentAmpTrans > targetAmpTrans ? decaySpeed : lerpSpeed;
        _currentAmpTrans = Mathf.Lerp(_currentAmpTrans, targetAmpTrans, Time.deltaTime * speed);

        speed = _currentAmpRot > targetAmpRot ? decaySpeed : lerpSpeed;
        _currentAmpRot = Mathf.Lerp(_currentAmpRot, targetAmpRot, Time.deltaTime * speed);
    }

    private void LerpFOV(float targetFOV) {
        _currentFOV = Mathf.Lerp(_currentFOV, targetFOV, Time.deltaTime * lerpSpeed);
        camera.fieldOfView = _currentFOV;
    }

    // private Vector3 FocusTarget() {
    //     Vector3 pos = new(transform.position.x, transform.position.y + cameraHolder.localPosition.y,
    //         transform.position.z);
    //
    //     pos += cameraHolder.forward * 15.0f;
    //     return pos;
    // }

    private void PlayMotion(Vector3 positionMotion, Vector3 rotationMotion) {
        camera.transform.localPosition += positionMotion;
        camera.transform.localRotation *= Quaternion.Euler(rotationMotion);
    }
}