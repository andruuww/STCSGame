using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    public PlayerStateManager inputManager;

    public Rigidbody rb;

    [SerializeField] [Range(0, 2)] private float standingHeight = 1.0f;
    [SerializeField] [Range(0, 2)] private float crouchingHeight = 0.72618f;
    [SerializeField] [Range(0, 1)] private float crouchingLerpSpeed = 0.1f;

    [SerializeField] [Range(0, 5)] private float walkSpeed = 2.1f;
    [SerializeField] [Range(0, 5)] private float crouchSpeed = 1f;
    [SerializeField] [Range(0, 5)] private float runSpeed = 3.5f;
    [SerializeField] [Range(0, 25)] private float acceleration = 13;


    private Vector3 _currentDisplacement;
    private Vector3 _targetDisplacement;

    private void Update() {
        CheckCrouch();
        Move();
    }

    private void Move() {
        float forward = inputManager.GetPlayerMovementForward();
        float right = inputManager.GetPlayerMovementRight();

        _targetDisplacement = transform.right * right + transform.forward * forward;

        float multiplier = 1;
        switch (inputManager.GetPlayerState()) {
            case PlayerState.Crouching:
                multiplier = crouchSpeed;
                break;
            case PlayerState.Walking:
                multiplier = walkSpeed;
                break;
            case PlayerState.Running:
                multiplier = runSpeed;
                break;
            case PlayerState.Idle:
                multiplier = 0;
                break;
        }

        _targetDisplacement *= multiplier;

        _currentDisplacement =
            Vector3.MoveTowards(_currentDisplacement, _targetDisplacement, acceleration * Time.deltaTime);

        rb.velocity = new Vector3(_currentDisplacement.x, rb.velocity.y, _currentDisplacement.z);
    }

    private void CheckCrouch() {
        float targetHeight = inputManager.ShouldPlayerCrouch() ? crouchingHeight : standingHeight;
        transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1, targetHeight, 1), crouchingLerpSpeed);
        transform.localPosition = new Vector3(transform.localPosition.x,
            transform.localScale.y + standingHeight,
            transform.localPosition.z);
    }
}