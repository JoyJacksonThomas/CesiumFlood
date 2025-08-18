using CesiumForUnity;
using UnityEngine;

public class CF_PlayerInput : MonoBehaviour {
    public CesiumCameraController camControls;
    public bool hideMouse;
    public WaterSlider waterSlider;
    public DroneMovement otherCamControls;

    // Start is called before the first frame update
    private void Start() {
        //Cursor.lockState = CursorLockMode.Confined;
        // Cursor.visible = false;
        // Cursor.lockState = CursorLockMode.Confined;
        waterSlider.GetComponent<WaterSlider>().UpdateWaterHeight();
        waterSlider.gameObject.SetActive(false);
        waterSlider.transform.parent.gameObject.SetActive(false);
    }

    // Update is called once per frame
    private void Update() {
        if (Input.GetKeyDown(KeyCode.LeftShift)) {
            hideMouse = !hideMouse;
            //Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = hideMouse;
            camControls.enabled = !hideMouse;
            otherCamControls.enabled = !hideMouse;
            waterSlider.transform.parent.gameObject.SetActive(hideMouse);
            waterSlider.gameObject.SetActive(hideMouse);
        }

        if (Cursor.visible) {
            if (Input.GetKeyDown(KeyCode.UpArrow)) {
                waterSlider.AdjustWaterToPresets(1);
            } else if (Input.GetKeyDown(KeyCode.DownArrow)) {
                waterSlider.AdjustWaterToPresets(-1);
            }
        }
    }
}