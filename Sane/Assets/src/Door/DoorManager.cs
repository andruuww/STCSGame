using UnityEngine;

public class DoorManager : MonoBehaviour {
    [SerializeField] [Range(-360, 360)] private float minAngle;
    [SerializeField] [Range(-360, 360)] private float maxAngle = 90;
    [SerializeField] [Range(0, 10)] private float lerpSpeed = 2;

    public Rigidbody rb;
    private float _startAngle;

    private float _target;

    public void Awake() {
        _startAngle = rb.rotation.eulerAngles.y;
        _target = _startAngle;
        rb.centerOfMass = Vector3.zero;
    }


    private void Update() {
        rb.rotation = Quaternion.Lerp(rb.rotation, Quaternion.Euler(0, _target, 0), Time.deltaTime * lerpSpeed);
    }

    public void RotateDoor(float deg) {
        _target += deg;
        // dont ask...
        _target = Mathf.Clamp(_target, _startAngle - maxAngle, _startAngle + minAngle);
    }
}