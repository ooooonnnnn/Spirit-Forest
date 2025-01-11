using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;
using Quaternion = System.Numerics.Quaternion;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class GenerationManager : MonoBehaviour
{
    // Dynamically creates the level
    [SerializeField] private Transform trail;
    [SerializeField] private int numSections;
    private List<GameObject> trailSections = new();

	[Header("Spline Parameters")] [SerializeField]
	private int numAnchors;
	[SerializeField] private float distanceBetweenAnchors;
	[SerializeField] private float angleRange;
	[SerializeField] private float tangentLength;

	[Header("Object Generation")]
	[SerializeField] private GameObject sectionPrefab;
	[SerializeField] private GameObject toriiPrefab;
	[SerializeField] private GameObject treePrefab;
	[SerializeField] private GameObject rockPrefab;
	[SerializeField] private GameObject bushPrefab;
	[SerializeField] private int maxNumToriiGates;
	[SerializeField] private float chanceToriiGate;
	[SerializeField] private float radiusAroundObjects; //to prevent overlap between objects
	[SerializeField] private float minDistFromTrail;
	[SerializeField] private float widthObjectField; //how far to the sides of the trail should objects spawn
	[SerializeField] private float chanceObject;
	[SerializeField] private int numRollsObjectSpawn; //how many times should the code attempt to spawn an object
	private List<Transform> treePositions = new();
	private List<Transform> rockPositions = new();
	private List<Transform> bushPositions = new();
	private List<Transform> toriigatePositions = new();
	

	public int currentSection = 0;
	
	public void Start()
	{
		//create an initial run of trail sections
		for (int i = 0; i < numSections; i++)
		{
			CreateSection();
		}
	}

	private void CreateSection()
	{
		//create trail
		GameObject newSection = Instantiate(sectionPrefab, trail);
		trailSections.Add(newSection);
		SplineContainer container = newSection.GetComponent<SplineContainer>();
		MeshFilter meshFilter = newSection.GetComponent<MeshFilter>();
			
		container.Spline = currentSection == 0
			? NewSpline(Vector3.zero, Vector3.forward)
			: NewSpline(trailSections[currentSection - 1].GetComponent<SplineContainer>());
		SetMeshFromSpline(container, meshFilter);

		currentSection++;
		
		//add torii gates
		int numGates = (Random.value < chanceToriiGate ? 1 : 0) * (int)(Math.Round(Random.value * (maxNumToriiGates - 1)) + 1); //1-x gates, not every time
		for (int i = 0; i < numGates; i++)
		{
			float3 position, heading;
			container.Evaluate(0.2f * i, out position, out heading, out _);

			toriigatePositions.Add(Instantiate(toriiPrefab, position,
				UnityEngine.Quaternion.FromToRotation(Vector3.forward, heading)).transform);
		}

		//add trees, rocks and bushes
		//save their positions in a map to prevent overlapping
		
		for (int i = 0; i < numRollsObjectSpawn; i++)
		{
			if (Random.value < chanceObject)
			{
				//determine where to spawn the new object
				float3 positionOnCurve, tangent, upVec;
				container.Evaluate((float)i / (numRollsObjectSpawn - 1), out positionOnCurve, out tangent, out upVec);
				Vector3 side = Vector3.Cross(Vector3.Normalize(tangent), Vector3.Normalize(upVec));
				float distance = math.lerp(minDistFromTrail, widthObjectField, Random.value);
				Vector3 position = (Random.value > 0.5 ? 1 : -1) * side * distance + (Vector3)positionOnCurve;
				
				//determine what kind of object it is
				ObjectType objectType;
				switch (Random.value)
				{
					case < 1f/3:
						objectType = ObjectType.Bush;
						break;
					case < 2f/3:
						objectType = ObjectType.Rock;
						break;
					default:
						objectType = ObjectType.Tree;
						break;
				}
				
				//determine which objects it should not interfere with
				List<ObjectType> types = new();
				switch (objectType)
				{
					case ObjectType.Bush:
						types.Add(ObjectType.Rock);
						break;
					case ObjectType.Rock:
						types.Add(ObjectType.Tree);
						types.Add(ObjectType.Bush);
						types.Add(ObjectType.ToriiGate);
						break;
					case ObjectType.Tree:
						types.Add(ObjectType.Tree);
						types.Add(ObjectType.Rock);
						types.Add(ObjectType.ToriiGate);
						break;
				}
				
				//check for interference with relevant existing objects
				bool interference = false;
				foreach (ObjectType type in types)
				{
					List<Transform> objects = new();
					switch (type)
					{
						case ObjectType.Bush:
							objects = bushPositions;
							break;
						case ObjectType.Rock:
							objects = rockPositions;
							break;
						case ObjectType.Tree:
							objects = treePositions;
							break;
						case ObjectType.ToriiGate:
							objects = toriigatePositions;
							break;
					}
					if (objects.Any(t => Vector3.Distance(t.position, position) < radiusAroundObjects)) interference = true;
				}
				if (interference) continue;
				
				//no interference: place object
				List<Transform> targetList = new();
				GameObject targetPrefab = new();
				switch (objectType)
				{
					case ObjectType.Rock:
						targetList = rockPositions;
						targetPrefab = rockPrefab;
						break;
					case ObjectType.Bush:
						targetList = bushPositions;
						targetPrefab = bushPrefab;
						break;
					case ObjectType.Tree:
						targetList = treePositions;
						targetPrefab = treePrefab;
						break;
				}
				targetList.Add(Instantiate(targetPrefab,
					position,
					UnityEngine.Quaternion.AngleAxis(Random.value * (float)Math.PI, Vector3.up))
					.transform);
			}
		}
		
	}
	
	private enum ObjectType
	{
		Rock, Tree, Bush, ToriiGate
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
			Vector3 tangent = (Vector3)direction * tangentLength;
			spline.Add(new BezierKnot(origin, -tangent, tangent));
			
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
		
		int numCenters = 50; //points on the curve
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
				float angle = j * (2 * (float)Math.PI) / (numVertsPerCenter - 1);
				Vector3 ray = UnitVectorByAngle(forwards[i], Vector3.up, angle);

				int vertInd = i * numVertsPerCenter + j;
				vertices[vertInd] = (Vector3)centers[i] + SnakeToTrail(ray);
				uvs[vertInd] = new Vector2((float)j / (numVertsPerCenter - 1), (float)i / (numCenters - 1));
				normals[vertInd] = ray;
			}
		}

		int[] triangles = new int[(numVerts - numVertsPerCenter) * 6];
		for (int i = 0; i < numCenters - 1; i++)
		{
			for (int j = 0; j < numVertsPerCenter - 1; j++)
			{
				int startInd = i * numVertsPerCenter * 6 + j * 6;
				//int nextVert = (j < numVertsPerCenter - 1) ? j + 1 : 0;
				triangles[startInd]     = i * numVertsPerCenter + j;
				triangles[startInd + 1] = (i + 1) * numVertsPerCenter + j;
				triangles[startInd + 2] = i * numVertsPerCenter + j + 1;
				triangles[startInd + 3] = (i + 1) * numVertsPerCenter + j;
				triangles[startInd + 4] = (i + 1) * numVertsPerCenter + j + 1;
				triangles[startInd + 5] = i * numVertsPerCenter + j + 1;
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
		//vector transformation to turn the default tube shape into a trail shape
		return new Vector3(input.x, input.y * 0.2f, input.z);
	}
	
	private Vector3 UnitVectorByAngle(Vector3 axis, Vector3 forward, float angle)
	{
		//returns a vector that is forward rotated by angle radians about up
		axis = axis.normalized;
		forward = forward.normalized;
		Vector3 side = -Vector3.Cross(axis, forward);

		return forward * (float)Math.Cos(angle) + side * (float)Math.Sin(angle);
	}
}
