using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("Références")]
    [Tooltip("Texte UI pour afficher le score actuel")]
    public TextMeshProUGUI scoreText;

    [Tooltip("Texte UI pour afficher le meilleur score")]
    public TextMeshProUGUI bestScoreText;

    [Tooltip("Référence au joueur")]
    public Transform player;

    [Header("Paramètres")]
    [Tooltip("Afficher le meilleur score en jeu")]
    public bool showBestScore = true;

    [Header("Animation")]
    [Tooltip("Activer l'animation du score")]
    public bool animateScore = true;

    [Tooltip("Couleur du score quand il augmente")]
    public Color scoreIncreaseColor = Color.green;

    [Tooltip("Échelle maximale lors de l'animation (1.0 = normal)")]
    [Range(1f, 2f)]
    public float maxScale = 1.3f;

    [Tooltip("Durée de l'animation en secondes")]
    [Range(0.1f, 1f)]
    public float animationDuration = 0.3f;

    private int currentScore = 0;
    private int bestScore = 0;
    private float maxZReached = 0f;
    private int scoreMultiplier = 1;

    // Variables pour l'animation
    private Color originalColor;
    private Vector3 originalScale;
    private float animationTimer = 0f;
    private bool isAnimating = false;

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
    }

    void Start()
    {
        bestScore = PlayerPrefs.GetInt("BestScore", 0);

        if (scoreText != null)
        {
            originalColor = scoreText.color;
            originalScale = scoreText.transform.localScale;
        }

        UpdateUI();
    }

    void Update()
    {
        if (player != null)
        {
            float currentZ = player.position.z;

            if (currentZ > maxZReached)
            {
                int previousScore = currentScore;

                // ⭐ Calculer le score de base (distance parcourue)
                int baseScore = Mathf.FloorToInt(currentZ);

                // ⭐ Appliquer le multiplicateur UNIQUEMENT pour l'affichage
                currentScore = baseScore * scoreMultiplier;

                maxZReached = currentZ;

                // Déclencher l'animation si le score a changé
                if (currentScore > previousScore)
                {
                    if (animateScore)
                    {
                        StartScoreAnimation();
                    }

                    // Mettre à jour le meilleur score (sans multiplicateur pour être juste)
                    int realScore = baseScore; // Score réel sans multiplicateur
                    if (realScore > bestScore)
                    {
                        bestScore = realScore;
                        PlayerPrefs.SetInt("BestScore", bestScore);
                        PlayerPrefs.Save();
                    }

                    UpdateUI();
                }
            }
        }

        // Gérer l'animation
        if (isAnimating)
        {
            UpdateScoreAnimation();
        }
    }


    void StartScoreAnimation()
    {
        if (scoreText == null) return;

        isAnimating = true;
        animationTimer = 0f;
    }

    void UpdateScoreAnimation()
    {
        if (scoreText == null) return;

        animationTimer += Time.deltaTime;
        float progress = animationTimer / animationDuration;

        if (progress <= 1f)
        {
            // Animation en forme de courbe (monte puis descend)
            float scale = 1f + (Mathf.Sin(progress * Mathf.PI) * (maxScale - 1f));
            scoreText.transform.localScale = originalScale * scale;

            // Transition de couleur
            scoreText.color = Color.Lerp(scoreIncreaseColor, originalColor, progress);
        }
        else
        {
            // Fin de l'animation
            scoreText.transform.localScale = originalScale;
            scoreText.color = originalColor;
            isAnimating = false;
        }
    }

    void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text = currentScore.ToString();
        }

        if (bestScoreText != null && showBestScore)
        {
            bestScoreText.text = "Best: " + bestScore.ToString();
        }
    }

    public void ResetScore()
    {
        currentScore = 0;
        maxZReached = 0f;
        scoreMultiplier = 1;

        if (scoreText != null)
        {
            scoreText.transform.localScale = originalScale;
            scoreText.color = originalColor;
        }

        isAnimating = false;
        UpdateUI();
    }

    // ⭐ MODIFIÉ : Ne recalcule PLUS le score total
    public void SetMultiplier(int multiplier)
    {
        scoreMultiplier = multiplier;
        Debug.Log($"Multiplicateur de score défini à x{multiplier}");
        // ⭐ SUPPRIMÉ : Ne pas recalculer le score !
    }

    public int GetMultiplier()
    {
        return scoreMultiplier;
    }

    public int GetCurrentScore()
    {
        return currentScore;
    }

    public int GetBestScore()
    {
        return bestScore;
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
