using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulCollector : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag( "Player"))
        {
            transform.Find("Fire").GetComponent<DeleteAfter>().AboutToDie();
            Destroy(gameObject);
            //Destroy(gameObject);
            ScoreManager.inst.AddScore();
        }
       
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
