using System.Collections.Generic;
using UnityEngine;

public class VehiculeSpawner : MonoBehaviour 
{
    [Header("Pool de Véhicules")]
    public List<GameObject> vehiclePrefabs;

    [Header("Paramètres de la Voie (Fixés au départ)")]
    private float roadSpeed;
    private float spawnInterval;

    void Start()
    {
        roadSpeed = Random.Range(7f, 12f);
        spawnInterval = Random.Range(0.8f, 1.5f);

        StartCoroutine(SpawnRoutine());
    }

    System.Collections.IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            
            if (vehiclePrefabs != null && vehiclePrefabs.Count > 0)
            {
               
                int randomIndex = Random.Range(0, vehiclePrefabs.Count);
                GameObject v = Instantiate(vehiclePrefabs[randomIndex], transform.position, transform.rotation);

                VehicleMovement moveScript = v.GetComponent<VehicleMovement>();
                if (moveScript != null)
                {
                    moveScript.speed = roadSpeed;
                }
            }
        }
    }
}