using UnityEngine;

public class HeadBobController : MonoBehaviour {
    [SerializeField] private bool enable = true;

    [SerializeField] [Range(0, 0.1f)] private float amplitude = 0.015f;
    [SerializeField] [Range(0, 30)] private float frequency = 10.0f;

    [SerializeField] private new Transform camera;
    [SerializeField] private Transform cameraHolder;
    [SerializeField] private Rigidbody rb;

    private readonly float toggleSpeed = 1.0f;
    private Vector3 startPos;

    private void Awake() {
        startPos = camera.localPosition;
    }

    private void Update() {
        if (!enable) return;

        CheckMotion();
        ResetPosition();
        // camera.LookAt(FocusTarget());
    }


    private Vector3 FootStepMotion() {
        Vector3 pos = Vector3.zero;
        pos.y += Mathf.Sin(Time.time * frequency) * amplitude;
        pos.x += Mathf.Cos(Time.time * frequency / 2) * amplitude * 1;
        return pos;
    }

    private void CheckMotion() {
        float speed = new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude;

        if (speed < toggleSpeed) return;

        PlayMotion(FootStepMotion());
    }

    private void ResetPosition() {
        if (camera.localPosition == startPos) return;
        camera.localPosition = Vector3.Lerp(camera.localPosition, startPos, 1 * Time.deltaTime);
    }

    private Vector3 FocusTarget() {
        Vector3 pos = new(transform.position.x, transform.position.y + cameraHolder.localPosition.y,
            transform.position.z);

        pos += cameraHolder.forward * 15.0f;
        return pos;
    }

    private void PlayMotion(Vector3 motion) {
        camera.localPosition += motion;
    }
}