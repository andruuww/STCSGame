using UnityEngine;

public class HeadBobController : MonoBehaviour {
    [SerializeField] private bool enable = true;

    [SerializeField] [Range(0, 50f)] private float lerpSpeed = 10f;
    [SerializeField] [Range(0, 100f)] private float decaySpeed = 10f;

    [SerializeField] [Range(0, 0.1f)] private float walkingAmp = 0.0003f;
    [SerializeField] [Range(0, 30)] private float walkingFreq = 10.0f;

    [SerializeField] [Range(0, 0.1f)] private float runningAmp = 0.03f;
    [SerializeField] [Range(0, 30)] private float runningFreq = 20.0f;

    [SerializeField] private new Transform camera;
    public InputManager inputManager;

    private float _currentAmp;
    private float _currentFreq;

    private Vector3 _startPos;

    private void Awake() {
        _startPos = camera.localPosition;
    }

    private void Update() {
        if (!enable) return;

        CheckMotion();
        ResetPosition();
        // camera.LookAt(FocusTarget());

        print(_currentAmp);
    }


    private Vector3 FootStepMotion(float amp, float freq) {
        Vector3 pos = Vector3.zero;
        pos.y += Mathf.Sin(Time.time * freq) * amp;
        pos.x += Mathf.Cos(Time.time * freq / 2) * amp * 1;
        return pos;
    }

    private void CheckMotion() {
        switch (inputManager.GetPlayerState()) {
            case PlayerState.Walking:
                _currentFreq = walkingFreq;
                LerpAmp(walkingAmp);
                break;
            case PlayerState.Running:
                _currentFreq = runningFreq;
                LerpAmp(runningAmp);
                break;
            default:
                _currentFreq = 0;
                LerpAmp(0);
                break;
        }

        PlayMotion(FootStepMotion(_currentAmp, _currentFreq));
    }

    private void ResetPosition() {
        if (camera.localPosition == _startPos) return;
        camera.localPosition = Vector3.Lerp(camera.localPosition, _startPos, 1 * Time.deltaTime);
    }

    private void LerpAmp(float targetAmp) {
        float speed = _currentAmp > targetAmp ? decaySpeed : lerpSpeed;
        _currentAmp = Mathf.Lerp(_currentAmp, targetAmp, Time.deltaTime * speed);
    }

    // private Vector3 FocusTarget() {
    //     Vector3 pos = new(transform.position.x, transform.position.y + cameraHolder.localPosition.y,
    //         transform.position.z);
    //
    //     pos += cameraHolder.forward * 15.0f;
    //     return pos;
    // }

    private void PlayMotion(Vector3 motion) {
        camera.localPosition += motion;
    }
}