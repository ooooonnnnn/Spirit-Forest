using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Splines;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;

public class GenerationManager : MonoBehaviour
{
    // Dynamically creates the level
    [Header("Trail Parameters")]
    [SerializeField] private Transform trailOrigin;
    [SerializeField] public float laneWidth;
    // [SerializeField] private int numSections;
    private Dictionary<int,SplineContainer> trailSections = new();

	[Header("Spline Parameters")] [SerializeField]
	private int numAnchors;
	[SerializeField] private float distanceBetweenAnchors;
	[SerializeField] private float angleRange;
	[SerializeField] private float tangentLength;

	[Header("Object Prefabs")]
	[SerializeField] private GameObject sectionPrefab;
	[SerializeField] private GameObject toriiPrefab;
	[SerializeField] private GameObject treePrefab;
	[SerializeField] private GameObject rockPrefab;
	[SerializeField] private GameObject bushPrefab;
	[SerializeField] private GameObject[] obstaclePrefabs;

	[Header("Obstable Generation")]
	[SerializeField] private int numRollsObstacles; //max number of obstacles per spline unit. shouldn't be changed in runtime
	[SerializeField] [Range(0f,1f)] public float totalObstacleChance;
	[SerializeField] public float[] ratioObstacleChances;
	private int[][] obstacleLaneOpts;
	[SerializeField] private float obstacleHeight;

    [Header("Prop Generation")]
	[SerializeField] private int maxNumToriiGates;
	[SerializeField] private float chanceToriiGate;
	[SerializeField] private float radiusAroundObjects; //to prevent overlap between objects
	[SerializeField] private float minDistFromTrail;
	[SerializeField] private float widthObjectField; //how far to the sides of the trail should objects spawn
	[SerializeField] private float chanceObject;
	[SerializeField] private int numRollsObjectSpawn; //how many times should the code attempt to spawn an object
	private List<Vector3> treePositions = new();
	private List<Vector3> rockPositions = new();
	private List<Vector3> bushPositions = new();
	private List<Vector3> toriigatePositions = new();

	private int currentSection = -1;

	public void Start()
	{
		instantiationQueue = objectInstantiator.goQueue; // splitting the instantiate calls between several frames
		
		//hard coded obstacle lane spawn options
		obstacleLaneOpts = new int[obstaclePrefabs.Length][];
		obstacleLaneOpts[0] = new [] {-1, 0, 1};//grave
		obstacleLaneOpts[1] = new [] {-1, 0, 1};//stump
		obstacleLaneOpts[2] = new [] {-1, 1};//mushroom
		
		//check that the number of obstacle prefabs matches the chance ratio array length
		if (ratioObstacleChances.Length != obstaclePrefabs.Length &&
		    obstaclePrefabs.Length != obstacleLaneOpts.Length)
			throw new Exception("every obstacle must be assigned a chance to appear and possible lanes");
		
		InvokeRepeating(nameof(CreateSection), 0f, 1f);
	}

	
	// //test trail mesh
	// private SplineContainer testContainer;
	// private MeshFilter testMeshFilter;
	// public void Update()
	// {
	// 	//SetMeshFromSpline(testContainer, testMeshFilter);
	// }

	public void InerpolateToTransform(float t, Transform targetTransform)
	{
		//use interpolation value t to get a position and rotation, and write them to transform.
		//t goes from 0 to the total number of trail sections created. 
		InterpolateToVectors(t, out Vector3 position, out Vector3 tangent, out _);
		targetTransform.position = position;
		targetTransform.rotation = Quaternion.LookRotation(tangent);
	}

	private void InterpolateToVectors(float t, out Vector3 position, out Vector3 tangent, out Vector3 right)
	{
		//use interpolation value t to get a position, tangent, and side vectors, and write them to the out vectors.
		//t goes from 0 to the total number of trail sections created.
		int sectionNum = (int)t;
		t -= sectionNum; //t from 0 to 1
		SplineContainer section = trailSections[sectionNum];
		
		float3 pos, tan, up;
		section.Evaluate(t, out pos, out tan, out up);
		position = pos;
		tangent = ((Vector3)tan).normalized;
		right = Vector3.Cross(up, tan).normalized;
	}

