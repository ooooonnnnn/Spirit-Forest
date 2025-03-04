using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //
    [SerializeField] private int health;
    [SerializeField] private PlayerMovement playerMovement;
    
    public void OnObstacleHit()
    {
        health--;
        print(health <= 0 ? "dead" : $"ouch, health is {health}");
        playerMovement.SlowDownOnHit();
    }
}
