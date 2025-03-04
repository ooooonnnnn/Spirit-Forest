using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySoulBehind : MonoBehaviour
{
    public float destroyDistance = 30f; // Distance behind the player to destroy the object
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        // Destroy the object if it's too far behind the player
        if (transform.position.z < -destroyDistance)
        {
            Destroy(gameObject);
        }
    }
}
