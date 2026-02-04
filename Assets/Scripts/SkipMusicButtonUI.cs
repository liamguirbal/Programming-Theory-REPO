using UnityEngine;
using UnityEngine.UI;

public class SkipMusicButtonUI : MonoBehaviour
{
    void Start()
    {
      
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(SkipMusic);
        }
    }

   
    public void SkipMusic()
    {
        // Trouver l'AudioManager 
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
