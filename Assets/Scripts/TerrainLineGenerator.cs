using System.Collections.Generic;
using UnityEngine;

public class TerrainLineGenerator : MonoBehaviour
{
    [Header("Configuration du Terrain")]
    [Tooltip("Type de terrain : Grass, Road, River")]
    public TerrainType terrainType = TerrainType.Grass;

    [Header("Obstacles Statiques")]
    [Tooltip("Obstacles qui bloquent le passage (arbres, rochers, murs)")]
    public GameObject[] obstaclePrefabs;

    [Tooltip("Obstacles qui doivent tourner par paliers de 90° (barrières, panneaux)")]
    public GameObject[] obstacles90DegreeRotation;

    [Tooltip("Densité des obstacles (0-100%)")]
    [Range(0f, 100f)]
    public float obstacleDensity = 30f;

    [Header("Décorations au Sol")]
    [Tooltip("Décorations qui ne bloquent pas (herbes, fleurs, cailloux)")]
    public GameObject[] decorationPrefabs;

    [Tooltip("Décorations à rotation 90° uniquement (pierres plates, planches)")]
    public GameObject[] decorations90DegreeRotation;

    [Tooltip("Densité des décorations (0-100%)")]
    [Range(0f, 100f)]
    public float decorationDensity = 20f;

    [Tooltip("Variation de position sur l'axe Z pour effet organique")]
    [Range(0f, 1f)]
    public float decorationZSpread = 0.4f;

    [Tooltip("Activer la variation d'échelle des décorations")]
    public bool randomizeScale = true;

    [Tooltip("Échelle minimale des décorations")]
    [Range(0.5f, 1.5f)]
    public float minScale = 0.8f;

    [Tooltip("Échelle maximale des décorations")]
    [Range(0.5f, 1.5f)]
    public float maxScale = 1.2f;

    [Tooltip("Rotation aléatoire uniquement sur l'axe Y")]
    public bool randomYRotationOnly = true;

    [Header("⭐ POWER-UPS ⭐")]
    [Tooltip("Préfabs des power-ups (Shield, Speed, Multiplier, etc.)")]
    public GameObject[] powerUpPrefabs;

    [Tooltip("Chance de spawn d'un power-up par ligne (0-100%)")]
    [Range(0f, 100f)]
    public float powerUpSpawnChance = 15f;

    [Tooltip("Hauteur de spawn des power-ups (Y)")]
    public float powerUpHeight = 0.5f;

    [Header("Paramètres de la Grille")]
    [Tooltip("Largeur totale du terrain (en unités)")]
    public int terrainWidth = 22;

    [Tooltip("Position de début de la route (axe X) - pour éviter les décos sur la route")]
    public int roadStartX = -3;

    [Tooltip("Largeur de la route (nombre de cases) - pour éviter les décos")]
    public int roadWidth = 6;

    [Tooltip("Nombre minimum de cases libres pour les terrains Grass")]
    [Range(1, 10)]
    public int minFreeTiles = 3;

    [Header("Plateformes Mobiles (Rondins)")]
    [Tooltip("Plateformes sur lesquelles le joueur peut monter")]
    public GameObject[] platformPrefabs;

    [Tooltip("Nombre de plateformes à spawner")]
    [Range(0, 10)]
    public int platformCount = 3;

    [Tooltip("Vitesse des plateformes")]
    public float platformSpeed = 2f;

    [Tooltip("Direction du mouvement (-1 = gauche, 1 = droite)")]
    public int platformDirection = 1;

    [Tooltip("Tailles possibles des plateformes (en unités de grille)")]
    public int[] platformSizes = new int[] { 2, 3, 4 };

    [Tooltip("Espacement entre les plateformes")]
    public float platformSpacing = 6f;

    private HashSet<int> occupiedPositions = new HashSet<int>();

    void Start()
    {
        GenerateTerrain();
    }

