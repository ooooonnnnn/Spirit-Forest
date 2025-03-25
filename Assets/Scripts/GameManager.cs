using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    [Header("Player variables")]
    [SerializeField] private int health;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private Animator playerAnimator;
    
    [Header("UI Screens")] 
    [SerializeField] private GameObject ingameInfoCanvas;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private GameObject deathScreen;
    [SerializeField] private TMP_Text deathScreenTotalSouls;

    private void Start()
    {
        ingameInfoCanvas.SetActive(true);
        deathScreen.SetActive(false);
        healthText.text = "Health: " + health;
    }

    public void OnObstacleHit()
    {
        health--;
        if (health > 0)
        {
            healthText.text = "Health: " + health;
            playerMovement.SlowDownOnHit();
        }
        else
        {
            playerMovement.speed = 0;
            playerMovement.enabled = false;
            playerAnimator.enabled = false;
            
            ingameInfoCanvas.SetActive(false);
            deathScreen.SetActive(true);
            deathScreenTotalSouls.text = $"Collected {ScoreManager.inst.score} Souls";
        }
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene("ProceduralLevel");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
