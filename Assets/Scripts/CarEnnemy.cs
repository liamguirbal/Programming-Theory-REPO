using UnityEngine;

public class VehicleMovement : MonoBehaviour
{
    
    [HideInInspector]
    public float speed;

    [Header("Réglages")]
    public float lifeTime = 10f; 

    void Start()
    {
        
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
       
        transform.Translate(Vector3.right * speed * Time.deltaTime, Space.World);
    }
}