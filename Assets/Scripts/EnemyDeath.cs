using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeath : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        print("WOWOWOWOWO");
        if (other.CompareTag("Weapon"))
        {
            Destroy(this.gameObject);
        }
    }
}
