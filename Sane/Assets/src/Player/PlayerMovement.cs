using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    public PlayerStateManager playerState;

    public Rigidbody rb;

    [SerializeField] [Range(0, 2)] private float standingHeight = 1.0f;
    [SerializeField] [Range(0, 2)] private float crouchingHeight = 0.72618f;
    [SerializeField] [Range(0, 1)] private float crouchingLerpSpeed = 0.1f;

    [SerializeField] [Range(0, 5)] private float walkSpeed = 2.1f;
    [SerializeField] [Range(0, 5)] private float crouchSpeed = 1f;
    [SerializeField] [Range(0, 5)] private float runSpeed = 3.5f;
    [SerializeField] [Range(0, 25)] private float acceleration = 13;

    public Transform body;

    private Vector3 _currentDisplacement;
    private Vector3 _targetDisplacement;

    private void Update() {
        Move();
        CheckCrouch();
    }

    private void Move() {
        float forward = playerState.GetPlayerMovementForward();
        float right = playerState.GetPlayerMovementRight();

        _targetDisplacement = transform.right * right + transform.forward * forward;

        float speedMult = 1;
        switch (playerState.GetMovementState()) {
            case MovementState.Crouching:
                speedMult = crouchSpeed;
                break;
            case MovementState.Walking:
                speedMult = walkSpeed;
                break;
            case MovementState.Running:
                speedMult = runSpeed;
                break;
            case MovementState.Idle:
                speedMult = 0;
                break;
        }


        _currentDisplacement =
            Vector3.MoveTowards(_currentDisplacement, _targetDisplacement * speedMult, acceleration * Time.deltaTime);

        rb.velocity = new Vector3(_currentDisplacement.x, rb.velocity.y, _currentDisplacement.z);
    }

    private void CheckCrouch() {
        float targetHeight = playerState.ShouldPlayerCrouch() ? crouchingHeight : standingHeight;
        body.localScale = Vector3.Lerp(body.localScale, new Vector3(1, targetHeight, 1), crouchingLerpSpeed);
        body.transform.localPosition = new Vector3(0, body.localScale.y - standingHeight, 0);
    }
}