    void GenerateTerrain()
    {
        // Les décorations pour tous les types de terrain
        SpawnDecorations();

        // Les obstacles seulement pour Grass
        if (terrainType == TerrainType.Grass)
        {
            SpawnStaticObstacles();
        }

        // Les plateformes seulement pour River(pas fait pour l'instant)
        if (terrainType == TerrainType.River && platformPrefabs.Length > 0)
        {
            SpawnPlatforms();
        }

        // ⭐ NOUVEAU : Spawner un power-up aléatoire
        SpawnPowerUp();
    }

    void SpawnDecorations()
    {
        if (decorationPrefabs.Length == 0 && decorations90DegreeRotation.Length == 0) return;

        int halfWidth = terrainWidth / 2;

        int decorationCount = Mathf.RoundToInt((terrainWidth * decorationDensity) / 100f);

        for (int i = 0; i < decorationCount; i++)
        {
            float randomX = UnityEngine.Random.Range(-halfWidth, halfWidth + 1f);
            float randomZ = UnityEngine.Random.Range(-decorationZSpread, decorationZSpread);

            if (terrainType == TerrainType.Road)
            {
                if (randomX >= roadStartX && randomX < roadStartX + roadWidth)
                {
                    continue;
                }
            }

            GameObject decorationPrefab = null;
            bool use90Degree = false;

            int totalDecoCount = decorationPrefabs.Length + decorations90DegreeRotation.Length;
            int randomChoice = UnityEngine.Random.Range(0, totalDecoCount);

            if (randomChoice < decorationPrefabs.Length && decorationPrefabs.Length > 0)
            {
                decorationPrefab = decorationPrefabs[randomChoice];
                use90Degree = false;
            }
            else if (decorations90DegreeRotation.Length > 0)
            {
                int index = randomChoice - decorationPrefabs.Length;
                decorationPrefab = decorations90DegreeRotation[index];
                use90Degree = true;
            }

            if (decorationPrefab != null)
            {
                Vector3 spawnPosition = transform.position + new Vector3(randomX, 0, randomZ);
                Quaternion rotation = use90Degree ? GetRandom90DegreeRotation() : GetRandomRotation();

                GameObject decoration = Instantiate(decorationPrefab, spawnPosition, rotation);
                decoration.transform.SetParent(this.transform);

                if (randomizeScale)
                {
                    float randomScale = UnityEngine.Random.Range(minScale, maxScale);
                    decoration.transform.localScale *= randomScale;
                }
            }
        }
    }

    void SpawnStaticObstacles()
    {
        if (obstaclePrefabs.Length == 0 && obstacles90DegreeRotation.Length == 0) return;

        int halfWidth = terrainWidth / 2;

        for (int x = -halfWidth; x <= halfWidth; x++)
        {
            float randomValue = UnityEngine.Random.Range(0f, 100f);

            if (randomValue <= obstacleDensity)
            {
                if (occupiedPositions.Count < terrainWidth - minFreeTiles)
                {
                    GameObject obstaclePrefab = null;
                    bool use90Degree = false;

                    int totalObstacleCount = obstaclePrefabs.Length + obstacles90DegreeRotation.Length;
                    int randomChoice = UnityEngine.Random.Range(0, totalObstacleCount);

                    if (randomChoice < obstaclePrefabs.Length && obstaclePrefabs.Length > 0)
                    {
                        obstaclePrefab = obstaclePrefabs[randomChoice];
                        use90Degree = false;
                    }
                    else if (obstacles90DegreeRotation.Length > 0)
                    {
                        int index = randomChoice - obstaclePrefabs.Length;
                        obstaclePrefab = obstacles90DegreeRotation[index];
                        use90Degree = true;
                    }

                    if (obstaclePrefab != null)
                    {
                        Vector3 spawnPosition = transform.position + new Vector3(x, 0, 0);
                        Quaternion rotation = use90Degree ? GetRandom90DegreeRotation() : GetRandomRotation();

                        GameObject obstacle = Instantiate(obstaclePrefab, spawnPosition, rotation);
                        obstacle.transform.SetParent(this.transform);

                        occupiedPositions.Add(x);
                    }
                }
            }
        }
    }

