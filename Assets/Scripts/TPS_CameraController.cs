using UnityEngine;

public class TPS_CameraController : MonoBehaviour {
    public Transform target;

    public float sensitivity;

    public float rotationX;
    public float rotationY;
    public float rotationZ;
    public Camera mCamera;

    // float oneOver180 = 1f / 180f;

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

    public float maxDeltaPosition;
    public float maxDeltaRotation;

    private readonly float[] zOffSetQueue = new float[10];
    private int currentQueueIndex;

    private Vector3 originalOffset;
    private Quaternion originalRotation;


    private void Start() {
        originalRotation = transform.localRotation;
        originalOffset = transform.position;
        zOffset = maxOffSet_Z;
        for (int i = 0; i < 10; i++) {
            zOffSetQueue[i] = zOffset;
        }
    }

    private void Update() {
        transform.position = Vector3.MoveTowards(transform.position, target.position + new Vector3(0, 1.8f, 0),
            Time.deltaTime * maxDeltaPosition);

        transform.rotation = Quaternion.Euler(rotationX, rotationY, rotationZ);
    }


    private void FixedUpdate() {
        rotationX = Mathf.Clamp(rotationX, -65, 90);


        RaycastHit hit;

        Vector3 pos = mCamera.transform.localPosition;
        bool raysCollided = false;

        if (Physics.Raycast(mCamera.transform.position, mCamera.transform.TransformDirection(Vector3.back), out hit,
                rayLength, camLayerMask)) {
            zOffset = transform.InverseTransformPoint(hit.point).z + DistanceToWalls;

            Debug.DrawRay(mCamera.transform.position, mCamera.transform.TransformDirection(Vector3.back), Color.blue,
                hit.distance);
            raysCollided = true;
        }

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.back), out hit, -zOffset,
                camLayerMask)) {
            zOffset = transform.InverseTransformPoint(hit.point).z + DistanceToWalls;
            Debug.DrawRay(mCamera.transform.position, mCamera.transform.TransformDirection(Vector3.forward),
                Color.green, -zOffset);
            raysCollided = true;
        }

        if (!raysCollided) {
            zOffset = Mathf.MoveTowards(zOffset, maxOffSet_Z, .7f);
        }

        zOffset = Mathf.Clamp(zOffset, maxOffSet_Z, minOffSet_Z); //max and min inversed bc negative values
        yOffset = Mathf.Clamp(yOffset, minOffSet_Y, maxOffSet_Y);

        zOffSetQueue[currentQueueIndex] = zOffset;
        currentQueueIndex = (currentQueueIndex + 1) % 10;

        float averageOffSet = 0;
        for (int i = 0; i < 10; i++) {
            averageOffSet += zOffSetQueue[i];
        }

        averageOffSet *= .1f; /**/

        pos.z = zOffset;

        mCamera.transform.localPosition = Vector3.MoveTowards(mCamera.transform.localPosition, pos, 1f);
        mCamera.transform.localPosition = pos;
    }

    public void OnLook(Vector2 input) {
        AddRotation(input.y, input.x, 0);
    }

    public void AddRotation(float x, float y, float z) {
        rotationX += x * sensitivity * Time.deltaTime;
        rotationY += y * sensitivity * Time.deltaTime;
        rotationZ += z * sensitivity * Time.deltaTime;
        //Offset.y =  (((int)transform.eulerAngles.x)*oneOver180 * verticalOffsetRange);
        Offset.y = (int)transform.eulerAngles.x;
    }

    public Vector3 GetLookDirection() {
        return mCamera.transform.forward;
    }
}