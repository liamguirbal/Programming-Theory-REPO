using UnityEngine;

public class SpeedBoost : PowerUp
{
    [Header("Speed Boost")]
    public float speedMultiplier = 2f;
    public string indicatorName = "SpeedIndicator"; // ⭐ Nom exact

    private float originalSpeed;
    private GameObject indicator;

    protected override void ApplyEffect()
    {
        PlayerController controller = player.GetComponent<PlayerController>();
        if (controller != null)
        {
            originalSpeed = controller.moveSpeed;
            controller.moveSpeed *= speedMultiplier;
            Debug.Log($"Vitesse augmentée ! Nouvelle vitesse : {controller.moveSpeed}");
        }

        // Activer l'indicateur
        ShowIndicator(true);
    }

    protected override void RemoveEffect()
    {
        PlayerController controller = player.GetComponent<PlayerController>();
        if (controller != null)
        {
            controller.moveSpeed = originalSpeed;
            Debug.Log($"Vitesse revenue à la normale : {controller.moveSpeed}");
        }

        ShowIndicator(false);
    }

    // ⭐ Fonction pour afficher/cacher l'indicateur
    private void ShowIndicator(bool show)
    {
        if (indicator == null && player != null)
        {
            // ⭐ DEBUG : Afficher TOUS les enfants du Player
            Debug.Log($"Player a {player.transform.childCount} enfants :");
            for (int i = 0; i < player.transform.childCount; i++)
            {
                Debug.Log($"  Enfant {i} : '{player.transform.GetChild(i).name}'");
            }

            Transform indicatorTransform = player.transform.Find(indicatorName);
            if (indicatorTransform != null)
            {
                indicator = indicatorTransform.gameObject;
                Debug.Log($"SpeedIndicator trouvé !");
            }
            else
            {
                Debug.LogError($"SpeedIndicator introuvable ! Nom recherché : '{indicatorName}'");
            }
        }

        if (indicator != null)
        {
            indicator.SetActive(show);
            Debug.Log($"SpeedIndicator {(show ? "activé" : "désactivé")} !");
        }
    }

}
