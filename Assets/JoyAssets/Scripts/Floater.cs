using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floater : MonoBehaviour
{
    public Rigidbody rb;
    public float depthBefSub;
    public float displacementAmt;
    public int floaters;
    public float waterDrag;
    public float waterAngularDrag;
    public LayerMask layerMask;
    public Transform waterTransform;

    public Bitgem.VFX.StylisedWater.WaterVolumeHelper WaterVolumeHelper = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void FixedUpdate()
    {
        var instance = WaterVolumeHelper ? WaterVolumeHelper : Bitgem.VFX.StylisedWater.WaterVolumeHelper.Instance;
        if (!instance)
        {
            return;
        }

        rb.AddForceAtPosition(Physics.gravity / floaters, transform.position, ForceMode.Acceleration);

        float waterHeight = instance.GetHeight(transform.position) ?? transform.position.y;

        if (transform.position.y < waterHeight)
        {
            
            float displacementMulti = Mathf.Clamp01((waterHeight - transform.position.y) / depthBefSub) * displacementAmt;

            rb.AddForceAtPosition(new Vector3(0f, Mathf.Abs(Physics.gravity.y) * displacementMulti, 0f), transform.position, ForceMode.Acceleration);

            rb.AddForce(displacementMulti * -rb.velocity * waterDrag * Time.fixedDeltaTime, ForceMode.VelocityChange);

            rb.AddTorque(displacementMulti * -rb.angularVelocity * waterAngularDrag * Time.fixedDeltaTime, ForceMode.VelocityChange);
        }
    }
}
