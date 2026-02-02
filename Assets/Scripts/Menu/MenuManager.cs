using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [Header("Références UI")]
    [Tooltip("Texte affichant le meilleur score")]
    public TextMeshProUGUI bestScoreText;

    [Header("Paramètres")]
    [Tooltip("Nom de la scène de jeu (doit correspondre au nom exact)")]
    public string gameSceneName = "MainGame";

    [Tooltip("Afficher le meilleur score dans le menu")]
    public bool showBestScore = true;

    void Start()
    {
        // Afficher le meilleur score si activé
        if (showBestScore && bestScoreText != null)
        {
            int bestScore = PlayerPrefs.GetInt("BestScore", 0);
            bestScoreText.text = "Best Score: " + bestScore;
        }
    }

    // Fonction appelée par le bouton Play
    public void PlayGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    // Fonction pour quitter le jeu
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    // Fonction pour réinitialiser le meilleur score
    public void ResetBestScore()
    {
        PlayerPrefs.SetInt("BestScore", 0);
        PlayerPrefs.Save();

        if (bestScoreText != null)
        {
            bestScoreText.text = "Best Score: 0";
        }
    }
}
