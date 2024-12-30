using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatMovement : MonoBehaviour
{
    Rigidbody rb;

    Vector2 move;
    public float moveForce;
    public float jumpForce;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        move.x = Input.GetAxis("Horizontal");
        move.y = Input.GetAxis("Vertical");

        if(Input.GetButtonDown("Jump"))
            rb.AddForce(new Vector3(0, jumpForce, 0) * moveForce, ForceMode.Impulse);
    }

    private void FixedUpdate()
    {
        rb.AddForce(new Vector3(move.x, 0, move.y) * moveForce, ForceMode.Acceleration);
    }
}
