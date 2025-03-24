using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneBoundary : MonoBehaviour
{
    [SerializeField] private Vector3Int myDirection;
    [SerializeField] private ExpandPlane plane;

    private void Start()
    {
        //test
        print((Vector3)(Vector2)Vector2Int.down);
        print(Vector2Int.down);
        
        HashSet<Vector3Int> allowedDirs = new HashSet<Vector3Int>
            { Vector3Int.forward, Vector3Int.back, Vector3Int.left, Vector3Int.right };
        
        if (!allowedDirs.Contains(myDirection))
        {
            throw new Exception("direction must be orthonormal vector");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        plane.CreatePlane(myDirection);
    }
}
