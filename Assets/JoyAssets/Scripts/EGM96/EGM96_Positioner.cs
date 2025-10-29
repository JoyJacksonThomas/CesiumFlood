using System.Collections;
using CesiumForUnity;
using Unity.Mathematics;
using UnityEngine;

namespace GeoidHeightsDotNet {
    public class EGM96_Positioner : MonoBehaviour {
        public bool updatePosition;


        private CesiumGlobeAnchor anchor;

        private float undulationOffset;

        // Start is called before the first frame update
        private void Start() {
            anchor = GetComponent<CesiumGlobeAnchor>();
            UpdatePosition();
            if (updatePosition) {
                StartCoroutine(UpdatePosition());
            }
        }

        // Update is called once per frame
        private void Update() { }

        public double UpdateUndulation() {
            double latitude = anchor.longitudeLatitudeHeight.y;
            double longitude = anchor.longitudeLatitudeHeight.x;
            double und = GeoidHeights.undulation(latitude, longitude);
            anchor.longitudeLatitudeHeight = new double3(longitude, latitude, und);
            return und;
        }

        private IEnumerator UpdatePosition() {
            while (updatePosition) {
                yield return new WaitForSeconds(1f);
                undulationOffset = WaterLevelManager.Instance.waterLevel;
                anchor.height = UpdateUndulation() + undulationOffset;
                Debug.Log("position updated");
            }
        }
    }
}