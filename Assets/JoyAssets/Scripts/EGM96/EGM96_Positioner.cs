using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CesiumForUnity;

namespace GeoidHeightsDotNet
{
    public class EGM96_Positioner : MonoBehaviour
    {


        CesiumGlobeAnchor anchor;
        public bool updatePosition = false;
        float undulationOffset;
        // Start is called before the first frame update
        void Start()
        {
            anchor = GetComponent<CesiumGlobeAnchor>();

            if (updatePosition)
            {
                StartCoroutine(UpdatePosition());
            }
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

        IEnumerator UpdatePosition()
        {
            while (updatePosition)
            {
                yield return new WaitForSeconds(5f);
                undulationOffset = WaterLevelManager.Instance.waterLevel;
                anchor.height = UpdateUndulation() + undulationOffset;
                Debug.Log("position updated");
            }
        }

    }
}
