using UnityEngine;
using UnityEngine.InputSystem;


public enum MovementType
{
    Walk,
    JetSki,
    Drone
}
[RequireComponent(typeof(PlayerInput))]
public class CF_PlayerController : MonoBehaviour {

    private MovementType movementType = MovementType.JetSki;

    private CF_InputControls controls;

    private void Awake() {
        controls = new CF_InputControls();

        controls.Player.SelectMovementType.performed += OnChangeMovementType;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OnToggleGUI(InputValue value) {

        Debug.Log("Toggle GUI action performed");

    }


    void OnChangeMovementType(InputAction.CallbackContext context) {
        string key = context.control.name;

        switch (key) {
            case "1":
                HandleEnterMovementState(MovementType.Walk);
                break;
            case "2":
                HandleEnterMovementState(MovementType.JetSki);
                break;
            case "3":
                HandleEnterMovementState(MovementType.Drone);
                break;
            default:
                Debug.LogWarning("Unknown movement type key: " + key);
                break;
        }
    }

    void HandleEnterMovementState(MovementType newMovementType) {
        MovementType oldMovementType = movementType;
        // Exit the old movement state if necessary
        // Enter New Movement State
        movementType = newMovementType;

        // Handle entering the new movement state here
        Debug.Log("Entering movement state: " + newMovementType);
        // You can add logic to switch animations, physics, etc. based on the movement type
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }
}
