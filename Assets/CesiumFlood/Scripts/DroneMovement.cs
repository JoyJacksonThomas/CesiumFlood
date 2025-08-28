// Adapted from: FreeFlyCamera (Version 1.2) (c) 2019 Sergey Stafeyev	//

using UnityEngine;
using UnityEngine.Serialization;

public class DroneMovement : MonoBehaviour {
    private float currentIncrease = 1;
    private float currentIncreaseMem;

    private Vector3 deltaPosition = Vector3.zero;

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
    [Tooltip("Rate which is applied during camera movement")]
    private float accelerationFactor = 1.01f;

    private float rotationTarget;
    private Vector3 input;


    private void Update() {
        float currentSpeed = movementSpeed;

        // Calc acceleration
        CalculateCurrentIncrease(deltaPosition != Vector3.zero);
        deltaPosition = transform.forward * input.z + transform.right * input.x + transform.up * input.y;
        transform.parent.position += deltaPosition * (currentSpeed * currentIncrease);


        if (!(Mathf.Abs(transform.parent.eulerAngles.y - rotationTarget) < 0.01f)) {
            float newYRot = Mathf.LerpAngle(transform.parent.eulerAngles.y, rotationTarget, 0.5f);
            transform.parent.eulerAngles =
                new Vector3(transform.parent.eulerAngles.x, newYRot, transform.parent.eulerAngles.z);
        }
    }

    public void OnEnterMovementState() {
        transform.parent.rotation = Quaternion.Euler(0, transform.parent.eulerAngles.y, 0);
    }

    public void OnMove(Vector3 _input) {
        input = _input;
    }

    private void CalculateCurrentIncrease(bool moving) {
        //this is all garbage, replace with min/max speed and transition time
        currentIncrease = Time.deltaTime;

        if (!enableAcceleration || (enableAcceleration && !moving)) {
            currentIncreaseMem = 0;
            return;
        }

        currentIncreaseMem += Time.deltaTime * (accelerationFactor - 1);
        currentIncrease = Time.deltaTime + Mathf.Pow(currentIncreaseMem, 3) * Time.deltaTime;
    }

    public void SetLookDirection(Vector3 dir) {
        rotationTarget = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
    }


}