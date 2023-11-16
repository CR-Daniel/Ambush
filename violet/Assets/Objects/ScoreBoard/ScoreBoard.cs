using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreBoard : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;

    private int currentScore = 0;
    private int highScore = 0;

    private void Start()
    {
        // Load the high score from saved data
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        highScoreText.text = highScore.ToString();
    }

    public void RegisterComet(Comet comet)
    {
        comet.OnHit += HandleCometHit;
    }

    private void HandleCometHit(CometHitEventArgs args)
    {
        if (args.HitObject.CompareTag("Enemy"))
        {
            currentScore++;
            scoreText.text = currentScore.ToString();

            // Check and update high score
            if (currentScore > highScore)
            {
                highScore = currentScore;
                highScoreText.text = highScore.ToString();
                PlayerPrefs.SetInt("HighScore", highScore);
            }
        }
    }
}
