using UnityEngine;

public class VehicleMovement : MonoBehaviour
{
    // On ne met pas de valeur ici, c'est le Spawner qui va la remplir
    [HideInInspector]
    public float speed;

    [Header("Réglages")]
    public float lifeTime = 10f; // Temps avant destruction pour ne pas ramer

    void Start()
    {
        // On détruit l'objet après X secondes
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        // On avance sur l'axe X (car tu as dit vouloir du -X vers X)
        // Space.World permet d'ignorer la rotation de l'objet
        transform.Translate(Vector3.right * speed * Time.deltaTime, Space.World);
    }
}