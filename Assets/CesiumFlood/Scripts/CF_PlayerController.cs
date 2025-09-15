using CesiumFlood;
using UnityEngine;
using UnityEngine.InputSystem;

public enum MovementType {
    Walk,
    JetSki,
    Drone
}

[RequireComponent(typeof(PlayerInput))]
public class CF_PlayerController : MonoBehaviour {
    public BoatMovement m_BoatMovement;
    public TPS_Player_NEW m_WalkMovement;
    public TPS_CameraController m_CameraController;
    public DroneMovement m_DroneMovement;

    [SerializeField]
    private MovementType movementType = MovementType.Drone;

    private CF_InputControls controls;

    private CharacterController m_CharacterController;
    private UIManager m_UIManager;

    private void Awake() {
        controls = new CF_InputControls();

        controls.Player.SelectMovementType.performed += OnChangeMovementType;

        m_UIManager = UIManager.Instance;

        m_CharacterController = GetComponent<CharacterController>();
        HandleEnterMovementState(movementType, true);
    }

    private void OnEnable() {
        controls.Enable();
    }

    private void OnDisable() {
        controls.Disable();
    }

    public void OnToggleConfigMenu(InputValue value) {
        m_UIManager.ToggleConfigMenu();
    }

    public void OnToggleMainMenu(InputValue value) {
        m_UIManager.ToggleMainMenu();
    }

    public void OnLook(InputValue value) {
        if (m_UIManager.GetCurrentMenuState() != UIMenuState.None) {
            // If a menu is open, ignore look input
            return;
        }

        Vector2 input = value.Get<Vector2>();
        if (input.magnitude > 100f) {
            // Ignore large input values that may be caused by glitches or unintended input
            return;
        }

        m_CameraController.OnLook(input);
        m_DroneMovement.SetLookDirection(m_CameraController.GetLookDirection());
    }


    public void OnMove(InputValue value) {
        Vector3 input = value.Get<Vector3>();

        Vector2 input2D = new(input.x, input.z);
        switch (movementType) {
            case MovementType.Walk:
                m_WalkMovement.OnMove(input2D);
                break;
            case MovementType.JetSki:
                m_BoatMovement.OnMove(input2D);
                break;
            case MovementType.Drone:
                m_DroneMovement.OnMove(input);
                break;
        }
    }

    public void OnJump(InputValue value) {
        // Handle jump action based on the current movement type
        switch (movementType) {
            case MovementType.Walk:
                Debug.Log("Jump in Walk mode");
                break;
            case MovementType.JetSki:
                m_BoatMovement.OnJump();
                break;
            case MovementType.Drone:
                // drone movement does not have a jump action
                return;
            default:
                Debug.LogWarning("Unknown movement type for jump: " + movementType);
                break;
        }
    }

    public void OnShift(InputValue value) {
        // // Handle shift action based on the current movement type
        // switch (movementType) {
        //     case MovementType.Drone:
        //         m_DroneMovement.OnShift();
        //         break;
        //     default:
        //         return;
        // }
    }

    private void OnChangeMovementType(InputAction.CallbackContext context) {
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

    private void HandleEnterMovementState(MovementType newMovementType, bool force = false) {
        MovementType oldMovementType = movementType;
        if (newMovementType == oldMovementType && !force) {
            // If the new movement type is the same as the old one, do nothing
            return;
        }

        if (force) {
            //disable all movement types
            m_WalkMovement.gameObject.SetActive(false);

            m_CharacterController.enabled = false;
            m_CharacterController.detectCollisions = false;

            m_BoatMovement.gameObject.SetActive(false);
            GetComponent<Rigidbody>().isKinematic = true;
        }

        // Exit the old movement state if necessary
        // Enter New Movement State
        movementType = newMovementType;

        switch (oldMovementType) {
            case MovementType.Walk:
                m_WalkMovement.gameObject.SetActive(false);

                m_CharacterController.enabled = false;
                m_CharacterController.detectCollisions = false;
                break;
            case MovementType.JetSki:
                m_BoatMovement.gameObject.SetActive(false);
                GetComponent<Rigidbody>().isKinematic = true;
                break;
            case MovementType.Drone:
                m_DroneMovement.gameObject.SetActive(false);
                break;
            default:
                Debug.LogWarning("Unknown old movement type: " + oldMovementType);
                break;
        }

        switch (movementType) {
            case MovementType.Walk:
                m_WalkMovement.gameObject.SetActive(true);
                transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
                m_CharacterController.enabled = true;
                m_CharacterController.detectCollisions = true;
                break;
            case MovementType.JetSki:
                m_BoatMovement.gameObject.SetActive(true);
                GetComponent<Rigidbody>().isKinematic = false;
                break;
            case MovementType.Drone:
                m_DroneMovement.gameObject.SetActive(true);
                m_DroneMovement.OnEnterMovementState();
                break;
            default:
                Debug.LogWarning("Unknown movement type: " + oldMovementType);
                break;
        }

        // Handle entering the new movement state here
        Debug.Log("Entering movement state: " + newMovementType);
        // You can add logic to switch animations, physics, etc. based on the movement type
    }
}