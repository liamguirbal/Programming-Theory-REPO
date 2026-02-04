using UnityEngine;

public abstract class PowerUp : MonoBehaviour
{
    [Header("Paramètres communs")]
    public float duration = 5f; // Durée de l'effet
    public GameObject pickupEffect; // Particules à l'activation

    protected PlayerController player;
    protected bool isActive = false;

    // POLYMORPHISME : Chaque power-up définit son propre effet
    protected abstract void ApplyEffect();
    protected abstract void RemoveEffect();

    // ENCAPSULATION : Logique commune cachée
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.GetComponent<PlayerController>();

            if (player != null)
            {
                Activate();
            }
        }
    }

    // ABSTRACTION : Interface simple
    public void Activate()
    {
        if (isActive) return;

        isActive = true;

        // Effet visuel
        if (pickupEffect != null)
        {
            GameObject effect = Instantiate(pickupEffect, transform.position, Quaternion.identity);
            Destroy(effect, 2f);
        }

        // ⭐ AJOUTÉ : Jouer le son de pick-up
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayPowerUpPickup();
        }

        ApplyEffect(); // Appel polymorphe

        // Désactiver l'objet visuel
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<Collider>().enabled = false;

        // Retirer l'effet après la durée
        Invoke(nameof(Deactivate), duration);
    }

    protected virtual void Deactivate()
    {
        RemoveEffect();
        Destroy(gameObject);
    }
}
