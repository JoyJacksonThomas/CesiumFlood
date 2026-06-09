using CesiumForUnity;
using GeoidHeightsDotNet;
using Unity.Mathematics;
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

        double longitude = waterPlaneAnchor.longitudeLatitudeHeight.x;
        double latitude = waterPlaneAnchor.longitudeLatitudeHeight.y;
        double height = waterHeightOffset + CalcUndulation(latitude, longitude);
        waterPlaneAnchor.longitudeLatitudeHeight = new double3(longitude, latitude, height);
    }

    private static double CalcUndulation(double latitude, double longitude) {
        return GeoidHeights.undulation(latitude, longitude);
    }

    public void UpdateWaterLevel() {
        if (waterPlaneAnchor != null) {
            waterPlaneAnchor.Sync();
        }

        SetWaterLevel(waterLevel);
    }

    public float GetWaterPlaneHeight() {
        return waterPlaneAnchor.transform.position.y;
    }
}