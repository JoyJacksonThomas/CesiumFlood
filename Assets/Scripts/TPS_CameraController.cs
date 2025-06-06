#region Defines
#if UNITY_2020 || UNITY_2021 || UNITY_2022 || UNITY_2023 || UNITY_2024 || UNITY_2025
#define UNITY_2020_PLUS
#endif
#if UNITY_2019 || UNITY_2020_PLUS
#define UNITY_2019_PLUS
#endif
#if UNITY_2018 || UNITY_2019_PLUS
#define UNITY_2018_PLUS
#endif
#if UNITY_2017 || UNITY_2018_PLUS
#define UNITY_2017_PLUS
#endif
#if UNITY_5 || UNITY_2017_PLUS
#define UNITY_5_PLUS
#endif
#if UNITY_5_1 || UNITY_5_2 || UNITY_5_3_OR_NEWER || UNITY_2017_PLUS
#define UNITY_5_1_PLUS
#endif
#if UNITY_5_2 || UNITY_5_3_OR_NEWER || UNITY_2017_PLUS
#define UNITY_5_2_PLUS
#endif
#if UNITY_5_3_OR_NEWER || UNITY_2017_PLUS
#define UNITY_5_3_PLUS
#endif
#if UNITY_5_4_OR_NEWER || UNITY_2017_PLUS
#define UNITY_5_4_PLUS
#endif
#if UNITY_5_5_OR_NEWER || UNITY_2017_PLUS
#define UNITY_5_5_PLUS
#endif
#if UNITY_5_6_OR_NEWER || UNITY_2017_PLUS
#define UNITY_5_6_PLUS
#endif
#if UNITY_5_7_OR_NEWER || UNITY_2017_PLUS
#define UNITY_5_7_PLUS
#endif
#if UNITY_5_8_OR_NEWER || UNITY_2017_PLUS
#define UNITY_5_8_PLUS
#endif
#if UNITY_5_9_OR_NEWER || UNITY_2017_PLUS
#define UNITY_5_9_PLUS
#endif
#pragma warning disable 0219
#pragma warning disable 0618
#pragma warning disable 0649
#pragma warning disable 0067
#endregion


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class TPS_CameraController : MonoBehaviour
{
    public Transform target;

    public float sensitivity;

    public float rotationX = 0f;
    public float rotationY = 0f;
    public float rotationZ = 0f;
    Quaternion originalRotation;
    public Camera mCamera;

    float oneOver180 = 1f / 180f;

    public Vector3 Offset = Vector3.zero;

    public float verticalOffsetRange;

    public LayerMask camLayerMask;
    public float rayLength;

    public float maxOffSet_Z;
    public float minOffSet_Z;
    public float zOffset;

    public float maxOffSet_Y;
    public float minOffSet_Y;
    public float yOffset;

    public float DistanceToWalls;

    float[] zOffSetQueue = new float[10];
    int currentQueueIndex = 0;

    public float maxDeltaPosition;
    public float maxDeltaRotation;


    void Start()
    {


        originalRotation = transform.localRotation;
        zOffset = maxOffSet_Z;
        for (int i = 0; i < 10; i++)
        {
            zOffSetQueue[i] = zOffset;
        }
    }

    public void OnLook(InputValue value){
        Vector2 input = value.Get<Vector2>();
        AddRotation(input.y, input.x, 0);
    }
    void Update()
    {
        Cursor.visible = false;
        transform.position = Vector3.MoveTowards(transform.position, target.position + new Vector3(0, 1.8f, 0), Time.deltaTime * maxDeltaPosition);

        transform.rotation = Quaternion.Euler(rotationX, rotationY, rotationZ);

        //transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(rotationX, rotationY, rotationZ), Time.deltaTime * maxDeltaRotation);
        //
        // float mouseX = Input.GetAxis("Mouse X");
        // float mouseY = Input.GetAxis("Mouse Y");

        // AddRotation(mouseY, mouseX, 0);
    }



    void FixedUpdate()
    {
        rotationX = Mathf.Clamp(rotationX, -65, 90);
        

        RaycastHit hit;

        Vector3 pos = mCamera.transform.localPosition;
        bool raysCollided = false;

        if (Physics.Raycast(mCamera.transform.position, mCamera.transform.TransformDirection(Vector3.back), out hit, rayLength, camLayerMask))
        {
            zOffset = transform.InverseTransformPoint(hit.point).z + DistanceToWalls;

            Debug.DrawRay(mCamera.transform.position, mCamera.transform.TransformDirection(Vector3.back), Color.blue, hit.distance);
            raysCollided = true;
        }
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.back), out hit, -zOffset, camLayerMask))
        {
            zOffset = transform.InverseTransformPoint(hit.point).z + DistanceToWalls;
            Debug.DrawRay(mCamera.transform.position, mCamera.transform.TransformDirection(Vector3.forward), Color.green, -zOffset);
            raysCollided = true;
        }

        if (raysCollided == false)
        {
            zOffset = Mathf.MoveTowards(zOffset, maxOffSet_Z, .7f);
        }

        zOffset = Mathf.Clamp(zOffset, maxOffSet_Z, minOffSet_Z); //max and min inversed bc negative values
        yOffset = Mathf.Clamp(yOffset, minOffSet_Y, maxOffSet_Y);

        zOffSetQueue[currentQueueIndex] = zOffset;
        currentQueueIndex = (currentQueueIndex + 1) % 10;

        float averageOffSet = 0;
        for (int i = 0; i < 10; i++)
        {
            averageOffSet += zOffSetQueue[i];
        }
        averageOffSet *= .1f;/**/

        pos.z = zOffset;

        mCamera.transform.localPosition = Vector3.MoveTowards(mCamera.transform.localPosition, pos, 1f);
        mCamera.transform.localPosition = pos;

    }

    public void AddRotation(float x, float y, float z)
    {
        rotationX += x * sensitivity * Time.deltaTime;
        rotationY += y * sensitivity * Time.deltaTime;
        rotationZ += z * sensitivity * Time.deltaTime;
        //Offset.y =  (((int)transform.eulerAngles.x)*oneOver180 * verticalOffsetRange);
        Offset.y = (int)transform.eulerAngles.x;
    }



}