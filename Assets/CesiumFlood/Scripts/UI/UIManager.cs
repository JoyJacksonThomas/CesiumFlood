using UnityEngine;

namespace CesiumFlood {

    public enum UIMenuState {
        None,
        Config,
        Menu
    }
    public class UIManager : MonoSingleton<UIManager> {
        [SerializeField]
        private GameObject configUI;
        [SerializeField]
        private GameObject menuUI;

        private UIMenuState currentMenuState = UIMenuState.Menu;

        public void ToggleConfigUI() {
            SetState(currentMenuState == UIMenuState.Config ? UIMenuState.None : UIMenuState.Config);
        }

        public void ToggleMenuUI() {
            SetState(currentMenuState == UIMenuState.Menu ? UIMenuState.None : UIMenuState.Menu);
        }

        void SetState(UIMenuState state) {
            currentMenuState = state;
            configUI.SetActive(state == UIMenuState.Config);
            menuUI.SetActive(state == UIMenuState.Menu);
        }


    }
}