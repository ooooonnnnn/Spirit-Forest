using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSouls : MonoBehaviour
{
    [SerializeField] float speed = 10f; // Speed of movement
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.back * speed * Time.deltaTime);
    }
}
