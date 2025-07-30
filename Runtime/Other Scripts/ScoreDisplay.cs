using UnityEngine;
using TMPro; 

public class ScoreDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;

    private float currentScore = 0f;

    public void UpdateScore(float newScore)
    {
        currentScore = newScore * 1000;
        if (scoreText != null)
        {
            scoreText.text = "Score: " + currentScore.ToString("F0"); 
        }
    }

}