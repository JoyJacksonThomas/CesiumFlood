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
    public bool submerged;
    

    public float WaveScale, WaveSpeed1, WaveSpeed2, WaveSpeed3, Amplitude1, Amplitude2, Amplitude3, Frequency1, Frequency2, Frequency3;

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

        if(floaters != 0)
            rb.AddForceAtPosition(Physics.gravity / floaters, transform.position, ForceMode.Acceleration);

        //float waterHeight = instance.GetHeight(transform.position) ?? transform.position.y;
        float waterHeight = GetWaterHeight();

        submerged = transform.position.y < WaterLevelManager.Instance.waterLevel + waterHeight;

        if (transform.position.y < WaterLevelManager.Instance.waterLevel + waterHeight)
        {
            
            float displacementMulti = Mathf.Clamp01((WaterLevelManager.Instance.waterLevel + waterHeight - transform.position.y) / depthBefSub) * displacementAmt;

            rb.AddForceAtPosition(new Vector3(0f, Mathf.Abs(Physics.gravity.y) * displacementMulti, 0f), transform.position, ForceMode.Acceleration);

            rb.AddForce(displacementMulti * -rb.linearVelocity * waterDrag * Time.fixedDeltaTime, ForceMode.VelocityChange);

            rb.AddTorque(displacementMulti * -rb.angularVelocity * waterAngularDrag * Time.fixedDeltaTime, ForceMode.VelocityChange);
        }
    }

    public float GetWaterHeight()
    {
        float height = 0;

        float posX = Mathf.Abs(transform.position.x);
        float posZ = Mathf.Abs(transform.position.z);

        float heightX = Amplitude1 * Mathf.Sin(posX * Frequency1 + Time.time * WaveSpeed1)
                            + Amplitude2 * Mathf.Sin(posX * Frequency2 + Time.time * WaveSpeed2)
                            + Amplitude3 * Mathf.Sin(posX * Frequency3 + Time.time * WaveSpeed3);

        float heightZ = Amplitude1 * Mathf.Sin(posZ * Frequency1 + Time.time * WaveSpeed1)
                            + Amplitude2 * Mathf.Sin(posZ * Frequency2 + Time.time * WaveSpeed2)
                            + Amplitude3 * Mathf.Sin(posZ * Frequency3 + Time.time * WaveSpeed3);


        

        height = WaveScale * (heightX + heightZ);


        return height;
    }

    private void OnDrawGizmos()
    {
        // water level instance doesn't exist before start
        if (!Application.isPlaying) {
            return;
        }
        if (WaterLevelManager.Instance == null)
            return;
        Gizmos.DrawCube(new Vector3(transform.position.x, WaterLevelManager.Instance.waterLevel + GetWaterHeight(), transform.position.z), Vector3.one *.3f);

    }
}
