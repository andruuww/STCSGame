using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    public InputManager inputManager;

    public Rigidbody rb;

    public float speed = 10;
    public float runSpeed = 15;

    public float acceleration = 10;

    private Vector3 _currentDisplacement;
    private Vector3 _targetDisplacement;

    private void Update() {
        Move();
        Crouch();
    }

    private void Move() {
        float forward = inputManager.GetPlayerMovementForward();
        float right = inputManager.GetPlayerMovementRight();

        _targetDisplacement = transform.right * right + transform.forward * forward;
        _targetDisplacement *= inputManager.ShouldPlayerRun() ? runSpeed : speed;

        _currentDisplacement =
            Vector3.MoveTowards(_currentDisplacement, _targetDisplacement, acceleration * Time.deltaTime);

        rb.velocity = new Vector3(_currentDisplacement.x, rb.velocity.y, _currentDisplacement.z);
    }

    private void Crouch() {
        transform.localScale = new Vector3(1, inputManager.ShouldPlayerCrouch() ? 0.72618f : 1f, 1);
    }
}