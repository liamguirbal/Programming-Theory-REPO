using UnityEngine;

public class Shield : PowerUp
{
    [Header("Shield - Effets Visuels")]
    public GameObject shieldHitParticles; // Particules quand le shield bloque

    private GameObject shieldIndicator; // L'indicateur visuel du shield

    protected override void ApplyEffect()
    {
        player.hasShield = true;
        player.activeShield = this;

        Debug.Log("Shield activé !");

        // Trouver et activer ShieldIndicator
        if (shieldIndicator == null)
        {
            shieldIndicator = player.transform.Find("ShieldIndicator")?.gameObject;
        }

        if (shieldIndicator != null)
        {
            shieldIndicator.SetActive(true);
            Debug.Log("ShieldIndicator activé !");
        }
        else
        {
            Debug.LogError("ShieldIndicator introuvable en tant qu'enfant du Player !");
        }
    }

    protected override void RemoveEffect()
    {
        if (player != null)
        {
            player.hasShield = false;
            Debug.Log("Shield expiré !");
        }

        // Désactiver ShieldIndicator
        if (shieldIndicator != null)
        {
            shieldIndicator.SetActive(false);
        }
    }

    public void OnShieldHit(Vector3 hitPosition)
    {
        Debug.Log("Shield a bloqué une attaque !");

        // Spawner les particules
        if (shieldHitParticles != null)
        {
            GameObject particles = Instantiate(shieldHitParticles, hitPosition, Quaternion.identity);
            Destroy(particles, 2f);
        }

        // Désactiver ShieldIndicator
        if (shieldIndicator != null)
        {
            shieldIndicator.SetActive(false);
        }

        // Retirer le shield
        CancelInvoke();
        RemoveEffect();
        Destroy(gameObject);
    }
}
