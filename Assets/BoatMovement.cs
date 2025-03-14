using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatMovement : MonoBehaviour
{
    Rigidbody rb;

    Vector2 move;
    public float moveForce;
    public float jumpForce;
    public float rotationSpeed;
    public float tiltSpeed;

    public Floater Motor;

    public Transform[] floaters;
    public Vector3[] restingFloaterPos;
    public Vector3[] movingFloaterPos;
    public float floaterTransitionTime;

    public float maxAngleX, maxAngleZ;

    Vector3 currentAngle;
    Quaternion _relativeTo;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        _relativeTo = transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        move.x = Input.GetAxis("Horizontal");
        move.y = Input.GetAxis("Vertical");

        if(move.y > 0)
        {
            for(int i = 0; i < floaters.Length; i++)
            {
                floaters[i].localPosition = Vector3.MoveTowards(floaters[i].localPosition, movingFloaterPos[i], floaterTransitionTime);
            }
        }
        else
        {
            for (int i = 0; i < floaters.Length; i++)
            {
                floaters[i].localPosition = Vector3.MoveTowards(floaters[i].localPosition, restingFloaterPos[i], floaterTransitionTime);
            }
        }

        if(Input.GetButtonDown("Jump"))
            rb.AddForce(new Vector3(0, jumpForce, 0) * moveForce, ForceMode.Impulse);
    }

    private void FixedUpdate()
    {
        Vector3 flatForward = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;

        float x = transform.localEulerAngles.x > 180 ? transform.localEulerAngles.x-360 : transform.localEulerAngles.x;
        x = Mathf.Clamp(x, -maxAngleX, maxAngleX);

        float z = transform.localEulerAngles.z > 180 ? transform.localEulerAngles.z - 360 : transform.localEulerAngles.z;
        z -= move.x * tiltSpeed;
        z = Mathf.Clamp(z, -maxAngleZ, maxAngleZ);


        transform.localEulerAngles = new Vector3(x, transform.localEulerAngles.y, z);

        transform.RotateAroundLocal(Vector3.up, rotationSpeed * move.x);


        if(Motor.submerged)
            rb.AddForce(flatForward * move.y * moveForce, ForceMode.Acceleration);
        else
            rb.AddForce(flatForward * move.y * moveForce * .3f, ForceMode.Acceleration);
    }

    
}
