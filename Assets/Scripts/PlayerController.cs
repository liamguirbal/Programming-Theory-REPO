using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Réglages de mouvement")]
    public float moveDistance = 1f;
    public float moveSpeed = 10f;
    public float jumpHeight = 0.5f;
    public LayerMask obstacleLayer;

    [Header("Effets Visuels")]
    public GameObject deathParticles;

    [Header("Game Over")]
    public GameOverManager gameOverManager;

    [Header("Power-Ups")]
    public bool hasShield = false;
    public Shield activeShield;

    [Header("Score")]
    public int pointsPerMove = 10;

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private bool isMoving = false;
    private float movePercent = 0f;
    private Vector3 lastMoveDirection; // ⭐ AJOUTÉ : Mémoriser la direction

    void Update()
    {
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

    void AttemptMove(Vector3 direction)
    {
        if (!Physics.Raycast(transform.position + Vector3.up * 0.2f, direction, moveDistance, obstacleLayer))
        {
            StartMove(direction);
        }
        else
        {
            Debug.Log("Mouvement bloqué par un mur !");
        }
    }

    void StartMove(Vector3 direction)
    {
        startPosition = transform.position;
        targetPosition = startPosition + direction * moveDistance;
        movePercent = 0f;
        isMoving = true;
        lastMoveDirection = direction; // ⭐ AJOUTÉ : Mémoriser la direction

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    void AdvanceMovement()
    {
        movePercent += Time.deltaTime * moveSpeed;

        Vector3 currentPos = Vector3.Lerp(startPosition, targetPosition, movePercent);
        float yOffset = Mathf.Sin(movePercent * Mathf.PI) * jumpHeight;
        currentPos.y += yOffset;

        transform.position = currentPos;

        if (movePercent >= 1f)
        {
            transform.position = targetPosition;
            isMoving = false;

            // ⭐ CORRIGÉ : Ajouter des points SEULEMENT si c'est vers l'avant
            if (lastMoveDirection == Vector3.forward)
            {
                AddScoreForMove();
            }
        }
    }

    void AddScoreForMove()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScore(pointsPerMove);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ennemi"))
        {
            Debug.Log($"Collision avec ennemi ! hasShield = {hasShield}");

            if (hasShield)
            {
                hasShield = false;
                Debug.Log("Bouclier a absorbé l'attaque !");

                if (activeShield != null)
                {
                    activeShield.OnShieldHit(collision.contacts[0].point);
                    activeShield = null;
                }

                Destroy(collision.gameObject);
                return;
            }

            // Code de mort normal...
            if (deathParticles != null)
            {
                GameObject effect = Instantiate(deathParticles, transform.position, Quaternion.identity);
                Destroy(effect, 2f);
            }

            // Jouer le son de mort
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayDeath();
            }

            Debug.Log("Le joueur devrait mourir maintenant !");

            if (gameOverManager != null)
            {
                gameOverManager.ShowGameOver();
            }

            gameObject.SetActive(false);

        }
    }

    public void SetActiveShield(Shield shield)
    {
        activeShield = shield;
    }
}
