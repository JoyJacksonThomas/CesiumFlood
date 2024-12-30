using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct IK_Info
{
    public MovementState MoveState;

    public Transform RayOrigin;
    public float RayLength;
}
public class WalkingIK_Script : MonoBehaviour
{
    public Transform LeftKneeTrans;
    public Transform RightKneeTrans;

    public Transform LeftThighTrans;
    public Transform RightThighTrans;

    public Transform LeftFootTarget;
    public Transform RightFootTarget;

    public Transform HipTrans;

    public float RayDistance;
    public LayerMask Mask;

    public float OffSetY;
    public float OffSetRotationY;

    public Animator Animator;
    public UnityEngine.Animations.Rigging.TwoBoneIKConstraint LeftFootConstraint;
    public UnityEngine.Animations.Rigging.TwoBoneIKConstraint RightFootConstraint;

    public bool LockLeftFoot;
    public bool LockRightFoot;

    Vector3 LockedLeftFootPos;
    Vector3 LockedRightFootPos;

    public TPS_Player_NEW myPlayer;

    public float playerRotationY;

    public Transform AlignTransform;
    public float maxGroundAlignAngleX, maxGroundAlignAngleZ;
    
    public AnimationCurve animCurve;
    public float velocityInfluenceOnAlign;
    public float alignTime;
    public LayerMask groundLayer;
    public float RaySpacing;
    public Vector3 RayCenter;
    [HideInInspector]
    public Vector3 StickDirection;

    public CharacterController characterController;

    public AnimationCurve TurningAnimCurve;
    public float TurningTime;

    public float maxLeanX, maxLeanZ;
    public Vector3 localVelPlusAccel, leaningScale, currentLean = Vector3.zero;
    public AnimationCurve leaningAnimCurve;
    public float leaningTime;
    public Transform leanTransform;
    public float lastYRot = 0;

    public AudioSource footSound;

    Vector3 groundNormal = Vector3.up;
    public float FinalRotationLerpTime;
    public Transform lowerSpineTrans;

    // Start is called before the first frame update
    void Start()
    {
        LeftFootConstraint.weight = 0;
        RightFootConstraint.weight = 0;
    }

    // Update is called once per frame
    //void Update()
    //{
    //    //transform.localEulerAngles = Vector3.zero;
    //}

