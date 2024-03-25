using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    public InputManager inputManager;

    public Rigidbody rb;

    public float speed = 10;
    public float runSpeed = 15;

    private void Update() {
        float forward = inputManager.GetPlayerMovementForward();
        float right = inputManager.GetPlayerMovementRight();

        Vector3 displacement = transform.right * right + transform.forward * forward;
        displacement *= inputManager.IsPlayerRunning() ? runSpeed : speed;

        transform.localScale = new Vector3(1, inputManager.IsPlayerCrouching() ? 0.72618f : 1f, 1);

        rb.velocity = new Vector3(displacement.x, rb.velocity.y, displacement.z);
    }
}