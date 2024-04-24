using UnityEngine;

public class DoorManager : MonoBehaviour {
    [Header("Door Settings")]
    [SerializeField] [Range(-360, 360)] private float minAngle;
    [SerializeField] [Range(-360, 360)] private float maxAngle = 90;
    [SerializeField] [Range(0, 10)] private float lerpSpeed = 2;

    [Header("Sounds")]
    [SerializeField] private AudioSource doorSoundsAudioSource;
    [SerializeField] private AudioClip[] doorOpenSounds;
    [SerializeField] [Range(0, 5)] private float threshold = 0.1f;

    public Rigidbody rb;

    private bool _isRotating;
    private float _startAngle;

    private float _target;

    public void Awake() {
        _startAngle = rb.rotation.eulerAngles.y;
        _target = _startAngle;
        rb.centerOfMass = Vector3.zero;
    }


    private void Update() {
        rb.rotation = Quaternion.Lerp(rb.rotation, Quaternion.Euler(0, _target, 0), Time.deltaTime * lerpSpeed);

        if (!_isRotating && Quaternion.Angle(rb.rotation, Quaternion.Euler(0, _target, 0)) > threshold)
            doorSoundsAudioSource.PlayOneShot(doorOpenSounds[Random.Range(0, doorOpenSounds.Length)]);

        _isRotating = Quaternion.Angle(rb.rotation, Quaternion.Euler(0, _target, 0)) > 0.05;
    }

    public void RotateDoor(float deg) {
        _target += deg;
        // dont ask...
        _target = Mathf.Clamp(_target, _startAngle - maxAngle, _startAngle + minAngle);
    }
}