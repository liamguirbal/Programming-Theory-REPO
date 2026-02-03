using UnityEngine;

public class ScoreMultiplier : PowerUp
{
    [Header("Score Multiplier")]
    public int multiplier = 2;
    public string indicatorName = "MultiplierIndicator";

    private GameObject indicator;

    protected override void ApplyEffect()
    {
        // ⭐ CORRIGÉ : FindFirstObjectByType au lieu de FindObjectOfType
        ScoreManager scoreManager = FindFirstObjectByType<ScoreManager>();
        if (scoreManager != null)
        {
            scoreManager.SetMultiplier(multiplier);
            Debug.Log($"Multiplicateur de score activé : x{multiplier}");
        }

        ShowIndicator(true);
    }

    protected override void RemoveEffect()
    {
        // ⭐ CORRIGÉ
        ScoreManager scoreManager = FindFirstObjectByType<ScoreManager>();
        if (scoreManager != null)
        {
            scoreManager.SetMultiplier(1);
            Debug.Log("Multiplicateur de score désactivé");
        }

        ShowIndicator(false);
    }

    private void ShowIndicator(bool show)
    {
        if (indicator == null && player != null)
        {
            Transform indicatorTransform = player.transform.Find(indicatorName);
            if (indicatorTransform != null)
            {
                indicator = indicatorTransform.gameObject;
            }
        }

        if (indicator != null)
        {
            indicator.SetActive(show);
        }
    }
}