    private void Update()
    {
        groundNormal = GetGroundData();
        transform.rotation = Quaternion.Lerp(transform.rotation, AlignTransform.rotation * leanTransform.rotation, Time.deltaTime * FinalRotationLerpTime);

        {
            /*
            int layerMask = 1 << 8;

            // This would cast rays only against colliders in layer 8.
            // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
            layerMask = ~layerMask;

            RaycastHit hit;

            Vector3 leftFootRayOrigin = Vector3.zero, rightFootRayOrigin = Vector3.zero, direction = Vector3.zero;
            float leftFootRayDistance = RayDistance, rightFootRayDistance = RayDistance;
            if (myPlayer.movementState == MovementState.WALKING)
            {
                leftFootRayOrigin = LeftKneeTrans.position;
                rightFootRayOrigin = RightKneeTrans.position;

                leftFootRayDistance = RayDistance;
                rightFootRayDistance = RayDistance;
                direction = -transform.up;
            }
            else
            {
                leftFootRayOrigin = LeftThighTrans.position;
                rightFootRayOrigin = RightThighTrans.position;

                leftFootRayDistance = RayDistance * 2;
                rightFootRayDistance = RayDistance * 2;
                direction = -transform.up;
            }

            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(leftFootRayOrigin, (direction), out hit, leftFootRayDistance, Mask) && !LockLeftFoot)
            {
                Debug.DrawRay(leftFootRayOrigin, (direction) * hit.distance, Color.yellow);

                LeftFootTarget.position = leftFootRayOrigin + (direction) * (hit.distance - OffSetY);
                LockedLeftFootPos = LeftFootTarget.position;

                LeftFootTarget.up = hit.normal;
                LeftFootTarget.Rotate(0f, HipTrans.eulerAngles.y + OffSetRotationY, 0f, Space.Self);
            }
            else
            {
                Debug.DrawRay(leftFootRayOrigin, Vector3.down * leftFootRayDistance, Color.white);
            }

            if (Physics.Raycast(rightFootRayOrigin, direction, out hit, rightFootRayDistance, Mask) && !LockRightFoot)
            {
                Debug.DrawRay(rightFootRayOrigin, direction * hit.distance, Color.yellow);

                RightFootTarget.position = rightFootRayOrigin + (direction) * (hit.distance - OffSetY);
                LockedRightFootPos = RightFootTarget.position;

                RightFootTarget.up = hit.normal;
                RightFootTarget.Rotate(0f, HipTrans.eulerAngles.y + OffSetRotationY, 0f, Space.Self);
            }
            else
            {
                Debug.DrawRay(RightKneeTrans.position, (-transform.up) * rightFootRayDistance, Color.white);
            }

            LeftFootConstraint.weight = Animator.GetFloat("IK_LeftFootWeight");
            RightFootConstraint.weight = Animator.GetFloat("IK_RightFootWeight");
            */
        }

        {
            int layerMask = 1 << 8;

            // This would cast rays only against colliders in layer 8.
            // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
            //layerMask = ~layerMask;
            layerMask = groundLayer;

            RaycastHit hit;

            Vector3 leftFootRayOrigin = Vector3.zero, rightFootRayOrigin = Vector3.zero, leftKneeDirection = Vector3.zero, rightKneeDirection = Vector3.zero;
            float leftFootRayDistance = RayDistance, rightFootRayDistance = RayDistance;
            if (myPlayer.movementState == MovementState.WALKING)
            {
                leftFootRayOrigin = LeftKneeTrans.position;
                rightFootRayOrigin = RightKneeTrans.position;

                leftFootRayDistance = RayDistance;
                rightFootRayDistance = RayDistance;
                leftKneeDirection = LeftKneeTrans.up;
                rightKneeDirection = RightKneeTrans.up;
            }
            else
            {
                leftFootRayOrigin = LeftThighTrans.position;
                rightFootRayOrigin = RightThighTrans.position;

                leftFootRayDistance = RayDistance * 2;
                rightFootRayDistance = RayDistance * 2;
                leftKneeDirection = -transform.up;
                rightKneeDirection = -transform.up;

                
            }

            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(leftFootRayOrigin, (leftKneeDirection), out hit, leftFootRayDistance, Mask))
            {
                if(LockLeftFoot == false)
                {
                    Debug.DrawRay(leftFootRayOrigin, (leftKneeDirection) * hit.distance, Color.yellow);

                    LeftFootTarget.position = leftFootRayOrigin + (leftKneeDirection) * (hit.distance - OffSetY);
                    LockedLeftFootPos = LeftFootTarget.position;

                    LeftFootTarget.up = hit.normal;
                    LeftFootTarget.Rotate(0f, HipTrans.eulerAngles.y + OffSetRotationY, 0f, Space.Self);
                    LockLeftFoot = true;

                    footSound.time = 0;
                    footSound.Play();
                }
                LeftFootConstraint.weight = Mathf.Lerp(LeftFootConstraint.weight, 1, .02f);

            }
            else
            {
                Debug.DrawRay(leftFootRayOrigin, leftKneeDirection * leftFootRayDistance, Color.white);
                LockLeftFoot = false;
                //LeftFootConstraint.weight = 0;
                LeftFootConstraint.weight = Mathf.Lerp(LeftFootConstraint.weight, 0, .05f);
            }

            if (Physics.Raycast(rightFootRayOrigin, (rightKneeDirection), out hit, rightFootRayDistance, Mask))
            {
                if (LockRightFoot == false)
                {
                    Debug.DrawRay(rightFootRayOrigin, (rightKneeDirection) * hit.distance, Color.yellow);

                    RightFootTarget.position = rightFootRayOrigin + (rightKneeDirection) * (hit.distance - OffSetY);
                    LockedRightFootPos = RightFootTarget.position;

                    RightFootTarget.up = hit.normal;
                    RightFootTarget.Rotate(0f, HipTrans.eulerAngles.y + OffSetRotationY, 0f, Space.Self);
                    LockRightFoot = true;

                    footSound.time = 0;
                    footSound.Play();
                }
                RightFootConstraint.weight = Mathf.Lerp(RightFootConstraint.weight, 1, .02f);
            }
            else
            {
                Debug.DrawRay(rightFootRayOrigin, rightKneeDirection * rightFootRayDistance, Color.white);
                LockRightFoot = false;
                //RightFootConstraint.weight = 0;
                RightFootConstraint.weight = Mathf.Lerp(RightFootConstraint.weight, 0, .05f);
            }

            {   
                //if (Physics.Raycast(rightFootRayOrigin, rightKneeDirection, out hit, rightFootRayDistance, Mask))
                //{
                //    if(!LockRightFoot)
                //    {
                //        Debug.DrawRay(rightFootRayOrigin, rightKneeDirection * hit.distance, Color.yellow);

                //        RightFootTarget.position = rightFootRayOrigin + (rightKneeDirection) * (hit.distance - OffSetY);
                //        LockedRightFootPos = RightFootTarget.position;

                //        RightFootTarget.up = hit.normal;
                //        RightFootTarget.Rotate(0f, HipTrans.eulerAngles.y + OffSetRotationY, 0f, Space.Self);
                //        LockLeftFoot = true;

                //        RightFootConstraint.weight = 1;
                //    }

                //}
                //else
                //{
                //    Debug.DrawRay(RightKneeTrans.position, rightKneeDirection * rightFootRayDistance, Color.white);
                //    LockLeftFoot = false;
                //    RightFootConstraint.weight = 0;
                //}
            }


            }

