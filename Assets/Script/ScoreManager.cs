using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [Header("Score Text")]
    public TextMeshProUGUI scoreText;

    private int currentScore = 0;

    void Start()
    {
        UpdateScoreUI();
    }

    public void AddScore(int amount)
    {
        currentScore += amount;
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = $"Score : {currentScore}";
    }

    public void ResetScore()
    {
        currentScore = 0;
        UpdateScoreUI();
    }
}
