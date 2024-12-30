using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CesiumForUnity;
using Unity.Mathematics;

public class EGM_Sphere : MonoBehaviour
{
    public float waterHeight;

    public GameObject EGM96_Tracer;

    public bool updateUndulation = false;

    public int currentVertIndex = 0;

    MeshFilter meshFilter;

    Vector3[] verts;
    Vector3[] vertDirections;

    private void Start()
    {
        meshFilter = GetComponent<MeshFilter>();

        updateUndulation = true;

        verts = meshFilter.mesh.vertices;
        vertDirections = new Vector3[verts.Length];
    }

    private void Update()
    {
        if (updateUndulation)
        {
            Vector3 worldPt = transform.TransformPoint(meshFilter.mesh.vertices[currentVertIndex]);
            EGM96_Tracer.transform.position = worldPt;
        }
    }
    private void LateUpdate()
    {
        if (updateUndulation)
        {
            EGM96_Tracer.GetComponent<GeoidHeightsDotNet.EGM96_Positioner>().UpdateUndulation();
            double3 posEarth = EGM96_Tracer.GetComponent<CesiumGlobeAnchor>().positionGlobeFixed;
            vertDirections[currentVertIndex] = new Vector3((float)posEarth.x, (float)posEarth.y, (float)posEarth.z).normalized;

            verts[currentVertIndex] = transform.InverseTransformPoint(EGM96_Tracer.transform.position + vertDirections[currentVertIndex] * waterHeight);

            currentVertIndex++;

            if (currentVertIndex >= meshFilter.mesh.vertices.Length)
            {
                meshFilter.mesh.vertices = verts;
                updateUndulation = false;
            }
        }
    }
}
