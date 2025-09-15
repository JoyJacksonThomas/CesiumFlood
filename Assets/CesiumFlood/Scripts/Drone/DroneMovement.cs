// Adapted from: FreeFlyCamera (Version 1.2) (c) 2019 Sergey Stafeyev	//

using UnityEngine;

public class DroneMovement : MonoBehaviour {
    [Header("Movement")]
    [SerializeField]
    private Transform modelTransform;

    [SerializeField]
    [Tooltip("Sensitivity of mouse rotation")]
    private float mouseSense = 1.8f;

    [SerializeField]
    [Tooltip("Camera movement speed")]
    private float movementSpeed = 10f;

    [SerializeField]
    [Tooltip("Acceleration at camera movement is active")]
    private bool enableAcceleration = true;

    [SerializeField]
    private Vector2 minMaxSpeed = new(10f, 50f);

    /**
     * Time to go from min to max speed
     */
    [SerializeField]
    [Tooltip("Time to reach target speed when accelerating")]
    private float accelerationRate = 5f;

    [SerializeField]
    [Tooltip("Time to reach target speed when decelerating")]
    private float decelerationRate = 5f;

    [Space]
    [Header("Drone Visuals")]
    [SerializeField]
    private float maxTiltAngle = 15f;

    [SerializeField]
    [Tooltip("How quickly the drone tilts to the target angle")]
    private float tiltSpeed = 1f;

    [SerializeField]
    [Tooltip("Controls how rapidly the tilt effect responds to speed changes")]
    private float tiltRate = 0.3f;

    // Speed control variables
    private float currentSpeed;

    private Vector3 deltaPosition = Vector3.zero;
    private Vector3 input;

    private float rotationTarget;
    private float targetSpeed;


    private void FixedUpdate() {
        if (!(Mathf.Abs(transform.parent.eulerAngles.y - rotationTarget) < 0.01f)) {
            float newYRot = Mathf.LerpAngle(transform.parent.eulerAngles.y, rotationTarget, 0.5f);
            transform.parent.eulerAngles =
                new Vector3(transform.parent.eulerAngles.x, newYRot, transform.parent.eulerAngles.z);
        }

        HandleMovement();
        HandleVisuals();
    }

    private void HandleVisuals() {
        // Skip if model transform is not assigned
        if (modelTransform == null) return;

        // Get movement direction, but ignore vertical movement
        // Create a horizontally projected movement vector by zeroing out the Y component
        Vector3 horizontalDelta = new(deltaPosition.x, 0f, deltaPosition.z);

        // Only apply tilt if we have horizontal movement
        if (horizontalDelta.magnitude < 0.01f) {
            // Return to neutral rotation when there's no horizontal movement
            modelTransform.localRotation = Quaternion.Slerp(
                modelTransform.localRotation,
                Quaternion.identity,
                Time.deltaTime / tiltSpeed);
            return;
        }

        Vector3 movementDirection = horizontalDelta.normalized;

        // Calculate tilt direction - forward tilt when accelerating, backward when decelerating
        float tiltDirection = targetSpeed > currentSpeed ? -1f : 1f;

        // Simplify tilt calculation to reach max angle faster
        // Use a percentage of max speed to determine tilt - tiltRate controls how quickly max tilt is reached
        float speedPercentage =
            Mathf.Clamp01(currentSpeed / (minMaxSpeed.y * tiltRate)); // Adjustable threshold for max tilt

        // Calculate tilt amount - simplified to reach maximum much sooner
        float tiltAmount = maxTiltAngle * speedPercentage * tiltDirection;

        // Calculate tilt direction aligned with movement direction
        // First determine the local forward/backward axis for the tilt
        Vector3 forwardDir = transform.forward;
        Vector3 rightDir = transform.right;

        // No need to project onto XZ plane since we've already removed the Y component
        Vector3 movementXZ = movementDirection;

        // Calculate angles for X and Z tilt
        float forwardDot = Vector3.Dot(movementXZ, forwardDir);
        float rightDot = Vector3.Dot(movementXZ, rightDir);

        // X rotation (pitch) - tilts forward/backward
        float pitchAngle = -tiltAmount * forwardDot;

        // Z rotation (roll) - tilts left/right
        float rollAngle = tiltAmount * rightDot;

        // Apply tilt to the model using both pitch and roll
        Quaternion targetRotation = Quaternion.Euler(pitchAngle, 0f, rollAngle);
        modelTransform.localRotation = Quaternion.Slerp(
            modelTransform.localRotation,
            targetRotation,
            Time.deltaTime / tiltSpeed);
    }

    private void HandleMovement() {
        // Set target speed based on input
        // Map input magnitude to speed range: no input = 0, full input = max speed
        float inputMagnitude = Mathf.Clamp01(input.magnitude);
        targetSpeed = Mathf.Lerp(0f, minMaxSpeed.y, inputMagnitude);

        // Choose appropriate lerp rate based on whether we're speeding up or slowing down
        float lerpRate = targetSpeed > currentSpeed
            ? Time.deltaTime / accelerationRate
            : Time.deltaTime / decelerationRate;

        // Smoothly lerp current speed toward target speed
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, lerpRate);

        // Apply movement if we have any speed
        if (currentSpeed > 0.01f) {
            // Calculate movement direction from input
            Vector3 delta = transform.forward * input.z + transform.right * input.x + transform.up * input.y;

            // If no input but still moving, use last movement direction
            if (input.magnitude < 0.01f) {
                delta = deltaPosition.normalized;
            } else {
                // Store current direction for use during deceleration
                deltaPosition = delta;
            }

            // Apply movement with Time.deltaTime for frame-rate independence
            transform.parent.position += delta.normalized * currentSpeed * Time.deltaTime;
        }
    }

    public void OnEnterMovementState() {
        transform.parent.rotation = Quaternion.Euler(0, transform.parent.eulerAngles.y, 0);
    }

    public void OnMove(Vector3 _input) {
        input = _input;
    }

    public void SetLookDirection(Vector3 dir) {
        rotationTarget = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
    }
}