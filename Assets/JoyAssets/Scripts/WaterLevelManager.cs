using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterLevelManager : MonoSingleton<WaterLevelManager>
{

    public float waterLevel;
    public Material[] materialsWithWaterLevel;

    private void Update()
    {
        for (int i = 0; i < materialsWithWaterLevel.Length; i++)
        {
            materialsWithWaterLevel[i].SetFloat("_WaterLevel", waterLevel);
        }
    }
}
