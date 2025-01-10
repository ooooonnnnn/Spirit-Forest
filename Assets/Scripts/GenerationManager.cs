using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Splines;

public class GenerationManager : MonoBehaviour
{
    // Dynamically creates the level
	[SerializeField] private SplineContainer splineContainer;
	[SerializeField] private MeshFilter meshFilter;
	private Mesh mesh;
	
	public void Start()
	{
		mesh = new Mesh();
		
		int numCenters = 20;
		float[] positionsOnSpline = new float[numCenters];
		for (int i = 0; i < numCenters; i++) positionsOnSpline[i] = i * 1f / numCenters;
		float3[] centers = new float3[numCenters];
		float3[] forwards = new float3[numCenters];

		int numVertsPerCenter = 50;
		int numVerts = numVertsPerCenter * numCenters;
		Vector3[] vertices = new Vector3[numVerts];
		Vector2[] uvs = new Vector2[numVerts];
		Vector3[] normals = new Vector3[numVerts];
		
		for (int i = 0; i < numCenters; i++)
		{
			float3 dummy;
			splineContainer.Evaluate(positionsOnSpline[i], out centers[i], out forwards[i], out dummy);

			for (int j = 0; j < numVertsPerCenter; j++)
			{
				float angle = j * (2 * (float)Math.PI) / numVertsPerCenter;
				Vector3 side = Vector3.Cross(Vector3.up,  Vector3.Normalize(forwards[i]));
				Vector3 ray = Vector3.up * (float)Math.Cos(angle) + side * (float)Math.Sin(angle);

				int vertInd = i * numVertsPerCenter + j;
				vertices[vertInd] = (Vector3)centers[i] + ray;
				uvs[vertInd] = new Vector2((float)j / numVertsPerCenter, (float)i / numCenters);
				normals[vertInd] = ray;
			}
		}

		int[] triangles = new int[(numVerts - numVertsPerCenter) * 6];
		for (int i = 0; i < numCenters - 1; i++)
		{
			for (int j = 0; j < numVertsPerCenter; j++)
			{
				int startInd = i * numVertsPerCenter * 6 + j * 6;
				int nextVert = (j < numVertsPerCenter - 1) ? j + 1 : 0;
				triangles[startInd]     = i * numVertsPerCenter + j;
				triangles[startInd + 1] = (i + 1) * numVertsPerCenter + j;
				triangles[startInd + 2] = i * numVertsPerCenter + nextVert;
				triangles[startInd + 3] = (i + 1) * numVertsPerCenter + j;
				triangles[startInd + 4] = (i + 1) * numVertsPerCenter + nextVert;
				triangles[startInd + 5] = i * numVertsPerCenter + nextVert;
			}
		}
		
		mesh.Clear();
		mesh.vertices = vertices;
		mesh.uv = uvs;
		mesh.triangles = triangles;
		mesh.normals = normals;
		
		meshFilter.mesh = mesh;
	}

	
	//Test
	private int index = 0;
	public void Update()
	{
		// if (index >= mesh.vertices.Length) index = 0;
		// Debug.DrawLine(mesh.vertices[index], mesh.vertices[index] + 0.1f * Vector3.up, Color.white, 0.2f);
		// index++;
	}
}
