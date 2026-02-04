using UnityEngine;
using UnityEngine.UI;

public class SkipMusicButtonUI : MonoBehaviour
{
    void Start()
    {
        // Ajouter le listener au bouton au démarrage
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(SkipMusic);
        }
    }

    // ⭐ Appelé quand on clique le bouton
    public void SkipMusic()
    {
        // Trouver l'AudioManager dynamiquement (peu importe où il est)
        AudioManager audioManager = FindFirstObjectByType<AudioManager>();

        if (audioManager != null)
        {
            audioManager.SkipToNextMusic();
            Debug.Log("Musique suivante !");
        }
        else
        {
            Debug.LogError("AudioManager introuvable !");
        }
    }
}
