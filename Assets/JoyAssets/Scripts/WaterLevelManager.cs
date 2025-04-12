using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterLevelManager : MonoBehaviour
{
    [HideInInspector]
    public static WaterLevelManager instance;

    public float waterLevel;
    public Material[] materialsWithWaterLevel;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Update()
    {
        for (int i = 0; i < materialsWithWaterLevel.Length; i++)
        {
            materialsWithWaterLevel[i].SetFloat("_WaterLevel", waterLevel);
        }
    }
}
