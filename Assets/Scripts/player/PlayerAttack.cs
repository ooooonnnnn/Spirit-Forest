using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private CollisionHit _attackHitbox;
    // Start is called before the first frame update


    void Start()
    {
        _attackHitbox = GetComponent<CollisionHit>();
    }

    // Update is called once per frame
    void Update()
    {
   
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Hit!");
        //    _attackHitbox.Attack();
           

        }
    }


}
