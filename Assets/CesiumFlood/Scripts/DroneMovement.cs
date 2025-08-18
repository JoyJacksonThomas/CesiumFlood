// Adapted from: FreeFlyCamera (Version 1.2) (c) 2019 Sergey Stafeyev	//

using UnityEngine;

public class DroneMovement : MonoBehaviour {
    private float _currentIncrease = 1;
    private float _currentIncreaseMem;

    private Vector3 _initPosition;
    private Vector3 _initRotation;

    private Vector3 deltaPosition = Vector3.zero;

    private void Start() {
        _initPosition = transform.parent.position;
        _initRotation = transform.parent.eulerAngles;
    }

    private void Update() {
        if (!_active)
            return;

        // // Translation
        // if (_enableTranslation) {
        //     transform.parent.Translate(Vector3.forward * Input.mouseScrollDelta.y * Time.deltaTime * _translationSpeed);
        // }

        // Movement
        if (_enableMovement) {
            float currentSpeed = _movementSpeed;
            //
            // if (Input.GetKey(_boostSpeed))
            //     currentSpeed = _boostedSpeed;
            //
            // if (Input.GetKey(KeyCode.W))
            //     deltaPosition += transform.parent.forward;
            //
            // if (Input.GetKey(KeyCode.S))
            //     deltaPosition -= transform.parent.forward;
            //
            // if (Input.GetKey(KeyCode.A))
            //     deltaPosition -= transform.parent.right;
            //
            // if (Input.GetKey(KeyCode.D))
            //     deltaPosition += transform.parent.right;
            //
            // if (Input.GetKey(_moveUp))
            //     deltaPosition += transform.parent.up;
            //
            // if (Input.GetKey(_moveDown))
            //     deltaPosition -= transform.parent.up;

            // Calc acceleration
            CalculateCurrentIncrease(deltaPosition != Vector3.zero);
            deltaPosition = transform.forward * input.z + transform.right * input.x + transform.up * input.y;
            transform.parent.position += deltaPosition * (currentSpeed * _currentIncrease);
            // deltaPosition = Vector3.zero;
        }


        if (!(Mathf.Abs(transform.parent.eulerAngles.y - rotationTarget) < 0.01f)) {
            float newYRot = Mathf.LerpAngle(transform.parent.eulerAngles.y, rotationTarget, 0.5f);
            transform.parent.eulerAngles =
                new Vector3(transform.parent.eulerAngles.x, newYRot, transform.parent.eulerAngles.z);
        }
    }

#if UNITY_EDITOR
    private void OnValidate() {
        if (_boostedSpeed < _movementSpeed)
            _boostedSpeed = _movementSpeed;
    }
#endif

    public void OnEnterMovementState() {
        _active = true;
        transform.parent.rotation = Quaternion.Euler(0, transform.parent.eulerAngles.y, 0);
    }

    public void OnMove(Vector3 _input) {
        input = _input;
    }

    public void OnLook(Vector2 input) {
        Debug.Log("Drone::OnLook input: " + input);
    }

    private void CalculateCurrentIncrease(bool moving) {
        _currentIncrease = Time.deltaTime;

        if (!_enableSpeedAcceleration || (_enableSpeedAcceleration && !moving)) {
            _currentIncreaseMem = 0;
            return;
        }

        _currentIncreaseMem += Time.deltaTime * (_speedAccelerationFactor - 1);
        _currentIncrease = Time.deltaTime + Mathf.Pow(_currentIncreaseMem, 3) * Time.deltaTime;
    }

    public void SetLookDirection(Vector3 dir) {
        rotationTarget = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;


        // transform.parent.rotation = Quaternion.Euler(0, targetAngle, 0);
    }

    #region UI

    [Space]
    [SerializeField]
    [Tooltip("The script is currently active")]
    private bool _active = true;

    [Space]
    [SerializeField]
    [Tooltip("Camera rotation by mouse movement is active")]
    private bool _enableRotation = true;

    [SerializeField]
    [Tooltip("Sensitivity of mouse rotation")]
    private float _mouseSense = 1.8f;

    [Space]
    [SerializeField]
    [Tooltip("Camera zooming in/out by 'Mouse Scroll Wheel' is active")]
    private bool _enableTranslation = true;

    [SerializeField]
    [Tooltip("Velocity of camera zooming in/out")]
    private float _translationSpeed = 55f;

    [Space]
    [SerializeField]
    [Tooltip("Camera movement by 'W','A','S','D','Q','E' keys is active")]
    private bool _enableMovement = true;

    [SerializeField]
    [Tooltip("Camera movement speed")]
    private float _movementSpeed = 10f;

    [SerializeField]
    [Tooltip("Speed of the quick camera movement when holding the 'Left Shift' key")]
    private float _boostedSpeed = 50f;

    [SerializeField]
    [Tooltip("Boost speed")]
    private KeyCode _boostSpeed = KeyCode.LeftShift;

    [Space]
    [SerializeField]
    [Tooltip("Acceleration at camera movement is active")]
    private bool _enableSpeedAcceleration = true;

    [SerializeField]
    [Tooltip("Rate which is applied during camera movement")]
    private float _speedAccelerationFactor = 1.5f;

    [Space]
    [SerializeField]
    [Tooltip("This keypress will move the camera to initialization position")]
    private KeyCode _initPositonButton = KeyCode.R;

    private float rotationTarget;
    private Vector3 input;

    #endregion UI
}