using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CesiumForUnity;
using Unity.Mathematics;

public class PlaneGenerator : MonoBehaviour
{
    public float waterHeight;

    public Material material;
    public GameObject EGM96_Tracer;

    bool updateUndulation = false;

    Vector3[] vertices = new Vector3[4];
    Vector2[] uv = new Vector2[4];
    int[] triangles = new int[6];
    Vector3[] vertDirections = new Vector3[4];

    GameObject waterPlane;

    int currentVertIndex = 0;

    Transform cameraTrans;

    private void Start()
    {
        cameraTrans = Camera.main.transform;

        vertices[0] = new Vector3(0, 0, 1);
        vertices[1] = new Vector3(1, 0, 1);
        vertices[2] = new Vector3(0, 0, 0);
        vertices[3] = new Vector3(1, 0, 0);

        uv[0] = new Vector2(0, 1);
        uv[1] = new Vector2(1, 1);
        uv[2] = new Vector2(0, 0);
        uv[3] = new Vector2(1, 0);

        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;
        triangles[3] = 2;
        triangles[4] = 1;
        triangles[5] = 3;

        Mesh mesh = new Mesh();

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        waterPlane = new GameObject("Mesh", typeof(MeshFilter), typeof(MeshRenderer), typeof(CesiumGlobeAnchor));
        waterPlane.transform.localScale = new Vector3(1600, 1, 1600);

        waterPlane.GetComponent<MeshFilter>().mesh = mesh;

        waterPlane.GetComponent<MeshRenderer>().material = material;

        waterPlane.transform.parent = transform;
        waterPlane.transform.localPosition = Vector3.zero;
        waterPlane.transform.parent = transform.parent;

        Vector3 worldPt = waterPlane.transform.TransformPoint(vertices[3]);
        EGM96_Tracer.transform.position = worldPt;

        updateUndulation = true;

        waterPlane.name = "WaterPlane";
    }

    private void Update()
    {
        if (updateUndulation)
        {
            Vector3 worldPt = waterPlane.transform.TransformPoint(vertices[currentVertIndex]);
            EGM96_Tracer.transform.position = worldPt;
        }
    }
    private void LateUpdate()
    {
        if(updateUndulation)
        {
            EGM96_Tracer.GetComponent<GeoidHeightsDotNet.EGM96_Positioner>().UpdateUndulation();
            double3 posEarth = EGM96_Tracer.GetComponent<CesiumGlobeAnchor>().positionGlobeFixed;
            vertDirections[currentVertIndex] = new Vector3((float)posEarth.x, (float)posEarth.y, (float)posEarth.z).normalized;

            vertices[currentVertIndex] = waterPlane.transform.InverseTransformPoint(EGM96_Tracer.transform.position + vertDirections[currentVertIndex] * waterHeight);

            currentVertIndex++;
            
            if(currentVertIndex >= vertices.Length)
            {
                waterPlane.GetComponent<MeshFilter>().mesh.vertices = vertices;
                updateUndulation = false;
            }
        }
    }
}
