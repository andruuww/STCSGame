using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    public PlayerStateManager playerState;

    public CharacterController characterController;

    [SerializeField] [Range(0, 2)] private float standingHeight = 1.0f;
    [SerializeField] [Range(0, 2)] private float crouchingHeight = 0.5f;
    [SerializeField] [Range(0, 1)] private float crouchingLerpSpeed = 0.1f;

    [SerializeField] [Range(0, 5)] private float walkSpeed = 1.9f;
    [SerializeField] [Range(0, 5)] private float crouchSpeed = 1f;
    [SerializeField] [Range(0, 5)] private float runSpeed = 3f;
    [SerializeField] [Range(0, 25)] private float acceleration = 13;
    [SerializeField] [Range(0, 20)] private float gravity = 9.8f;

    public Transform bodyHeight;
    public Transform body;

    private Vector3 _currentDisplacement;

    private bool _locked;
    private Vector3 _targetDisplacement;

    private float _verticalSpeed;

    private void Update() {
        if (_locked) return;
        Move();
        CheckCrouch();
    }

    private void Move() {
        float forward = playerState.GetPlayerMovementForward();
        float right = playerState.GetPlayerMovementRight();

        _targetDisplacement = body.right * right + body.forward * forward;

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

        _currentDisplacement = Vector3.MoveTowards(_currentDisplacement, _targetDisplacement * speedMult,
            acceleration * Time.deltaTime);


        characterController.Move(_currentDisplacement * Time.deltaTime);

        // apply gravity

        if (!characterController.isGrounded)
            _verticalSpeed -= gravity * Time.deltaTime;
        else
            _verticalSpeed = 0f;

        _currentDisplacement.y = _verticalSpeed;
    }

    private void CheckCrouch() {
        float targetHeight = playerState.ShouldPlayerCrouch() ? crouchingHeight : standingHeight;
        bodyHeight.localScale =
            Vector3.Lerp(bodyHeight.localScale, new Vector3(1, targetHeight, 1), crouchingLerpSpeed);
        bodyHeight.transform.localPosition = new Vector3(0, bodyHeight.localScale.y - standingHeight,
            0);
    }

    public void Lock() {
        _locked = true;
    }

    public void Unlock() {
        _locked = false;
    }
}