using UnityEngine;

public class MovingEnemy : MonoBehaviour
{
    public float speed = 3f;
    public int direction = 1; 
    public bool moveOnZAxis = true; // true = se déplace sur Z, false = sur X

   
    public float minZ = -30f;
    public float maxZ = 30f;

   
    public float minX = -30f;
    public float maxX = 30f;

    void Update()
    {
        if (moveOnZAxis)
        {
            // Déplacement sur l'axe Z (profondeur)
            transform.Translate(Vector3.forward * direction * speed * Time.deltaTime, Space.World);

            // Téléportation
            if (direction > 0 && transform.position.z > maxZ)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, minZ);
            }
            else if (direction < 0 && transform.position.z < minZ)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, maxZ);
            }
        }
        else
        {
            // Déplacement sur l'axe X (largeur)
            transform.Translate(Vector3.right * direction * speed * Time.deltaTime, Space.World);

            if (direction > 0 && transform.position.x > maxX)
            {
                transform.position = new Vector3(minX, transform.position.y, transform.position.z);
            }
            else if (direction < 0 && transform.position.x < minX)
            {
                transform.position = new Vector3(maxX, transform.position.y, transform.position.z);
            }
        }
    }
}
