using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class ScoreManager : MonoBehaviour
{
    public int score;
    public static ScoreManager inst;
    [SerializeField] TMP_Text scoreText;
    
    public void AddScore()
    {
        score++;
        scoreText.text = "Souls: " + score;
    }

    public void AddScore(int score)
    {
        for (int i = 0; i < score; i++)
        {
            AddScore();
        }
    }
    
    private void Awake()
    {
        inst = this;
        scoreText.text = "Souls: 0";
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
