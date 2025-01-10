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

		int numVertsPerCenter = 10;
		Vector3[] vertices = new Vector3[numCenters * numVertsPerCenter];
		
		for (int i = 0; i < numCenters; i++)
		{
			float3 dummy;
			splineContainer.Evaluate(positionsOnSpline[i], out centers[i], out forwards[i], out dummy);

			for (int j = 0; j < numVertsPerCenter; j++)
			{
				float angle = j * (2 * (float)Math.PI) / numVertsPerCenter;
				Vector3 side = Vector3.Cross(Vector3.up,  Vector3.Normalize(forwards[i]));
				Vector3 ray = Vector3.up * (float)Math.Cos(angle) + side * (float)Math.Sin(angle);
				vertices[i * numVertsPerCenter + j] = (Vector3)centers[i] + ray;
			}
		}
		mesh.vertices = vertices;
	}

	private int i = 0;
	public void Update()
	{
		if (i >= mesh.vertices.Length) i = 0;
		Debug.DrawLine(mesh.vertices[i], mesh.vertices[i] + 0.1f * Vector3.up, Color.white, 0.2f);
		i++;
	}
}
