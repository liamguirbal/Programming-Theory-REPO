using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;        // Glisse ton joueur ici
    public float smoothSpeed = 5f;  // Vitesse de l'amorti (plus c'est bas, plus c'est fluide)

    private Vector3 offset;         // Distance de décalage initiale

    void Start()
    {
        // On calcule la distance entre la caméra et le joueur au lancement du jeu
        if (target != null)
        {
            offset = transform.position - target.position;
        }
    }

    // On utilise LateUpdate pour que la caméra bouge APRÈS le joueur
    void LateUpdate()
    {
        if (target == null) return;

        // 1. On calcule la position désirée (Position du joueur + décalage)
        Vector3 desiredPosition = target.position + offset;

        // 2. MAGIE : On force la position désirée à garder la hauteur initiale de la caméra
        // Cela empêche la caméra de monter/descendre pendant les sauts
        desiredPosition.y = transform.position.y;

        // 3. On applique un mouvement fluide (Lerp)
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        transform.position = smoothedPosition;
    }
}