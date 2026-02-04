using UnityEngine;

public class SpeedBoost : PowerUp
{
    [Header("Speed Boost")]
    public float speedMultiplier = 2f;
    public string indicatorName = "SpeedIndicator";

    private float originalSpeed;
    private GameObject indicator;
    private PlayerController controller;

    protected override void ApplyEffect()
    {
        controller = player.GetComponent<PlayerController>();
        if (controller != null)
        {
            originalSpeed = controller.moveSpeed;
            controller.moveSpeed *= speedMultiplier;
            Debug.Log($"Vitesse augmentée ! Nouvelle vitesse : {controller.moveSpeed}");
        }

        ShowIndicator(true);
    }

    protected override void RemoveEffect()
    {
        // ⭐ FIX : Vérifier que le controller existe toujours
        if (controller != null)
        {
            controller.moveSpeed = originalSpeed;
            Debug.Log($"Vitesse revenue à la normale : {controller.moveSpeed}");
        }

        ShowIndicator(false);
    }

    // ⭐ AJOUTÉ : S'assurer que RemoveEffect est appelé avant destruction
    private void OnDestroy()
    {
        // Si le GameObject est détruit, remettre la vitesse normale
        if (controller != null && controller.moveSpeed != originalSpeed)
        {
            controller.moveSpeed = originalSpeed;
            Debug.Log("SpeedBoost détruit, vitesse réinitialisée via OnDestroy");
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
