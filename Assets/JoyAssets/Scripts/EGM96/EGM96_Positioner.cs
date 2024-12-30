using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CesiumForUnity;

namespace GeoidHeightsDotNet
{
    public class EGM96_Positioner : MonoBehaviour
    {


        CesiumGlobeAnchor anchor;
        // Start is called before the first frame update
        void Start()
        {
            anchor = GetComponent<CesiumGlobeAnchor>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public double UpdateUndulation()
        {
            double latitude = anchor.longitudeLatitudeHeight.y;
            double longitude = anchor.longitudeLatitudeHeight.x;
            double und = GeoidHeights.undulation(latitude, longitude);
            anchor.longitudeLatitudeHeight = new Unity.Mathematics.double3(longitude, latitude, und);
            return und;
        }
    }
}
