using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class AddressRequester : MonoBehaviour
{
    public Vector2 latLong;

    public CesiumForUnity.CesiumGlobeAnchor camAnchor;
    //public CesiumForUnity.CesiumGlobeAnchor waterAnchor;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SearchAddress(string address)
    {
        latLong = GeoCoder.RequestLatLong(address);
        camAnchor.longitudeLatitudeHeight = new double3(latLong.y, latLong.x, 100);
        //waterAnchor.longitudeLatitudeHeight = new double3(latLong.y, latLong.x, 100);
        camAnchor.gameObject.GetComponent<WaterPlaneSpawner>().CenterWaterOnCamera();
    }
}
