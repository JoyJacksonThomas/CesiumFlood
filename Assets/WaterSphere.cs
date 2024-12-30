using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CesiumForUnity;

public class WaterSphere : MonoBehaviour
{
    public CesiumGlobeAnchor CameraAnchor;
    public float Height;

    CesiumGlobeAnchor anchor;

    // Start is called before the first frame update
    void Start()
    {
        anchor = GetComponent<CesiumGlobeAnchor>();
    }

    // Update is called once per frame
    void Update()
    {
        anchor.longitudeLatitudeHeight = new Unity.Mathematics.double3(CameraAnchor.longitudeLatitudeHeight.x, CameraAnchor.longitudeLatitudeHeight.y, Height);
    }
}
