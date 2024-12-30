using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterTextureGenerator : MonoBehaviour
{
    public int TextureWidth = 10, TextureHeight = 10;
    public LayerMask layerMask;
    public Material mat;

    Mesh Mesh;
    public Texture2D WaterMask;
    Vector3[] EdgePositions;

    [Range(0, 1)]
    public float testX;
    [Range(0, 1)]
    public float testY;

    // Start is called before the first frame update
    void Start()
    {
        WaterMask = new Texture2D(TextureWidth, TextureHeight);
        GetComponent<Renderer>().material = mat;
        GetComponent<Renderer>().material.mainTexture = WaterMask;
        Mesh = GetComponent<MeshFilter>().mesh;

        EdgePositions = new Vector3[4];
        EdgePositions[0] = Mesh.vertices[2];
        EdgePositions[1] = Mesh.vertices[11];
        EdgePositions[2] = Mesh.vertices[24];
        EdgePositions[3] = Mesh.vertices[14];

        GenerateTexture();
    }

    void GenerateTexture()
    {
        for(int i = 0; i < TextureWidth; i++)
        {
            for(int j = 0; j < TextureHeight; j++)
            {
                Vector3 origin = transform.position;
                origin.x = Mathf.Lerp(EdgePositions[0].x, EdgePositions[1].x, i / TextureWidth);
                origin.z = Mathf.Lerp(EdgePositions[0].z, EdgePositions[3].z, j / TextureHeight);

                RaycastHit hit;
                if (Physics.Raycast(origin, Vector3.up, out hit, Mathf.Infinity))
                {
                    WaterMask.SetPixel(i, j, Color.black);
                }
                else
                {
                    WaterMask.SetPixel(i, j, Color.white);
                }
            }
        }
        WaterMask.Apply();
    }


    
    private void OnDrawGizmos()
    {
        Vector3 origin;
        origin.y = transform.TransformPoint(EdgePositions[0]).y;
        origin.x = Mathf.Lerp(transform.TransformPoint(EdgePositions[0]).x, transform.TransformPoint(EdgePositions[1]).x, testX);
        origin.z = Mathf.Lerp(transform.TransformPoint(EdgePositions[0]).z, transform.TransformPoint(EdgePositions[3]).z, testY);



        Gizmos.color = Color.green;
        Gizmos.DrawSphere(origin, 1000);
    }
}
