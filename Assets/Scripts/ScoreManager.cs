using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public int score;
    public static ScoreManager inst;
    [SerializeField] TMP_Text scoreText;
    
    
    
    public void AddScore()
    {
        score++;
        scoreText.text = "Score:" +score;
        
    }
    private void Awake()
    {
        inst = this;
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
