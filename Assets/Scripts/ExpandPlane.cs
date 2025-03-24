using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpandPlane : MonoBehaviour
{
    //Creates more planes when you get to the edge of one
    public static HashSet<Vector3Int> occupiedPositions = new HashSet<Vector3Int> { Vector3Int.zero }; //don't create a new plane in an  occupied position
    public Vector3Int thisPosition = Vector3Int.zero;
    [SerializeField] private GameObject planePrefab;
    
    public void CreatePlane(Vector3Int direction)
    {
        //create a new plane adjacent to this one, going off to direction
        Vector3Int newPos = thisPosition + direction;
        if (occupiedPositions.Contains(newPos)) return;

        occupiedPositions.Add(newPos);
        Vector3 actualPos = (Vector3)newPos * transform.lossyScale.x * 10;
        
        GameObject newPlane = Instantiate(planePrefab, actualPos, Quaternion.identity);
        newPlane.GetComponent<ExpandPlane>().thisPosition = newPos;
    }
}
