using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Réglages de mouvement")]
    public float moveDistance = 1f;
    public float moveSpeed = 10f;
    public float jumpHeight = 0.5f;
    public LayerMask obstacleLayer; // Définir ici le Layer "Obstacle"

    [Header("Effets Visuels")]
    public GameObject deathParticles; // Prefab de particules (Looping décoché)

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private bool isMoving = false;
    private float movePercent = 0f;

    void Update()
    {
        // On ne lit les touches que si le joueur n'est pas déjà en train de bouger
        if (!isMoving)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow)) AttemptMove(Vector3.forward);
            else if (Input.GetKeyDown(KeyCode.DownArrow)) AttemptMove(Vector3.back);
            else if (Input.GetKeyDown(KeyCode.LeftArrow)) AttemptMove(Vector3.left);
            else if (Input.GetKeyDown(KeyCode.RightArrow)) AttemptMove(Vector3.right);
        }
        else
        {
            AdvanceMovement();
        }
    }

    // 1. Vérification de la présence d'un mur avant de sauter
    void AttemptMove(Vector3 direction)
    {
        // Raycast envoyé depuis le centre du joueur vers la direction visée
        // On monte le point de départ de 0.5f pour éviter de toucher le sol
        if (!Physics.Raycast(transform.position + Vector3.up * 0.2f, direction, moveDistance, obstacleLayer))
        {
            StartMove(direction);
        }
        else
        {
            Debug.Log("Mouvement bloqué par un mur !");
        }
    }

    // 2. Initialisation des données de mouvement
    void StartMove(Vector3 direction)
    {
        startPosition = transform.position;
        targetPosition = startPosition + direction * moveDistance;
        movePercent = 0f;
        isMoving = true;

        // Oriente le personnage vers sa destination
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    // 3. Calcul de l'animation de saut et translation
    void AdvanceMovement()
    {
        movePercent += Time.deltaTime * moveSpeed;

        // Position horizontale (X et Z)
        Vector3 currentPos = Vector3.Lerp(startPosition, targetPosition, movePercent);

        // Position verticale (Y) pour l'effet de saut
        float yOffset = Mathf.Sin(movePercent * Mathf.PI) * jumpHeight;
        currentPos.y += yOffset;

        transform.position = currentPos;

        // Fin du mouvement
        if (movePercent >= 1f)
        {
            transform.position = targetPosition;
            isMoving = false;
        }
    }

    // 4. Gestion de la collision avec les ennemis
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ennemi"))
        {
            if (deathParticles != null)
            {
                // Apparition des particules
                GameObject effect = Instantiate(deathParticles, transform.position, Quaternion.identity);
                // Destruction de l'objet particules après 2 secondes pour nettoyer la scène
                Destroy(effect, 2f);
            }

            Debug.Log("Le joueur a été touché !");

            // On désactive le joueur au lieu de le détruire pour éviter les erreurs de caméra
            gameObject.SetActive(false);
        }
    }
<<<<<<< Updated upstream
}
=======

    /// <summary>
    /// Alternative avec OnTriggerEnter si vous utilisez des colliders en Trigger
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ennemi"))
        {
            HandleDeath();
        }
    }

    /// <summary>
    /// Gère la mort du joueur
    /// </summary>
    void HandleDeath()
    {
        if (deathParticles != null)
        {
            GameObject effect = Instantiate(deathParticles, transform.position, Quaternion.identity);
            Destroy(effect, 2f);
        }

        Debug.Log("Le joueur a été touché !");
        gameObject.SetActive(false);

       

                // Déclencher le Game Over
                if (GameOverManager.Instance != null)
                {
                    GameOverManager.Instance.ShowGameOver();
                }
            
        

    }

    /// <summary>
    /// Visualisation de la grille dans l'éditeur (optionnel)
    /// </summary>
    private void OnDrawGizmos()
    {
        // Dessiner un cercle à la position cible quand en mouvement
        if (isMoving)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(targetPosition, 0.3f);
        }

        // Dessiner la position actuelle sur la grille
        Gizmos.color = Color.blue;
        Vector3 snappedPos = new Vector3(
            Mathf.Round(transform.position.x),
            transform.position.y,
            Mathf.Round(transform.position.z)
        );
        Gizmos.DrawWireCube(snappedPos, Vector3.one * 0.9f);
    }
}
>>>>>>> Stashed changes
