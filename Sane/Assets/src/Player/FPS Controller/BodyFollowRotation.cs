using UnityEngine;

public class BodyFollowRotation : MonoBehaviour {
    [SerializeField] private Transform follow;

    private Collider _collider;

    private void Start() {
        _collider = GetComponent<Collider>();
    }

    private void Update() {
        _collider.transform.rotation = follow.rotation;
    }
}