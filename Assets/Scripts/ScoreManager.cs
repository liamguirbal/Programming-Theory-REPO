using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("Score")]
    public int currentScore = 0;
    public TextMeshProUGUI scoreText;

    [Header("Best Score")]
    private int bestScore = 0;

    [Header("Multiplicateur")]
    private int scoreMultiplier = 1;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Charger le meilleur score
        bestScore = PlayerPrefs.GetInt("BestScore", 0);
    }

    void Start()
    {
        UpdateScoreUI();
    }

    // Ajouter des points (avec multiplicateur)
    public void AddScore(int points)
    {
        int pointsToAdd = points * scoreMultiplier;
        currentScore += pointsToAdd;

        
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayScorePoint();
        }

        Debug.Log($"Points ajoutés : {points} x{scoreMultiplier} = {pointsToAdd}");

        if (currentScore > bestScore)
        {
            bestScore = currentScore;
            PlayerPrefs.SetInt("BestScore", bestScore);
            PlayerPrefs.Save();
        }

        UpdateScoreUI();
    }


 
    public void SetMultiplier(int multiplier)
    {
        scoreMultiplier = multiplier;
        Debug.Log($"Multiplicateur de score : x{scoreMultiplier}");
        UpdateScoreUI();
    }


    public int GetCurrentScore()
    {
        return currentScore;
    }

 
    public int GetBestScore()
    {
        return bestScore;
    }

 
    public int GetScore()
    {
        return currentScore;
    }

   
    public void ResetScore()
    {
        currentScore = 0;
        scoreMultiplier = 1;
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            if (scoreMultiplier > 1)
            {
                scoreText.text = $" {currentScore} (x{scoreMultiplier})";
            }
            else
            {
                scoreText.text = $" {currentScore}";
            }
        }
    }
}
