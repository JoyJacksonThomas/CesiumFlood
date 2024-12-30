using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ShapeGenerator
{
    public static Mesh GeneratePlaneMesh(MeshRenderer renderer, MeshFilter filter, int resolution, int size)
    {
        renderer.sharedMaterial = new Material(Shader.Find("Standard"));

        Mesh planeMesh = UpdatePlaneMesh(filter, resolution, size);

        return planeMesh;
    }

    public static Mesh UpdatePlaneMesh(MeshFilter filter, int resolution, int size)
    {
        Mesh planeMesh = new Mesh();

        int vertexPerRow = VertexAndTriangles.GetVertexPerRow(resolution);
        int numberOfVertices = vertexPerRow * vertexPerRow;
        Vector3[] vertices = VertexAndTriangles.GetVertices(vertexPerRow, numberOfVertices, size);

        int[] triangles = VertexAndTriangles.GetTriangles(vertexPerRow, resolution);

        planeMesh.vertices = vertices;
        planeMesh.triangles = triangles;

        planeMesh.RecalculateNormals();

        filter.mesh = planeMesh;

        return planeMesh;
    }

    public static Mesh GenerateCubeFace(MeshRenderer renderer, MeshFilter filter, int resolution, int size, Vector3 direction)
    {
        renderer.sharedMaterial = new Material(Shader.Find("Standard"));

        Mesh planeMesh = UpdateCubeFaceMesh(filter, resolution, size, direction);

        return planeMesh;
    }

    public static Mesh UpdateCubeFaceMesh(MeshFilter filter, int resolution, int size, Vector3 direction)
    {
        Mesh planeMesh = new Mesh();

        int vertexPerRow = VertexAndTriangles.GetVertexPerRow(resolution);
        int numberOfVertices = vertexPerRow * vertexPerRow;
        Vector3[] vertices = VertexAndTriangles.GetCubeVertices(vertexPerRow, numberOfVertices, size, direction);

        int[] triangles = VertexAndTriangles.GetTriangles(vertexPerRow, resolution);

        planeMesh.vertices = vertices;
        planeMesh.triangles = triangles;

        planeMesh.RecalculateNormals();

        filter.mesh = planeMesh;

        return planeMesh;
    }

    public static Mesh GenerateSphereMesh(MeshRenderer renderer, MeshFilter filter, int resolution, int size, Vector3 direction)
    {
        renderer.sharedMaterial = new Material(Shader.Find("Custom/Sphere"));

        Mesh planeMesh = UpdateSphereMesh(filter, resolution, size, direction);

        return planeMesh;
    }

    public static Mesh UpdateSphereMesh(MeshFilter filter, int resolution, int size, Vector3 direction)
    {
        Mesh planeMesh = new Mesh();

        int vertexPerRow = VertexAndTriangles.GetVertexPerRow(resolution);
        int numberOfVertices = vertexPerRow * vertexPerRow;
        Vector3[] vertices = VertexAndTriangles.GetSphereVertices(vertexPerRow, numberOfVertices, size, direction);

        int[] triangles = VertexAndTriangles.GetTriangles(vertexPerRow, resolution);

        planeMesh.vertices = vertices;
        planeMesh.triangles = triangles;

        planeMesh.RecalculateNormals();

        filter.mesh = planeMesh;

        return planeMesh;
    }

    public static Mesh GenerateCircleFromMesh(Mesh mesh)
    {
        Mesh newMesh = new Mesh();
        newMesh.vertices = new Vector3[mesh.vertices.Length];
        newMesh.normals = new Vector3[mesh.normals.Length];

        Vector3[] newNormals = new Vector3[mesh.normals.Length];
        Vector3[] newVertices = new Vector3[mesh.vertices.Length];

        for (int i = 0; i < mesh.normals.Length; i++)
        {
            var newVertex = mesh.vertices[i].normalized;
            newNormals[i] = new Vector3(newVertex.x, newVertex.y, newVertex.z);
            newVertices[i] = new Vector3(newVertex.x, newVertex.y, newVertex.z);
        }

        newMesh.normals = newNormals;
        newMesh.vertices = newVertices;

        return newMesh;
    }
}
