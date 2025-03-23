using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class ObstacleInteraction : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private PlayerAudio playerAudio;

    private void OnTriggerEnter(Collider other)
    {
        // print("hit");
        if (other.gameObject.CompareTag("Obstacle"))
        {
            other.enabled = false;
            gameManager.OnObstacleHit();
            playerAudio.FallSound();
        }
    }
}
