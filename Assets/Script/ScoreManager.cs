using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [Header("Score Text")]
    public TextMeshProUGUI scoreText;

    private int currentScore = 0;

    void Start()
    {
        // Initialize score display
        UpdateScoreUI();
    }

    public void AddScore(int amount)
    {
        // Update the current score
        currentScore += amount;
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        // Update the score text UI
        if (scoreText != null)
            scoreText.text = $"Score : {currentScore}";
    }

    public void ResetScore()
    {
        // Reset the score to zero
        currentScore = 0;
        UpdateScoreUI();
    }
}
