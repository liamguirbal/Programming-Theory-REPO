using UnityEngine;

public class ScoreMultiplier : PowerUp
{
    [Header("Score Multiplier")]
    public int multiplier = 2;
    public string indicatorName = "MultiplierIndicator";

    private GameObject indicator;

    protected override void ApplyEffect()
    {
        ScoreManager scoreManager = FindFirstObjectByType<ScoreManager>();
        if (scoreManager != null)
        {
            scoreManager.SetMultiplier(multiplier);
            Debug.Log($"Multiplicateur de score activé : x{multiplier}");
        }
        else
        {
            Debug.LogError("ScoreManager introuvable !");
        }

        ShowIndicator(true);
    }

    protected override void RemoveEffect()
    {
        ScoreManager scoreManager = FindFirstObjectByType<ScoreManager>();
        if (scoreManager != null)
        {
            scoreManager.SetMultiplier(1); // ⭐ Remettre à x1
            Debug.Log("Multiplicateur de score désactivé (x1)");
        }

        ShowIndicator(false);
    }

    // ⭐ S'assurer que RemoveEffect est appelé même si le GameObject est détruit
    private void OnDestroy()
    {
        // Remettre le multiplicateur à 1
        ScoreManager scoreManager = FindFirstObjectByType<ScoreManager>();
        if (scoreManager != null)
        {
            scoreManager.SetMultiplier(1);
            Debug.Log("ScoreMultiplier détruit, multiplicateur remis à x1");
        }

        FindAndDeactivateIndicator();
    }

    private void ShowIndicator(bool show)
    {
        if (indicator == null && player != null)
        {
            Transform indicatorTransform = player.transform.Find(indicatorName);
            if (indicatorTransform != null)
            {
                indicator = indicatorTransform.gameObject;
                Debug.Log("MultiplierIndicator trouvé !");
            }
        }

        if (indicator != null)
        {
            indicator.SetActive(show);
            Debug.Log($"MultiplierIndicator {(show ? "activé" : "désactivé")} !");
        }
    }

    // ⭐ Désactiver l'indicateur avec vérification
    private void FindAndDeactivateIndicator()
    {
        if (player != null)
        {
            Transform indicatorTransform = player.transform.Find(indicatorName);
            if (indicatorTransform != null)
            {
                indicatorTransform.gameObject.SetActive(false);
                Debug.Log("MultiplierIndicator désactivé !");
            }
        }
    }
}