	[SerializeField] private ObjectInstantiator objectInstantiator;
	private Queue<InstantiationData> instantiationQueue;
	private void CreateSection()
	{
		//create trail
		currentSection++;
		GameObject newSection = Instantiate(sectionPrefab, trailOrigin);
		SplineContainer container = newSection.GetComponent<SplineContainer>();
		trailSections.Add(currentSection, container);
		MeshFilter meshFilter = newSection.GetComponent<MeshFilter>();
			
		container.Spline = currentSection == 0
			? NewSpline(Vector3.zero, Vector3.forward)
			: NewSpline(trailSections[currentSection - 1]);
		SetMeshFromSpline(container, meshFilter);
		
		//--------------------------add obstacles--------------------------
		for (int i = 0; i < numRollsObstacles; i++)
		{
			//determine whether to create an obstacle
			if (Random.value <= totalObstacleChance)
			{
				//choose what to instantiate
				float cumProb = 0;
				int chosenObstacle = -1;
				for (int j = 0; j < obstaclePrefabs.Length; j++)
				{
					cumProb += ratioObstacleChances[j];
					if (Random.value <= cumProb)
					{
						chosenObstacle = j;
						break;
					}
				}
				if (chosenObstacle == -1)
				{
					throw new Exception("total obstacle spawn chances not equal to 1");
				}
				
				GameObject targetPrefab = obstaclePrefabs[chosenObstacle];
				int numOpts = obstacleLaneOpts[chosenObstacle].Length;
				int targetLane = obstacleLaneOpts[chosenObstacle][Random.Range(0, numOpts)];
				
				//determine actual position
				float interpolant = (float)i / numRollsObstacles + currentSection;
				InterpolateToVectors(interpolant, out Vector3 position, out Vector3 tangent, out Vector3 side);
				position += targetLane * laneWidth * side + obstacleHeight*Vector3.up;
				
				//add to instantiation queue
				instantiationQueue.Enqueue(new InstantiationData(targetPrefab,
					position,
					Quaternion.FromToRotation(Vector3.forward, tangent)));
			}
		}
		
		
		//--------------------------add props------------------------------
		//add torii gates
		int numGates = (Random.value < chanceToriiGate ? 1 : 0) * (int)(Math.Round(Random.value * (maxNumToriiGates - 1)) + 1); //1-x gates, not every time
		for (int i = 0; i < numGates; i++)
		{
			container.Evaluate(0.2f * i, out float3 position, out float3 heading, out _);

			instantiationQueue.Enqueue(new InstantiationData(toriiPrefab, position
				, Quaternion.FromToRotation(Vector3.forward, heading)));
			toriigatePositions.Add(new Vector3(position.x, position.y, position.z));
		}

		//add trees, rocks and bushes
		//save their positions in a map to prevent overlapping
		
		for (int i = 0; i < numRollsObjectSpawn; i++)
		{
			if (Random.value <= chanceObject)
			{
				//determine where to spawn the new object
				float interpolant = (float)i / (numRollsObjectSpawn - 1) + currentSection;
				InterpolateToVectors(interpolant, out Vector3 positionOnCurve, out _, out Vector3 side);
				float distance = math.lerp(minDistFromTrail, widthObjectField, Random.value);
				Vector3 position = (Random.value > 0.5 ? 1 : -1) * distance * side + positionOnCurve;
				
				//determine what kind of object it is
				ObjectType objectType;
				switch (Random.value)
				{
					case < 1f/5:
						objectType = ObjectType.Bush;
						break;
					case < 2f/5:
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
					List<Vector3> objects = new();
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
					if (objects.Any(pos => Vector3.Distance(pos, position) < radiusAroundObjects)) interference = true;
				}
				if (interference) continue;
				
				//no interference: place object
				List<Vector3> targetList = new();
				GameObject targetPrefab = null;
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

				targetList.Add(new Vector3(position.x, position.y, position.z));
				instantiationQueue.Enqueue(new InstantiationData(targetPrefab, position,
					UnityEngine.Quaternion.AngleAxis(Random.value * 360, Vector3.up)));
					
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

		Vector3[] rays = new Vector3[numVertsPerCenter];
		for (int i = 0; i < numVertsPerCenter; i++)
		{
			rays[i] = UnitVectorByAngle(Vector3.forward, Vector3.up,
				i * (2 * (float)Math.PI) / (numVertsPerCenter - 1));
		}
		
		for (int i = 0; i < numCenters; i++)
		{
			splineContainer.Evaluate(positionsOnSpline[i], out centers[i], out forwards[i], out _);
			Matrix4x4 rotation = Matrix4x4.Rotate(Quaternion.FromToRotation(Vector3.forward, (Vector3)forwards[i]));

			for (int j = 0; j < numVertsPerCenter; j++)
			{
				int vertInd = i * numVertsPerCenter + j;
				vertices[vertInd] = (Vector3)centers[i] + (Vector3)(rotation * SnakeToTrail(rays[j]));
				uvs[vertInd] = new Vector2((float)j / (numVertsPerCenter - 1), (float)i / (numCenters - 1));
				normals[vertInd] = rotation * rays[j];
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

	[SerializeField] private float trailMeshWidth;
	[SerializeField] private float trailMeshHeight;
	private Vector3 SnakeToTrail(Vector3 input)
	{
		//vector transformation to turn the default tube shape into a trail shape
		return new Vector3(input.x * trailMeshWidth, input.y * trailMeshHeight, input.z);
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
