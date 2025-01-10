using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Splines;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class GenerationManager : MonoBehaviour
{
    // Dynamically creates the level
    [SerializeField] private GameObject sectionPrefab;
    [SerializeField] private Transform trail;
    [SerializeField] private int numSections;
    private List<GameObject> trailSections = new();

	[Header("Spline Parameters")] [SerializeField]
	private int numAnchors;
	[SerializeField] private float distanceBetweenAnchors;
	[SerializeField] private float angleRange;
	
	
	public void Start()
	{
		//create an initial run of trail sections
		Vector3 origin = Vector3.zero;
		Vector3 direction = Vector3.forward;
		for (int i = 0; i < numSections; i++)
		{
			GameObject newSection = Instantiate(sectionPrefab, trail);
			trailSections.Add(newSection);
			SplineContainer container = newSection.GetComponent<SplineContainer>();
			MeshFilter meshFilter = newSection.GetComponent<MeshFilter>();
			container.Spline = i == 0
				? NewSpline(origin, direction)
				: NewSpline(trailSections[i - 1].GetComponent<SplineContainer>());
			SetMeshFromSpline(container, meshFilter);
		}
	}

	private Spline NewSpline(SplineContainer oldSpline)
	{
		//makes a new SplineContainer that is tangent and congruent with another spline oldSpline
		BezierKnot lastAnchor = oldSpline.Spline.Last();
		return NewSpline(lastAnchor.Position, Vector3.Normalize(lastAnchor.TangentOut));
	}

	private Spline NewSpline(Vector3 origin, Vector3 direction)
	{
		//makes a new SplineContainer with a spline that starts in origin and initially faces towards direction
		Spline spline = new Spline();
		for (int i = 0; i < numAnchors; i++)
		{
			spline.Add(new BezierKnot(origin, -direction, direction));
			
			float dirChange = (Random.value - 0.5f) * angleRange * 2;
			print(dirChange);
			direction = UnitVectorByAngle(Vector3.up, direction, dirChange);
			origin += distanceBetweenAnchors * direction;
		}
		return spline;
	}
	
	private void SetMeshFromSpline(SplineContainer splineContainer, MeshFilter meshFilter)
	{
		//sets the mesh in meshFilter as a snake with radius 1 around the spline in splineContainer
		
		Mesh mesh = new Mesh();
		
		int numCenters = 50;
		float[] positionsOnSpline = new float[numCenters];
		for (int i = 0; i < numCenters; i++) positionsOnSpline[i] = i * 1f / (numCenters - 1);
		float3[] centers = new float3[numCenters];
		float3[] forwards = new float3[numCenters];

		int numVertsPerCenter = 50;
		int numVerts = numVertsPerCenter * numCenters;
		Vector3[] vertices = new Vector3[numVerts];
		Vector2[] uvs = new Vector2[numVerts];
		Vector3[] normals = new Vector3[numVerts];
		
		for (int i = 0; i < numCenters; i++)
		{
			splineContainer.Evaluate(positionsOnSpline[i], out centers[i], out forwards[i], out _);

			for (int j = 0; j < numVertsPerCenter; j++)
			{
				float angle = j * (2 * (float)Math.PI) / numVertsPerCenter;
				Vector3 ray = UnitVectorByAngle(forwards[i], Vector3.up, angle);

				int vertInd = i * numVertsPerCenter + j;
				vertices[vertInd] = (Vector3)centers[i] + SnakeToTrail(ray);
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

	private Vector3 SnakeToTrail(Vector3 input)
	{
		return new Vector3(input.x, input.y * 0.2f, input.z);
	}
	
	
	private Vector3 UnitVectorByAngle(Vector3 axis, Vector3 forward, float angle)
	{
		//returns a vector that is forward rotated by angle radian about up
		axis = axis.normalized;
		forward = forward.normalized;
		Vector3 side = -Vector3.Cross(axis, forward);

		return forward * (float)Math.Cos(angle) + side * (float)Math.Sin(angle);
	}
}
