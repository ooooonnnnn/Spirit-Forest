using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionHit : MonoBehaviour
{
    bool mushroomInRange;
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        //if (Input.GetKeyDown(KeyCode.E))
        if (other.gameObject.CompareTag("Attack"))
        {
            Destroy(gameObject);
        }
    }
    void Start()

    {
        
    }

    // Update is called once per frame
   // public void Attack()
   // {
    //    if (mushroomInRange)
    //        Destroy(mushroom);
   // }
}
