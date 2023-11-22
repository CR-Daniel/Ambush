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

    public delegate void SpeedAdjustmentHandler(float newSpeed);
    public static event SpeedAdjustmentHandler OnSpeedAdjustment;

    private Queue<float> hitTimestamps = new Queue<float>();
    private float timeWindow = 5f; // 5 seconds window
    private float baseSpeed = 4f;
    private float speedIncreasePerHit = 1f; // Increase in speed per hit

    private void Start()
    {
        // Load the high score from saved data
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        highScoreText.text = highScore.ToString();
    }

    private void Update()
    {
        while (hitTimestamps.Count > 0 && hitTimestamps.Peek() < Time.time - timeWindow)
        {
            hitTimestamps.Dequeue();
        }

        float newSpeed = baseSpeed + hitTimestamps.Count * speedIncreasePerHit;
        OnSpeedAdjustment?.Invoke(newSpeed);
    }

    public void HandleCometHitEnemy()
    {
        hitTimestamps.Enqueue(Time.time);
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
