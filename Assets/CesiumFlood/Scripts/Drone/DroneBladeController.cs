using UnityEngine;

public class DroneBladeController : MonoBehaviour {
    [SerializeField]
    private GameObject RotorFL;

    [SerializeField]
    private GameObject RotorFR;

    [SerializeField]
    private GameObject RotorBL;

    [SerializeField]
    private GameObject RotorBR;


    [SerializeField]
    private float Speed = 2f;

    private void Update() {
        float angle = -60f * Speed * Time.deltaTime;
        //cw
        RotorFL.transform.Rotate(Vector3.up, angle);
        RotorBR.transform.Rotate(Vector3.up, angle);
        //ccw , these have negative scale so they invert automatically
        RotorFR.transform.Rotate(Vector3.up, angle);
        RotorBL.transform.Rotate(Vector3.up, angle);
    }
}