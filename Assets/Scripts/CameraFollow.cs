using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;        
    public float smoothSpeed = 5f;  // Vitesse de l'amorti 

    private Vector3 offset;         // Distance de décalage initiale

    void Start()
    {
       
        if (target != null)
        {
            offset = transform.position - target.position;
        }
    }

   
    void LateUpdate()
    {
        if (target == null) return;

        
        Vector3 desiredPosition = target.position + offset;

   
        desiredPosition.y = transform.position.y;


        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        transform.position = smoothedPosition;
    }
}