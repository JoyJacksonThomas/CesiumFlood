using CesiumFlood;
using CesiumForUnity;
using GeoidHeightsDotNet;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public enum MovementType {
    Walk,
    JetSki,
    Drone
}

[RequireComponent(typeof(PlayerInput))]
public class CF_PlayerController : MonoBehaviour {
    [SerializeField]
    private BoatMovement m_BoatMovement;

    [SerializeField]
    private TPS_Player_NEW m_WalkMovement;

    [SerializeField]
    private TPS_CameraController m_CameraController;

    [SerializeField]
    private DroneMovement m_DroneMovement;

    [SerializeField]
    private CesiumGlobeAnchor m_Anchor;

    [SerializeField]
    private MovementType movementType = MovementType.Drone;

    private CF_InputControls controls;

    private CharacterController m_CharacterController;

    private PlayerInput m_PlayerInput;
    private UIManager m_UIManager;

    private void Awake() {
        m_UIManager = UIManager.Instance;

        m_CharacterController = GetComponent<CharacterController>();

        m_Anchor = GetComponent<CesiumGlobeAnchor>();

        HandleEnterMovementState(movementType, true);

        m_PlayerInput = GetComponent<PlayerInput>();
        m_UIManager.SetPlayerInput(m_PlayerInput);
    }

    private void OnEnable() {
        // controls.Enable();
        // m_PlayerInput.actions.FindActionMap("UI").Enable();
        m_PlayerInput.actions["SelectMovementType"].performed +=
            OnChangeMovementType;
    }

    private void OnDisable() {
        // controls.Disable();
        // m_PlayerInput.actions.FindActionMap("UI").Disable();
        m_PlayerInput.actions["SelectMovementType"].performed -=
            OnChangeMovementType;
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

    public void SetGlobalPosition(Vector2 latLong) {
        double undulation = GeoidHeights.undulation(latLong.x, latLong.y);
        m_Anchor.longitudeLatitudeHeight = new double3(latLong.y, latLong.x, undulation + 10f);
        transform.position = Vector3.zero;
        m_BoatMovement.transform.position = Vector3.zero;
        m_CharacterController.transform.position = Vector3.zero;
        m_DroneMovement.transform.position = Vector3.zero;
    }

    public Vector2 GetGlobalPosition() {
        if (m_Anchor) {
            return new Vector2((float)m_Anchor.longitudeLatitudeHeight.y, (float)m_Anchor.longitudeLatitudeHeight.x);
        }

        return new Vector2(38.8973575f, -77.0327477f);
    }
}