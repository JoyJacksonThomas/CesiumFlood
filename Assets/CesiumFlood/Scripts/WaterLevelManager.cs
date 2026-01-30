using CesiumForUnity;
using GeoidHeightsDotNet;
using UnityEngine;

public class WaterLevelManager : MonoSingleton<WaterLevelManager> {
    public float waterLevel;

    [SerializeField]
    private CesiumGlobeAnchor waterPlaneAnchor;

    public Material[] materialsWithWaterLevel;


    public void SetWaterLevel(float waterHeightOffset) {
        waterLevel = waterHeightOffset;
        for (int i = 0; i < materialsWithWaterLevel.Length; i++) {
            materialsWithWaterLevel[i].SetFloat("_WaterLevel", waterLevel);
        }

        double latitude = waterPlaneAnchor.longitudeLatitudeHeight.y;
        double longitude = waterPlaneAnchor.longitudeLatitudeHeight.x;
        waterPlaneAnchor.height = waterHeightOffset + CalcUndulation(latitude, longitude);
    }

    private static double CalcUndulation(double latitude, double longitude) {
        return GeoidHeights.undulation(latitude, longitude);
    }

    public void UpdateWaterLevel() {
        SetWaterLevel(waterLevel);
    }

    public float GetWaterPlaneHeight() {
        return waterPlaneAnchor.transform.position.y;
    }
}