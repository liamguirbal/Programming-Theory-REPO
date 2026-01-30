using UnityEngine;
using System.Collections.Generic;

public class TerrainGenerator : MonoBehaviour
{
    [Header("Configurations")]
    public GameObject terrainPrefab;
    public Transform player;
    public int initialAmount = 20;
    public float plotSize = 1f;
    public float distanceToSpawn = 25f;
    public float distanceToDestroy = 10f; // Distance derrière le joueur pour détruire

    private Vector3 currentSpawnPos = Vector3.zero;

    // Une Queue est parfaite pour "Premier entré, Premier sorti"
    private Queue<GameObject> activeTerrains = new Queue<GameObject>();

    void Start()
    {
        for (int i = 0; i < initialAmount; i++)
        {
            SpawnTerrain();
        }
    }

    void Update()
    {
        if (player == null) return;

        // 1. On crée devant
        while (currentSpawnPos.z - player.position.z < distanceToSpawn)
        {
            SpawnTerrain();
        }

        // 2. On nettoie derrière
        // Si le premier terrain de la liste est trop loin derrière le joueur
        if (activeTerrains.Count > 0)
        {
            GameObject oldestTerrain = activeTerrains.Peek(); // On regarde le plus vieux
            if (player.position.z - oldestTerrain.transform.position.z > distanceToDestroy)
            {
                // On le retire de la file et on le détruit
                Destroy(activeTerrains.Dequeue());
            }
        }
    }

    void SpawnTerrain()
    {
        GameObject go = Instantiate(terrainPrefab, currentSpawnPos, Quaternion.identity);
        activeTerrains.Enqueue(go); // On l'ajoute à la file
        currentSpawnPos.z += plotSize;
    }
}