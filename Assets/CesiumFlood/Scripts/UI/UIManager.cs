using UnityEngine;
using UnityEngine.InputSystem;

namespace CesiumFlood {
    public enum UIMenuState {
        None,
        Config,
        Menu
    }

    [RequireComponent(typeof(AddressRequester))]
    public class UIManager : MonoSingleton<UIManager> {
        [SerializeField]
        private GameObject configMenu;

        [SerializeField]
        private GameObject mainMenu;

        [SerializeField]
        private GameObject currentAddressDisplay;


        private AddressRequester addressRequester;

        private UIMenuState currentMenuState = UIMenuState.Menu;

        private PlayerInput m_PlayerInput;

        private void Start() {
            addressRequester = GetComponent<AddressRequester>();
            SetState(UIMenuState.Menu);
        }


        public UIMenuState GetCurrentMenuState() {
            return currentMenuState;
        }


        public void SetPlayerInput(PlayerInput playerInput) {
            m_PlayerInput = playerInput;
        }

        public void ToggleConfigMenu() {
            SetState(currentMenuState == UIMenuState.Config ? UIMenuState.None : UIMenuState.Config);
        }

        public void ToggleMainMenu() {
            SetState(currentMenuState == UIMenuState.Menu ? UIMenuState.None : UIMenuState.Menu);
        }

        private void SetState(UIMenuState state) {
            currentMenuState = state;
            configMenu.SetActive(state == UIMenuState.Config);
            mainMenu.SetActive(state == UIMenuState.Menu);
            currentAddressDisplay.SetActive(state == UIMenuState.None || state == UIMenuState.Config);


            if (state == UIMenuState.None) {
                // If no menu is open, ensure the cursor is hidden and locked
                m_PlayerInput.actions.FindActionMap("Player").Enable();
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            } else {
                // If a menu is open, show the cursor and unlock it
                m_PlayerInput.actions.FindActionMap("Player").Disable();
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        public void QuitApp() {
            Application.Quit();
        }

        public AddressRequester GetAddressRequester() {
            if (!addressRequester) {
                Debug.LogError("AddressRequester not found");
            }

            return addressRequester;
        }
    }
}