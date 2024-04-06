using UnityEngine;
using Quaternion = UnityEngine.Quaternion;

public class DoorManager : MonoBehaviour {
    [SerializeField] [Range(0, 360)] private float minAngle;
    [SerializeField] [Range(0, 360)] private float maxAngle = 90;
    [SerializeField] [Range(0, 10)] private float lerpSpeed = 2;

    [SerializeField] private Transform pivot;
    public Rigidbody rb;
    private float _startAngle;

    private float _target;

    public void Awake() {
        _startAngle = pivot.rotation.eulerAngles.y;
        _target = _startAngle;
    }

    public void RotateDoor(float deg) {
        _target += deg;
        // dont ask...
        _target = Mathf.Clamp(_target, _startAngle - maxAngle, _startAngle + minAngle);
        RotatePivot();
    }


    private void RotatePivot() {
        rb.rotation = Quaternion.Lerp(rb.rotation, Quaternion.Euler(0, _target, 0), Time.deltaTime * lerpSpeed);
        // pivot.localRotation =
        //     Quaternion.Lerp(pivot.localRotation, Quaternion.Euler(0, _target, 0), Time.deltaTime * lerpSpeed);
    }
}