using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Réglages de mouvement")]
    public float moveDistance = 1f; // Toujours 1 pour un système de grille
    public float moveSpeed = 10f;
    public float jumpHeight = 0.5f;
    public LayerMask obstacleLayer;

    [Header("Effets Visuels")]
    public GameObject deathParticles;

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private bool isMoving = false;
    private float movePercent = 0f;

    void Start()
    {
        // Forcer la position initiale à être alignée sur la grille
        SnapToGrid();
    }

    void Update()
    {
        if (!isMoving)
        {
            // Lecture des inputs uniquement quand le joueur ne bouge pas
            if (Input.GetKeyDown(KeyCode.UpArrow))
                AttemptMove(Vector3.forward);
            else if (Input.GetKeyDown(KeyCode.DownArrow))
                AttemptMove(Vector3.back);
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
                AttemptMove(Vector3.left);
            else if (Input.GetKeyDown(KeyCode.RightArrow))
                AttemptMove(Vector3.right);
        }
        else
        {
            AdvanceMovement();
        }
    }

    /// <summary>
    /// Force la position du joueur à s'aligner sur la grille (nombres entiers)
    /// </summary>
    void SnapToGrid()
    {
        Vector3 snappedPosition = new Vector3(
            Mathf.Round(transform.position.x),
            transform.position.y, // On ne snap pas le Y (hauteur)
            Mathf.Round(transform.position.z)
        );
        transform.position = snappedPosition;
    }

    /// <summary>
    /// Vérifie si le mouvement est possible avant de l'initier
    /// </summary>
    void AttemptMove(Vector3 direction)
    {
        // S'assurer que la direction est normalisée et ne contient que -1, 0 ou 1
        direction = new Vector3(
            Mathf.Round(direction.x),
            0, // Pas de mouvement vertical
            Mathf.Round(direction.z)
        );

        // Calculer la position cible exacte (toujours un nombre entier)
        Vector3 nextPosition = transform.position + direction * moveDistance;
        nextPosition = new Vector3(
            Mathf.Round(nextPosition.x),
            transform.position.y,
            Mathf.Round(nextPosition.z)
        );

        // Vérifier s'il y a un obstacle
        Vector3 rayOrigin = transform.position + Vector3.up * 0.2f;

        if (!Physics.Raycast(rayOrigin, direction, moveDistance, obstacleLayer))
        {
            StartMove(direction, nextPosition);
        }
        else
        {
            Debug.Log("Mouvement bloqué par un obstacle !");
        }
    }

    /// <summary>
    /// Initialise le mouvement vers la position cible
    /// </summary>
    void StartMove(Vector3 direction, Vector3 exactTarget)
    {
        startPosition = transform.position;
        targetPosition = exactTarget; // Utiliser la position exacte calculée
        movePercent = 0f;
        isMoving = true;

        // Orienter le personnage vers sa destination
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    /// <summary>
    /// Animation du saut et déplacement progressif
    /// </summary>
    void AdvanceMovement()
    {
        movePercent += Time.deltaTime * moveSpeed;

        // Limiter movePercent à 1 pour éviter les dépassements
        movePercent = Mathf.Clamp01(movePercent);

        // Position horizontale (X et Z) interpolée
        Vector3 currentPos = Vector3.Lerp(startPosition, targetPosition, movePercent);

        // Position verticale (Y) pour l'effet de saut
        float yOffset = Mathf.Sin(movePercent * Mathf.PI) * jumpHeight;
        currentPos.y = startPosition.y + yOffset;

        transform.position = currentPos;

        // Fin du mouvement
        if (movePercent >= 1f)
        {
            // Forcer la position exacte pour éviter tout décalage dû aux arrondis
            transform.position = new Vector3(
                Mathf.Round(targetPosition.x),
                startPosition.y,
                Mathf.Round(targetPosition.z)
            );

            isMoving = false;
        }
    }

    /// <summary>
    /// Gestion de la collision avec les ennemis
    /// </summary>
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ennemi"))
        {
            HandleDeath();
        }
    }

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
