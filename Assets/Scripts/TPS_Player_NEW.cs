using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Rewired.ControllerExtensions;
using Microsoft.Win32;

public enum MovementState
{
    IDLE,
    WALKING
}
public class TPS_Player_NEW : MonoBehaviour
{
    public CharacterController Controller;
    //public WalkingIK_Script WalkingIK;
    public float maxSpeed, currentSpeed;
    
    public Transform MainCamera;
    public TPS_CameraController CameraController;
    public float LookSensitivity;
    float mouseX, mouseY;

    float gravity = -9.81f;
    public float gravityWhileGrounded;
    public float GravityMultiplier;
    float verticalVelocity;
    public bool grounded;

    public int playerId = 0;
    //private Rewired.Player player { get { return Rewired.ReInput.players.GetPlayer(playerId); } }

    public Animator Animator;
    [Range(0f, 1f)]
    public float motionTime;
    AnimatorClipInfo[] m_CurrentClipInfo;

    public MovementState movementState = MovementState.IDLE;

    Vector3 position;

    public Vector3 velocity = Vector3.zero;
    public Vector3 acceleration = Vector3.zero;

    public AnimationCurve stickCurve;

    public LayerMask layerMask;
    Vector3 stickDirection;
    public float turningSpeed;
    public float stopSpeed;

    // Start is called before the first frame update
    void Start()
    {
        position = transform.position;
        m_CurrentClipInfo = Animator.GetCurrentAnimatorClipInfo(0);
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        stickDirection = new Vector3(horizontal, 0, vertical);
        //WalkingIK.StickDirection = stickDirection.normalized;

        //float currentAnimLength = 
        //Animator.SetFloat("MotionTime", Time.timeSinceLevelLoad % );
        Animator.SetFloat("HorizontalMovement", currentSpeed);

        if (stickDirection.magnitude > .1f)
        {

            float stickAngle = Mathf.Atan2(stickDirection.x, stickDirection.z) * Mathf.Rad2Deg + MainCamera.eulerAngles.y;
            float velocityAngle = Mathf.Atan2(velocity.x, velocity.z) * Mathf.Rad2Deg;

            currentSpeed = maxSpeed * stickCurve.Evaluate(stickDirection.magnitude);//Mathf.Lerp(currentSpeed, maxSpeed * stickCurve.Evaluate(stickDirection.magnitude), .5f);

            Vector3 moveDirection = Quaternion.Euler(0, stickAngle, 0) * Vector3.forward;

            Controller.Move(moveDirection.normalized * currentSpeed * Time.deltaTime);

            //

            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, velocityAngle, 0), turningSpeed * Time.deltaTime * 50);

            if (movementState == MovementState.IDLE)
            {
                //Animator.SetFloat("MotionTime", 0f);
                Animator.Play("Idle Walk Run", 0, 0);
            }

            movementState = MovementState.WALKING;
        }
        else
        {
            movementState = MovementState.IDLE;
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, stopSpeed*Time.deltaTime*50);
            
        }

       

        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");

        CameraController.AddRotation(mouseY, mouseX, 0);

        Controller.Move(Vector3.up * verticalVelocity * Time.deltaTime);

        UpdateMotionData(Time.deltaTime);
    }

    void FixedUpdate()
    {

        


        ApplyGravity();

    }

    void UpdateMotionData(float deltaTime)
    {
        acceleration = (transform.position - position) * (1/ deltaTime) - velocity;
        velocity = (transform.position - position) * (1 / deltaTime);
        position = transform.position;
    }

    void ApplyGravity()
    {
        if(grounded)
        {
            verticalVelocity = gravityWhileGrounded;
        }
        else
        {
            verticalVelocity += gravity * GravityMultiplier * Time.deltaTime;
        }

       
    }

    private void OnCollisionStay(Collision col)
    {
        grounded = true;
    }
    private void OnCollisionExit(Collision col)
    {
        grounded = false;
    }

}
