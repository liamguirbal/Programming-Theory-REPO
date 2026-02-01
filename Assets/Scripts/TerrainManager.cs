using UnityEngine;
using System.Collections.Generic;

public class TerrainManager : MonoBehaviour
{
    [Header("Configurations")]
    public Transform player;
    public int initialAmount = 20;
    public float plotSize = 1f;
    public float distanceToSpawn = 25f;
    public float distanceToDestroy = 10f;

    [Header("Prefabs de Terrains")]
    [Tooltip("Prefab de terrain normal (Forêt avec arbres)")]
    public GameObject terrainPrefab;

    [Tooltip("Prefab de terrain Route (avec VehiculeSpawner)")]
    public GameObject roadPrefab;

    [Tooltip("Prefab de transition vide (sans arbres/champignons)")]
    public GameObject transitionPrefab;

    [Header("Offsets de Position (si décalage visuel)")]
    [Tooltip("Offset de position pour le prefab de route")]
    public Vector3 roadOffset = Vector3.zero;

    [Tooltip("Offset de position pour le prefab de transition")]
    public Vector3 transitionOffset = Vector3.zero;

    [Header("Probabilités de Spawn")]
    [Tooltip("Chance qu'une route apparaisse (0-100%)")]
    [Range(0f, 100f)]
    public float roadSpawnChance = 15f;

    [Tooltip("Nombre de terrains normaux garantis au début du jeu")]
    [Range(0, 20)]
    public int safeStartTerrains = 5;

    [Tooltip("Longueur minimale d'une route (nombre de lignes)")]
    [Range(1, 10)]
    public int minRoadLength = 3;

    [Tooltip("Longueur maximale d'une route (nombre de lignes)")]
    [Range(1, 10)]
    public int maxRoadLength = 6;

    [Tooltip("Ajouter des lignes de transition vides avant/après la route")]
    public bool useTransitions = true;

    [Tooltip("Nombre de lignes de transition AVANT la route")]
    [Range(0, 5)]
    public int transitionsBeforeRoad = 1;

    [Tooltip("Nombre de lignes de transition APRÈS la route")]
    [Range(0, 5)]
    public int transitionsAfterRoad = 1;

    private Vector3 currentSpawnPos = Vector3.zero;
    private Queue<GameObject> activeTerrains = new Queue<GameObject>();
    private int terrainsSpawned = 0;

    private bool isSpawningRoad = false;
    private int roadLinesRemaining = 0;
    private int transitionsBeforeRemaining = 0;
    private int transitionsAfterRemaining = 0;

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

        while (currentSpawnPos.z - player.position.z < distanceToSpawn)
        {
            SpawnTerrain();
        }

        if (activeTerrains.Count > 0)
        {
            GameObject oldestTerrain = activeTerrains.Peek();
            if (player.position.z - oldestTerrain.transform.position.z > distanceToDestroy)
            {
                Destroy(activeTerrains.Dequeue());
            }
        }
    }

    void SpawnTerrain()
    {
        GameObject prefabToSpawn = null;
        Vector3 spawnOffset = Vector3.zero;

        // PRIORITÉ 1 : Transitions AVANT la route
        if (transitionsBeforeRemaining > 0)
        {
            if (useTransitions && transitionPrefab != null)
            {
                prefabToSpawn = transitionPrefab;
                spawnOffset = transitionOffset;
            }
            else
            {
                prefabToSpawn = terrainPrefab;
            }
            transitionsBeforeRemaining--;
        }
        // PRIORITÉ 2 : Transitions APRÈS la route
        else if (transitionsAfterRemaining > 0)
        {
            if (useTransitions && transitionPrefab != null)
            {
                prefabToSpawn = transitionPrefab;
                spawnOffset = transitionOffset;
            }
            else
            {
                prefabToSpawn = terrainPrefab;
            }
            transitionsAfterRemaining--;
        }
        // PRIORITÉ 3 : Continuer la route en cours
        else if (isSpawningRoad && roadLinesRemaining > 0)
        {
            prefabToSpawn = roadPrefab;
            spawnOffset = roadOffset;
            roadLinesRemaining--;

            if (roadLinesRemaining == 0)
            {
                isSpawningRoad = false;
                transitionsAfterRemaining = transitionsAfterRoad;
            }
        }
        // PRIORITÉ 4 : Décider si on commence une nouvelle route OU terrain normal
        else
        {
            if (terrainsSpawned < safeStartTerrains)
            {
                prefabToSpawn = terrainPrefab;
            }
            else
            {
                float randomValue = Random.Range(0f, 100f);

                if (randomValue < roadSpawnChance && roadPrefab != null)
                {
                    transitionsBeforeRemaining = transitionsBeforeRoad;

                    if (transitionsBeforeRemaining > 0)
                    {
                        if (useTransitions && transitionPrefab != null)
                        {
                            prefabToSpawn = transitionPrefab;
                            spawnOffset = transitionOffset;
                        }
                        else
                        {
                            prefabToSpawn = terrainPrefab;
                        }
                        transitionsBeforeRemaining--;
                    }
                    else
                    {
                        prefabToSpawn = roadPrefab;
                        spawnOffset = roadOffset;
                        roadLinesRemaining--;
                    }

                    isSpawningRoad = true;
                    roadLinesRemaining = Random.Range(minRoadLength, maxRoadLength + 1);
                }
                else
                {
                    prefabToSpawn = terrainPrefab;
                }
            }
        }

        if (prefabToSpawn == null)
        {
            prefabToSpawn = terrainPrefab;
        }

        // Spawner le terrain avec l'offset approprié
        GameObject go = Instantiate(prefabToSpawn, currentSpawnPos + spawnOffset, Quaternion.identity);
        activeTerrains.Enqueue(go);
        currentSpawnPos.z += plotSize;
        terrainsSpawned++;
    }
}
