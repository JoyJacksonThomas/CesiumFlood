using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using CesiumForUnity;
using System;

public class RadiusAtLatitude : MonoBehaviour
{
    public double Latitude;
    public double radius;

    CesiumGlobeAnchor anchor;
    // Start is called before the first frame update
    void Start()
    {
        anchor = GetComponent<CesiumGlobeAnchor>();
    }

    // Update is called once per frame
    void Update()
    {
        //radius = 6378137 * ( 1 - .00335281092 * (Math.Sin(Latitude) * Math.Sin(Latitude)) );

        /*radius = Math.Sqrt( (Math.Pow((double)6378137 * 6378137 * Math.Cos(Latitude), 2)
                            + Math.Pow((double)6356752 * 6356752 * Math.Sin(Latitude), 2))
                            / (Math.Pow((double)6378137 * Math.Cos(Latitude), 2)
                            + Math.Pow((double)6356752 * Math.Sin(Latitude), 2))
                            );*/

        double mag = Math.Sqrt(anchor.positionGlobeFixed.x * anchor.positionGlobeFixed.x
                                + anchor.positionGlobeFixed.y * anchor.positionGlobeFixed.y
                                + anchor.positionGlobeFixed.z * anchor.positionGlobeFixed.z);

        double3 normalVector = anchor.positionGlobeFixed / mag;
        
        normalVector *= radius;
        
        anchor.positionGlobeFixed = normalVector;
    }
}
