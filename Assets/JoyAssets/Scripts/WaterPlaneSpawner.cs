using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CesiumForUnity;

public class WaterPlaneSpawner : MonoBehaviour
{
    public Transform GeoReference;
    public GameObject EGM_Tracer;
    public GameObject WaterPrefab;
    public float WaterScale = 4000000;
    Transform cameraTransform;
    CesiumGlobeAnchor cameraAnchor;

    public EGM_Plane[] waterPools;
    public GameObject[] waterObjects;
    int currentPool = 0;
    public float WaterHeight;
    // Start is called before the first frame update
    void Start()
    {
        cameraTransform = Camera.main.transform;
        cameraAnchor = Camera.main.GetComponent<CesiumGlobeAnchor>();

        waterObjects = new GameObject[9];
        waterPools = new EGM_Plane[9];

        for (int i = 0; i < 9; i++)
        {
            int row = i / 3;
            int col = i % 3;
        
            float scale = WaterScale;
            
            GameObject water = Instantiate(WaterPrefab, cameraTransform.position + new Vector3(-scale + row * scale, 0, -scale + col * scale), 
                                            Quaternion.Euler(-90, 0, 0));
            
            water.GetComponent<EGM_Plane>().EGM96_Tracer = EGM_Tracer;
            water.transform.parent = GeoReference;

            waterObjects[i] = water;
            waterPools[i] = water.GetComponent<EGM_Plane>();
        }

        waterPools[currentPool].updateUndulation = true;
    }

    // Update is called once per frame
    void Update()
    {
        
        if ( currentPool < waterPools.Length && waterPools[currentPool].updateUndulation == false)
        {
            currentPool++;
            if(currentPool < waterPools.Length)
                waterPools[currentPool].updateUndulation = true;
        }

        for (int i = 0; i < 9; i++)
        {
            waterPools[i].waterHeight = WaterHeight;
        }
    }

    public void CenterWaterOnCamera()
    {
        for (int i = 0; i < 9; i++)
        {
            int row = i / 3;
            int col = i % 3;

            float scale = 80000;

            waterObjects[i].transform.position = cameraTransform.position + new Vector3(-scale + row * scale, 0, -scale + col * scale);
            waterObjects[i].transform.rotation = Quaternion.Euler(-90, 0, 0);
        }

        currentPool = 0;
        waterPools[currentPool].updateUndulation = true;
    }

}