    void SpawnPlatforms()
    {
        for (int i = 0; i < platformCount; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, platformPrefabs.Length);
            GameObject platformPrefab = platformPrefabs[randomIndex];

            float startX = (platformDirection > 0 ? -terrainWidth : terrainWidth) + (i * platformSpacing * platformDirection);

            Vector3 spawnPosition = transform.position + new Vector3(startX, 0, 0);

            Quaternion rotation = platformDirection > 0 ? Quaternion.Euler(0, 90, 0) : Quaternion.Euler(0, -90, 0);

            GameObject platform = Instantiate(platformPrefab, spawnPosition, rotation);
            platform.transform.SetParent(this.transform);

            int size = platformSizes[UnityEngine.Random.Range(0, platformSizes.Length)];
            Vector3 currentScale = platform.transform.localScale;
            platform.transform.localScale = new Vector3(size, currentScale.y, currentScale.z);

            MovingPlatform moveScript = platform.AddComponent<MovingPlatform>();
            moveScript.speed = platformSpeed;
            moveScript.direction = platformDirection;
            moveScript.minX = transform.position.x - terrainWidth * 2;
            moveScript.maxX = transform.position.x + terrainWidth * 2;
        }
    }

    // Spawner un power-up aléatoire
    void SpawnPowerUp()
    {
        // Si pas de power-ups configurés, on sort
        if (powerUpPrefabs == null || powerUpPrefabs.Length == 0) return;

        // Test de chance de spawn
        float randomChance = UnityEngine.Random.Range(0f, 100f);
        if (randomChance > powerUpSpawnChance) return; 

       
        int randomIndex = UnityEngine.Random.Range(0, powerUpPrefabs.Length);
        GameObject powerUpPrefab = powerUpPrefabs[randomIndex];

        // Position aléatoire sur la ligne
        int halfWidth = terrainWidth / 2;
        float randomX = UnityEngine.Random.Range(-halfWidth + 1, halfWidth);

        // Pour les routes, spawner sur la route 
        if (terrainType == TerrainType.Road)
        {
            randomX = UnityEngine.Random.Range(roadStartX + 1, roadStartX + roadWidth - 1);
        }


        Vector3 spawnPosition = transform.position + new Vector3(randomX, powerUpHeight, 0);

        // Spawner le power-up
        GameObject powerUp = Instantiate(powerUpPrefab, spawnPosition, Quaternion.identity);
        powerUp.transform.SetParent(this.transform);

        Debug.Log($"Power-up spawné : {powerUpPrefab.name} à la position {spawnPosition}");
    }

    Quaternion GetRandomRotation()
    {
        if (randomYRotationOnly)
        {
            float randomY = UnityEngine.Random.Range(0f, 360f);
            return Quaternion.Euler(0, randomY, 0);
        }
        else
        {
            return UnityEngine.Random.rotation;
        }
    }

    Quaternion GetRandom90DegreeRotation()
    {
        int randomRotation = UnityEngine.Random.Range(0, 4) * 90;
        return Quaternion.Euler(0, randomRotation, 0);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        int halfWidth = terrainWidth / 2;
        Vector3 leftPos = transform.position + new Vector3(-halfWidth, 0, 0);
        Vector3 rightPos = transform.position + new Vector3(halfWidth, 0, 0);
        Gizmos.DrawLine(leftPos + Vector3.back * 2, leftPos + Vector3.forward * 2);
        Gizmos.DrawLine(rightPos + Vector3.back * 2, rightPos + Vector3.forward * 2);

        if (terrainType == TerrainType.Road)
        {
            Gizmos.color = Color.red;
            Vector3 roadLeft = transform.position + new Vector3(roadStartX, 0.1f, 0);
            Vector3 roadRight = transform.position + new Vector3(roadStartX + roadWidth, 0.1f, 0);
            Gizmos.DrawLine(roadLeft + Vector3.back * 2, roadLeft + Vector3.forward * 2);
            Gizmos.DrawLine(roadRight + Vector3.back * 2, roadRight + Vector3.forward * 2);
        }
    }
}

public enum TerrainType
{
    Grass,
    Road,
    River
}
