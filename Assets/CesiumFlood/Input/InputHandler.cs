using UnityEngine;
using UnityEngine.InputSystem;

namespace CesiumFlood{
    public class InputHandler : MonoBehaviour {

        public BoatMovement m_BoatMovement;
        public TPS_CameraController m_CameraController;
        void OnMove(InputValue value) {
            m_BoatMovement.OnMove(value);
        }

        void OnJump(InputValue value) {
            m_BoatMovement.OnJump(value);
        }

        void OnLook(InputValue value) {
            m_CameraController.OnLook(value);
        }

        void OnToggleGUI(InputValue value) {
            Debug.Log("Toggle GUI");
        }
    }
}