using CesiumFlood;
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

    public BoatMovement m_BoatMovement;
    public TPS_Player_NEW m_WalkMovement;
    public TPS_CameraController m_CameraController;

    private CF_InputControls controls;
    private UIManager m_UIManager;

    private void Awake() {
        controls = new CF_InputControls();

        controls.Player.SelectMovementType.performed += OnChangeMovementType;

        m_UIManager = UIManager.Instance;
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
    }


    public void OnMove(InputValue value) {

        Vector2 input = value.Get<Vector2>();
        switch (movementType) {
            case MovementType.Walk:
                m_WalkMovement.OnMove(input);
                break;
            case MovementType.JetSki:
                m_BoatMovement.OnMove(input);
                break;
            case MovementType.Drone:
                Debug.Log("Move in Drone mode");
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
                Debug.Log("Jump in Drone mode");
                break;
            default:
                Debug.LogWarning("Unknown movement type for jump: " + movementType);
                break;
        }
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

    void HandleEnterMovementState(MovementType newMovementType, bool force = false) {
        MovementType oldMovementType = movementType;
        if (newMovementType == oldMovementType && !force) {
            // If the new movement type is the same as the old one, do nothing
            return;
        }
        CharacterController cc = GetComponent<CharacterController>();
        // Exit the old movement state if necessary
        // Enter New Movement State
        movementType = newMovementType;

        switch (oldMovementType) {
            case MovementType.Walk:
                m_WalkMovement.gameObject.SetActive(false);

                cc.enabled = false;
                cc.detectCollisions = false;
                break;
            case MovementType.JetSki:
                m_BoatMovement.gameObject.SetActive(false);
                GetComponent<Rigidbody>().isKinematic = true;
                break;
            case MovementType.Drone:
                // Handle exiting drone state if needed
                break;
            default:
                Debug.LogWarning("Unknown old movement type: " + oldMovementType);
                break;
        }

        switch (movementType) {
            case MovementType.Walk:
                m_WalkMovement.gameObject.SetActive(true);
                transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
                cc.enabled = true;
                cc.detectCollisions = true;
                break;
            case MovementType.JetSki:
                m_BoatMovement.gameObject.SetActive(true);
                GetComponent<Rigidbody>().isKinematic = false;
                break;
            case MovementType.Drone:
                // Handle entering drone state if needed
                break;
            default:
                Debug.LogWarning("Unknown movement type: " + oldMovementType);
                break;
        }

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
