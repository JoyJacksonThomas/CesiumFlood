using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CesiumForUnity;
using UnityEngine.UI;

public class PlayerInput : MonoBehaviour
{

    public CesiumCameraController camControls;
    public bool hideMouse;
    public WaterSlider waterSlider;

    // Start is called before the first frame update
    void Start()
    {
        //Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        waterSlider.GetComponent<WaterSlider>().UpdateWaterHeight();
        waterSlider.gameObject.SetActive(false);
        waterSlider.transform.parent.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            hideMouse = !hideMouse;
            Cursor.visible = hideMouse;
            camControls.enabled = !hideMouse;
            waterSlider.transform.parent.gameObject.SetActive(hideMouse);
            waterSlider.gameObject.SetActive(hideMouse);
        }

        if(Cursor.visible == true)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                waterSlider.AdjustWaterToPresets(1);
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                waterSlider.AdjustWaterToPresets(-1);
            }
        }
    }
}