        LeftFootTarget.position = LockedLeftFootPos;
        RightFootTarget.position = LockedRightFootPos;
    }

    private void FixedUpdate()
    {
        
        ApplyGroundAlign();
        ApplyLean();
    }

    public void LeftFootPlaced(int placed)
    {
        if (placed == 1)
        {
            LockLeftFoot = true;
            footSound.time = 0;
            footSound.Play();
        }
        else
            LockLeftFoot = false;
    }

    public void RightFootPlaced(int placed)
    {
        if (placed == 1)
        {
            LockRightFoot = true;
            footSound.time = 0;
            footSound.Play();
        }    
        else
            LockRightFoot = false;
    }

    void ApplyGroundAlign()
    {
        Ray ray = new Ray(transform.position, -Vector3.up);
        RaycastHit info = new RaycastHit();
        Quaternion rotationRef = Quaternion.Euler(0, 0, 0);

        if (Physics.Raycast(ray, out info, groundLayer))
        {
            Vector3 localGroundNormal = transform.parent.InverseTransformDirection(groundNormal);
            Debug.DrawRay(transform.position, groundNormal * 10f, Color.yellow);
            Debug.DrawRay(transform.position, localGroundNormal * 10f, Color.green);

            Vector3 clampedAngleNormalXY = ClampVector(new Vector3(groundNormal.x, groundNormal.y, 0f), Vector3.up, maxGroundAlignAngleZ);
            Vector3 clampedAngleNormalZY = ClampVector(new Vector3(0, groundNormal.y, groundNormal.z), Vector3.up, maxGroundAlignAngleX);


            rotationRef = Quaternion.Lerp(AlignTransform.rotation, Quaternion.FromToRotation(Vector3.up, new Vector3(clampedAngleNormalXY.x, clampedAngleNormalXY.y, clampedAngleNormalZY.z)), alignTime);

            AlignTransform.up = rotationRef * Vector3.up;

            AlignTransform.Rotate(0, transform.parent.eulerAngles.y, 0, Space.Self);

            //AlignTransform.position = transform.position;
        }
    }

    void ApplyLean()
    {
        //Rotation Offset Method Static Up
        {
            /*
            localVelPlusAccel = transform.parent.InverseTransformDirection(new Vector3(myPlayer.velocity.x, 0, myPlayer.velocity.z));
            localVelPlusAccel += transform.parent.InverseTransformDirection(new Vector3(myPlayer.acceleration.x, 0, myPlayer.acceleration.z));

            float targetLeanX = Mathf.Clamp(localVelPlusAccel.z * leaningScale.x * Mathf.Rad2Deg, -maxLeanX, maxLeanX);
            float targetLeanZ = Mathf.Clamp(localVelPlusAccel.x * leaningScale.z * Mathf.Rad2Deg, -maxLeanZ, maxLeanZ);

            currentLean.x = Mathf.LerpAngle(currentLean.x, targetLeanX, leaningAnimCurve.Evaluate(leaningTime));
            currentLean.z = Mathf.LerpAngle(currentLean.z, targetLeanZ, leaningAnimCurve.Evaluate(leaningTime));

            currentLean.y = Mathf.LerpAngle(lastYRot - transform.parent.eulerAngles.y, 0, TurningTime);


            transform.rotation = transform.parent.rotation * Quaternion.Euler(currentLean.x, currentLean.y, currentLean.z);

            //transform.rotation = Quaternion.Euler(0, 90, 0);


            lastYRot = transform.eulerAngles.y;
            */
        }

        //Rotation Offset Method Dynamic Up
        {
            localVelPlusAccel = AlignTransform.InverseTransformDirection(new Vector3(myPlayer.velocity.x, myPlayer.velocity.y, myPlayer.velocity.z));
            localVelPlusAccel += AlignTransform.InverseTransformDirection(new Vector3(myPlayer.acceleration.x, myPlayer.velocity.y, myPlayer.acceleration.z));

            float targetLeanX = Mathf.Clamp(localVelPlusAccel.z * leaningScale.x * Mathf.Rad2Deg, -maxLeanX, maxLeanX);
            float targetLeanZ = Mathf.Clamp(localVelPlusAccel.x * leaningScale.z * Mathf.Rad2Deg, -maxLeanZ, maxLeanZ);


            //currentLean.x = Mathf.MoveTowardsAngle(currentLean.x, targetLeanX, leaningTime * Time.deltaTime);
            //currentLean.z = Mathf.MoveTowardsAngle(currentLean.z, targetLeanZ, leaningTime * Time.deltaTime);
            currentLean.x = Mathf.LerpAngle(currentLean.x, targetLeanX, leaningAnimCurve.Evaluate(leaningTime));
            currentLean.z = Mathf.LerpAngle(currentLean.z, targetLeanZ, leaningAnimCurve.Evaluate(leaningTime));

            currentLean.y = Mathf.MoveTowardsAngle(lastYRot - transform.parent.eulerAngles.y, 0, TurningTime);
            //currentLean.y = Mathf.LerpAngle(lastYRot - transform.parent.eulerAngles.y, 0, 1);

            leanTransform.rotation = Quaternion.Euler(currentLean.x, currentLean.y, currentLean.z);

            //transform.rotation = Quaternion.Euler(0, 90, 0);


            lastYRot = transform.eulerAngles.y;
        }
    }

    Vector3 GetGroundData()
    {
        Vector3 avgGroundNormal = Vector3.zero;

        localVelPlusAccel = AlignTransform.InverseTransformDirection(new Vector3(myPlayer.velocity.x, myPlayer.velocity.y, myPlayer.velocity.z));
        localVelPlusAccel += AlignTransform.InverseTransformDirection(new Vector3(myPlayer.acceleration.x, myPlayer.velocity.y, myPlayer.acceleration.z));

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                float rayCenterZ = RayCenter.z * localVelPlusAccel.z* velocityInfluenceOnAlign;
                Vector3 origin = transform.position + transform.right * (RaySpacing * i + RayCenter.x)  + RayCenter.y * transform.up + transform.forward * (RaySpacing * j + rayCenterZ); //new Vector3(RaySpacing * i * transform.right, 0, RaySpacing * j);
                Ray ray = new Ray(origin, -transform.up);
                RaycastHit info = new RaycastHit();
                Quaternion rotationRef = Quaternion.Euler(0, 0, 0);

                if (Physics.Raycast(ray, out info, groundLayer))
                {
                    avgGroundNormal += info.normal;

                    Debug.DrawRay(origin, -transform.up * info.distance, Color.red);
                    Debug.Log(info.collider.gameObject.name);
                }
            }
        }

        return avgGroundNormal / 9;
    }

    public static Quaternion ClampRotation(Quaternion q, Vector3 bounds)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);
        angleX = Mathf.Clamp(angleX, -bounds.x, bounds.x);
        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        float angleY = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.y);
        angleY = Mathf.Clamp(angleY, -bounds.y, bounds.y);
        q.y = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleY);

        float angleZ = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.z);
        angleZ = Mathf.Clamp(angleZ, -bounds.z, bounds.z);
        q.z = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleZ);

        return q;
    }

    Vector3 ClampVector(Vector3 direction, Vector3 center, float maxAngle)
    {


        float angle = Vector3.Angle(center, direction);
        if (angle > maxAngle)
        {

            direction.Normalize();
            center.Normalize();
            Vector3 rotation = (direction - center) / angle;
            return (rotation * maxAngle) + center;

        }

        return direction;

    }

}
