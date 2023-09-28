using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverWindow : MonoBehaviour
{
    private int score;
    private Text scoreText;
    private Text highScoreText;

    private void Awake() {
        scoreText = transform.Find("scoreText").GetComponent<Text>();
        highScoreText = transform.Find("bestScoreText").GetComponent<Text>();
        transform.Find("retryButton").GetComponent<Button>().onClick.AddListener(Retry);
        transform.Find("resetButton").GetComponent<Button>().onClick.AddListener(ResetHighScore);
        
}

    private void Retry() {
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
        
    }

    // Start is called before the first frame update
    private void Start()
    {
        
        Bird.GetInstance().OnDied += Bird_OnDied;
        Hide();
    }

    private void Bird_OnDied(object sender, System.EventArgs e) {
        score = Level.GetInstance().GetPipesPassedCount() / 2;
        SetNewHighScore(score);
        scoreText.text = score.ToString();
        highScoreText.text = GetHighScore().ToString();
        Show();
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

    private void Show() {
        
        gameObject.SetActive(true);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static int GetHighScore() {
        return PlayerPrefs.GetInt("highscore");
    }

    public static void SetNewHighScore(int score) {
        int highscore = GetHighScore();
        if (score > highscore) {
            PlayerPrefs.SetInt("highscore", score);
            PlayerPrefs.Save();
        }
    }

    private void ResetHighScore() {
        PlayerPrefs.SetInt("highscore", 0);
        PlayerPrefs.Save();
        highScoreText.text = GetHighScore().ToString();
    }
}
