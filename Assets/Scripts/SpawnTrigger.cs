using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTrigger : MonoBehaviour
{
    //when the player enters this trigger, some mobs spawn
    [SerializeField] private GameObject[] mobsToSpawn;
    [SerializeField] private Transform[] spawnAnchors;
    private bool triggered = false;

    public void Start()
    {
        //check mobsToSpawn and spawnAnchors correspond
        if (mobsToSpawn.Length != spawnAnchors.Length)
        {
            throw new Exception("Number of mobs must equal number of spawn anchors");
        }
    }
    
    public void OnTriggerEnter(Collider other)
    {
        if (!triggered && other.CompareTag("Player"))
        {
            //spawns all mobs in their locations
            for (int i = 0; i < mobsToSpawn.Length; i++)
            {
                Instantiate(mobsToSpawn[i], spawnAnchors[i]);
            }
            
            //spawning only happens once
            triggered = true;
        }
    }
}
