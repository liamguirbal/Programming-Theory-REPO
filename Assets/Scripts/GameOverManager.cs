using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance;

    [Header("Références UI")]
    [Tooltip("Panel principal du Game Over")]
    public GameObject gameOverPanel;

    [Tooltip("Texte affichant le score final")]
    public TextMeshProUGUI finalScoreText;

    [Tooltip("Texte affichant le meilleur score")]
    public TextMeshProUGUI bestScoreText;

    [Tooltip("Texte 'GAME OVER'")]
    public TextMeshProUGUI gameOverText;

    [Header("Animation")]
    [Tooltip("Durée de l'animation d'apparition")]
    [Range(0.1f, 2f)]
    public float animationDuration = 0.5f;

    [Tooltip("Délai avant l'apparition du panel")]
    [Range(0f, 2f)]
    public float appearDelay = 0.5f;

    [Tooltip("Type d'animation")]
    public AnimationType animationType = AnimationType.ScaleAndFade;

    [Header("Paramètres")]
    [Tooltip("Nom de la scène de jeu pour restart")]
    public string gameSceneName = "MainGame";

    [Tooltip("Nom de la scène du menu")]
    public string menuSceneName = "Menu";

    private CanvasGroup canvasGroup;
    private RectTransform panelRect;
    private bool isAnimating = false;
    private float animationTimer = 0f;
    private Vector3 originalScale;

    public enum AnimationType
    {
        ScaleAndFade,
        SlideDown,
        Bounce
    }

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
        // S'assurer que le panel est caché au départ
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);

            // Ajouter un CanvasGroup si pas déjà présent
            canvasGroup = gameOverPanel.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameOverPanel.AddComponent<CanvasGroup>();
            }

            panelRect = gameOverPanel.GetComponent<RectTransform>();
            originalScale = panelRect.localScale;
        }
    }

    void Update()
    {
        // TEST : Appuyer sur G pour afficher le Game Over
        if (Input.GetKeyDown(KeyCode.G))
        {
            ShowGameOver();
        }

        if (isAnimating)
        {
            UpdateAnimation();
        }
    }

    // Fonction appelée quand le joueur meurt
    public void ShowGameOver()
    {
        Debug.Log("ShowGameOver appelé !");

        if (gameOverPanel == null)
        {
            Debug.LogError("gameOverPanel est NULL !");
            return;
        }

        // Lancer la séquence de Game Over avec délai
        StartCoroutine(GameOverSequence());
    }

    IEnumerator GameOverSequence()
    {
        // Récupérer les scores tout de suite
        int currentScore = 0;
        int bestScore = 0;

        if (ScoreManager.Instance != null)
        {
            currentScore = ScoreManager.Instance.GetCurrentScore();
            bestScore = ScoreManager.Instance.GetBestScore();
        }

        // Mettre à jour les textes
        if (finalScoreText != null)
        {
            finalScoreText.text = currentScore.ToString();
        }

        if (bestScoreText != null)
        {
            bestScoreText.text = "Best: " + bestScore.ToString();
        }

        // ATTENDRE 0.5 secondes AVANT de mettre en pause
        yield return new WaitForSeconds(0.5f);

        // MAINTENANT on met en pause
        Time.timeScale = 0f;

        // Lancer l'animation avec le délai configuré
        StartCoroutine(StartGameOverAnimationDelayed());
    }

    IEnumerator StartGameOverAnimationDelayed()
    {
        // Attendre en temps réel (ignore timeScale)
        yield return new WaitForSecondsRealtime(appearDelay);

        Debug.Log("Animation démarrée !");

        gameOverPanel.SetActive(true);
        isAnimating = true;
        animationTimer = 0f;

        // Configuration initiale selon le type d'animation
        switch (animationType)
        {
            case AnimationType.ScaleAndFade:
                panelRect.localScale = Vector3.zero;
                canvasGroup.alpha = 0f;
                break;
            case AnimationType.SlideDown:
                panelRect.anchoredPosition = new Vector2(0, Screen.height);
                canvasGroup.alpha = 1f;
                break;
            case AnimationType.Bounce:
                panelRect.localScale = Vector3.zero;
                canvasGroup.alpha = 1f;
                break;
        }
    }

    void UpdateAnimation()
    {
        // Utiliser unscaledDeltaTime car Time.timeScale = 0
        animationTimer += Time.unscaledDeltaTime;
        float progress = Mathf.Clamp01(animationTimer / animationDuration);

        switch (animationType)
        {
            case AnimationType.ScaleAndFade:
                AnimateScaleAndFade(progress);
                break;
            case AnimationType.SlideDown:
                AnimateSlideDown(progress);
                break;
            case AnimationType.Bounce:
                AnimateBounce(progress);
                break;
        }

        if (progress >= 1f)
        {
            isAnimating = false;
            Debug.Log("Animation terminée !");
        }
    }

    void AnimateScaleAndFade(float progress)
    {
        // Courbe d'ease-out pour un effet smooth
        float easedProgress = 1f - Mathf.Pow(1f - progress, 3f);

        panelRect.localScale = originalScale * easedProgress;
        canvasGroup.alpha = easedProgress;
    }

    void AnimateSlideDown(float progress)
    {
        // Ease-out
        float easedProgress = 1f - Mathf.Pow(1f - progress, 3f);

        float targetY = 0f;
        float startY = Screen.height;
        float currentY = Mathf.Lerp(startY, targetY, easedProgress);

        panelRect.anchoredPosition = new Vector2(0, currentY);
    }

    void AnimateBounce(float progress)
    {
        // Effet de rebond élastique
        float bounce = Mathf.Sin(progress * Mathf.PI * 2f) * (1f - progress) * 0.3f;
        float scale = progress + bounce;

        panelRect.localScale = originalScale * scale;
    }

    // Fonction pour le bouton Restart
    public void RestartGame()
    {
        Time.timeScale = 1f; // Remettre le temps normal
        SceneManager.LoadScene(gameSceneName);
    }

    // Fonction pour le bouton Menu
    public void ReturnToMenu()
    {
        Time.timeScale = 1f; // Remettre le temps normal
        SceneManager.LoadScene(menuSceneName);
    }
}
