using CesiumForUnity;
using Unity.Mathematics;
using UnityEngine;

public class EGM_Plane : MonoBehaviour {
    public float waterHeight;

    public GameObject EGM96_Tracer;

    public bool updateUndulation;

    public int currentVertIndex;

    public bool foundCenter;

    private CesiumGlobeAnchor globeAnchor;

    private MeshFilter meshFilter;
    private double undulation;
    private Vector3[] vertDirections;

    private Vector3[] verts;

    private void Start() {
        meshFilter = GetComponent<MeshFilter>();

        globeAnchor = GetComponent<CesiumGlobeAnchor>();

        verts = meshFilter.mesh.vertices;
        vertDirections = new Vector3[verts.Length];
    }

    private void Update() {
        if (updateUndulation) {
            Vector3 worldPt;
            if (foundCenter)
                worldPt = transform.TransformPoint(meshFilter.mesh.vertices[currentVertIndex]);
            else
                worldPt = transform.position;

            EGM96_Tracer.transform.position = worldPt;
        }
    }

    private void LateUpdate() {
        if (updateUndulation) {
            GetComponent<CesiumGlobeAnchor>().rotationEastUpNorth = Quaternion.Euler(-90, 0, 0);
            // EGM96_Tracer.GetComponent<GeoidHeightsDotNet.EGM96_Positioner>().CalcUndulation();

            if (!foundCenter) {
                undulation = EGM96_Tracer.GetComponent<CesiumGlobeAnchor>().longitudeLatitudeHeight.z;
                transform.position = EGM96_Tracer.transform.position;
                foundCenter = true;
                return;
            }

            double3 posEarth = EGM96_Tracer.GetComponent<CesiumGlobeAnchor>().positionGlobeFixed;
            vertDirections[currentVertIndex] =
                new Vector3((float)posEarth.x, (float)posEarth.y, (float)posEarth.z).normalized;

            verts[currentVertIndex] =
                transform.InverseTransformPoint(EGM96_Tracer.transform.position + Vector3.up * waterHeight);

            currentVertIndex++;

            if (currentVertIndex >= meshFilter.mesh.vertices.Length) {
                meshFilter.mesh.vertices = verts;
                updateUndulation = false;
                currentVertIndex = 0;
                foundCenter = false;
            }
        } else {
            globeAnchor.longitudeLatitudeHeight = new double3(globeAnchor.longitudeLatitudeHeight.x,
                globeAnchor.longitudeLatitudeHeight.y, undulation + waterHeight);
        }
    }
}