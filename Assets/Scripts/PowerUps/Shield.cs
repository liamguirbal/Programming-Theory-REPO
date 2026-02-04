using UnityEngine;

public class Shield : PowerUp
{
    [Header("Shield - Effets Visuels")]
    public GameObject shieldHitParticles;
    public string indicatorName = "ShieldIndicator";

    private GameObject indicator;

    protected override void ApplyEffect()
    {
        player.hasShield = true;
        player.activeShield = this;

        Debug.Log("Shield activé !");

        ShowIndicator(true);
    }

    protected override void RemoveEffect()
    {
        if (player != null)
        {
            player.hasShield = false;
            player.activeShield = null;
            Debug.Log("Shield expiré !");
        }

        ShowIndicator(false);
    }

    public void OnShieldHit(Vector3 hitPosition)
    {
        Debug.Log("Shield a bloqué une attaque !");

        // ⭐ Jouer le son du shield
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayShieldBlock();
        }

        if (shieldHitParticles != null)
        {
            GameObject particles = Instantiate(shieldHitParticles, hitPosition, Quaternion.identity);
            Destroy(particles, 2f);
        }

        FindAndDeactivateIndicator();

        if (player != null)
        {
            player.hasShield = false;
            player.activeShield = null;
        }

        CancelInvoke();
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        FindAndDeactivateIndicator();

        if (player != null)
        {
            player.hasShield = false;
            player.activeShield = null;
        }
    }

    private void ShowIndicator(bool show)
    {
        if (indicator == null && player != null)
        {
            Transform indicatorTransform = player.transform.Find(indicatorName);
            if (indicatorTransform != null)
            {
                indicator = indicatorTransform.gameObject;
                Debug.Log("ShieldIndicator trouvé !");
            }
        }

        if (indicator != null)
        {
            indicator.SetActive(show);
            Debug.Log($"ShieldIndicator {(show ? "activé" : "désactivé")} !");
        }
    }

    private void FindAndDeactivateIndicator()
    {
        if (player != null)
        {
            Transform indicatorTransform = player.transform.Find(indicatorName);
            if (indicatorTransform != null)
            {
                indicatorTransform.gameObject.SetActive(false);
                Debug.Log("ShieldIndicator désactivé !");
            }
        }
    }
}
