using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreWindow : MonoBehaviour
{

    private Text scoreText;
    private int score;

    private void Awake() {
        scoreText = transform.Find("scoreText").GetComponent<Text>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        score = Level.GetInstance().GetPipesPassedCount() / 2;
        scoreText.text = score.ToString();
    }
}
