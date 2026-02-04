using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public float speed = 2f;
    public int direction = 1; // 1 = droite, -1 = gauche
    public float minX = -20f;
    public float maxX = 20f;

    void Update()
    {
     
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

    
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }
}
