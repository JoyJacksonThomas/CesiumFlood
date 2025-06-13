using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BoatMovement : MonoBehaviour
{
    Rigidbody rb;

    Vector2 move;
    public float moveForce;
    public float jumpForce;
    public float rotationSpeed;
    public float tiltSpeed;

    public Floater Motor, LeftSplashFloater, RightSplashFloater, CenterFloater;

    public Transform[] floaters;
    public Vector3[] restingFloaterPos;
    public Vector3[] movingFloaterPos;
    public float floaterTransitionTime;

    public float maxAngleX, maxAngleZ;

    Vector3 currentAngle;
    Quaternion _relativeTo;

    public AudioSource EngineAudio;
    public float topSpeed;

    public ParticleSystem backSplash, leftSplash, rightSplash, mainFoam, engineFoam, ripples;
    ParticleSystem.RotationOverLifetimeModule rotationOverLifetime;

    public bool motorSubmerged = false;
    public Vector3 particleRestingQuaternion, particleMovingQuaternion;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponentInParent<Rigidbody>();

        _relativeTo = transform.localRotation;

        rotationOverLifetime = backSplash.rotationOverLifetime;
    }

    public void OnJump() {
        // Debug.Log("Jump");
        rb.AddForce(new Vector3(0, jumpForce, 0) * moveForce, ForceMode.Impulse);
    }

    public void OnMove(Vector2 value)
    {
        // Debug.Log("Move");
        move = value;
    }

    // Update is called once per frame
    void Update()
    {
        // move.x = Input.GetAxis("Horizontal");
        // move.y = Input.GetAxis("Vertical");

        Vector3 flatVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        float flatSpeed = flatVelocity.magnitude;
        for (int i = 0; i < floaters.Length; i++)
        {
            floaters[i].localPosition = Vector3.Lerp(restingFloaterPos[i], movingFloaterPos[i], flatSpeed / topSpeed * .25f);
        }
        //if (move.y > 0)
        //{
        //    for(int i = 0; i < floaters.Length; i++)
        //    {
        //        floaters[i].localPosition = Vector3.MoveTowards(floaters[i].localPosition, movingFloaterPos[i], floaterTransitionTime);
        //    }
        //}
        //else
        //{
        //    for (int i = 0; i < floaters.Length; i++)
        //    {
        //        floaters[i].localPosition = Vector3.MoveTowards(floaters[i].localPosition, restingFloaterPos[i], floaterTransitionTime);
        //    }
        //}
        
        EngineAudio.pitch = Mathf.Lerp(1, 2.5f, rb.linearVelocity.magnitude/ topSpeed);

        
        rotationOverLifetime.enabled = true;
        rotationOverLifetime.separateAxes = true;

        rotationOverLifetime.xMultiplier =  move.x *4;
        //backSplash.emission.rateOverTimeMultiplier = Mathf.Lerp(0, 200, rb.velocity.magnitude / topSpeed);
        //backSplash.emissionRate = Mathf.Lerp(0, 200, rb.velocity.magnitude / topSpeed);
        //backSplash.startSpeed = Mathf.Lerp(2f, 0, rb.velocity.magnitude / topSpeed);
        //backSplash.startSize = Mathf.Lerp(1f, 4, rb.velocity.magnitude / topSpeed);
        //backSplash.startLifetime = Mathf.Lerp(.8f, .4f, rb.velocity.magnitude / topSpeed);
        //backSplash.gravityModifier = Mathf.Lerp(.6f, .3f, rb.velocity.magnitude / topSpeed);
        //backSplash.transform.localRotation = Quaternion.Lerp(Quaternion.Euler(particleRestingQuaternion), Quaternion.Euler(particleMovingQuaternion), rb.velocity.magnitude / topSpeed *10 );

        //if (Motor.submerged && move.y > 0)
        //{
        //    if(!backSplash.isPlaying)
        //        backSplash.Play();
        //}
        //else
        //{
        //    if (backSplash.isPlaying)
        //        backSplash.Stop();
        //}

        if ((CenterFloater.submerged || Motor.submerged) && move.y != 0)
        {
            if (!backSplash.isPlaying)
                backSplash.Play();
            if (!mainFoam.isPlaying)
                mainFoam.Play();
            if (!ripples.isPlaying)
                ripples.Play();
        }
        else
        {
            if (backSplash.isPlaying)
                backSplash.Stop();
            if (mainFoam.isPlaying)
                mainFoam.Stop();
            if (ripples.isPlaying)
                ripples.Stop();
        }

        if (Motor.submerged && !engineFoam.isPlaying)
        {
            engineFoam.Play();
        }
        else if (!Motor.submerged && engineFoam.isPlaying)
        {
            engineFoam.Stop();
        }

        //if ((CenterFloater.submerged || Motor.submerged) && !mainFoam.isPlaying && move.y > 0)
        //{
        //    mainFoam.Play();
        //}
        //else if (!CenterFloater.submerged && Motor.submerged && mainFoam.isPlaying)
        //{
        //    mainFoam.Stop();
        //}

        //if (Motor.submerged && !ripples.isPlaying && move.y > 0)
        //{
        //    ripples.Play();
        //}
        //else if (!Motor.submerged && ripples.isPlaying)
        //{
        //    ripples.Stop();
        //}

        //if (LeftSplashFloater.submerged && !leftSplash.isPlaying)
        //{
        //    leftSplash.Play();
        //}
        //else if (!LeftSplashFloater.submerged && leftSplash.isPlaying)
        //{
        //    leftSplash.Stop();
        //}
        //
        //if (RightSplashFloater.submerged && !rightSplash.isPlaying)
        //{
        //    rightSplash.Play();
        //}
        //else if (!RightSplashFloater.submerged && rightSplash.isPlaying)
        //{
        //    rightSplash.Stop();
        //}

        transform.parent.position = transform.position;
    }

    private void LateUpdate()
    {
        motorSubmerged = CenterFloater.submerged;
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

        transform.Rotate(Vector3.up, rotationSpeed * move.x);


        if(Motor.submerged)
            rb.AddForce(flatForward * move.y * moveForce, ForceMode.Acceleration);
        else
            rb.AddForce(flatForward * move.y * moveForce * .3f, ForceMode.Acceleration);


    }

    
}
