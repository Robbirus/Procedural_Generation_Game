using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralTerrainGenerator : MonoBehaviour
{
    // ─────────────────────────────────────────────
    //  GRID SETTINGS
    // ─────────────────────────────────────────────
    [Header("Grid")]
    [Tooltip("Number of tiles on the X-axis.")]
    [SerializeField] private int gridWidth = 20;

    [Tooltip("Number of tiles on the Z-axis.")]
    [SerializeField] private int gridHeight = 20;

    [Tooltip("Size of a tile in Unity units (ground prefabs are ~1 unit).")]
    [SerializeField] private float tileSize = 1f;

    // ─────────────────────────────────────────────
    //  NOISE SETTINGS
    // ─────────────────────────────────────────────
    [Header("Noise - Terrain")]
    [Tooltip("Random seed. Change it to get a different terrain.")]
    [SerializeField] private int seed = 42;

    [Tooltip("Noise scale: larger = more smoothed relief.")]
    [SerializeField] private float noiseScale = 8f;

    [Tooltip("Noise offset X (exploration of the map).")]
    [SerializeField] private float offsetX = 0f;

    [Tooltip("Noise Z-offset.")]
    [SerializeField] private float offsetZ = 0f;

    [Header("Noise - Decoration")]
    [SerializeField] private float decoNoiseScale = 5f;
    [SerializeField] private float decoOffsetX = 100f;
    [SerializeField] private float decoOffsetZ = 100f;

    // ─────────────────────────────────────────────
    //  BIOME THRESHOLDS
    // ─────────────────────────────────────────────
    [Header("Biome thresholds (noise value 0->1)")]
    [Tooltip("Below: river / water.")]
    [Range(0f, 1f)][SerializeField] private float riverThreshold = 0.25f;

    [Tooltip("Between riverThreshold and pathThreshold: Path.")]
    [Range(0f, 1f)][SerializeField] private float pathThreshold = 0.40f;

    [Tooltip("Above: grass (main area).")]
    [Range(0f, 1f)][SerializeField] private float grassThreshold = 0.40f;

    // ─────────────────────────────────────────────
    //  DECORATION THRESHOLDS
    // ─────────────────────────────────────────────
    [Header("Decoration thresholds")]
    [Tooltip("Probability of a tree appearing on a grass box (0->1).")]
    [Range(0f, 1f)][SerializeField] private float treeDensity = 0.4f;

    [Tooltip("Probability of a rock appearing (0->1).")]
    [Range(0f, 1f)][SerializeField] private float rockDensity = 0.2f;

    [Tooltip("Probability of a plant appearing (0->1).")]
    [Range(0f, 1f)][SerializeField] private float plantDensity = 0.3f;

    // ─────────────────────────────────────────────
    //  PREFABS – SOLS
    // ─────────────────────────────────────────────
    [Header("Prefabs - Grass Ground")]
    [Tooltip("Ex : ground_grass.prefab")]
    [SerializeField] private List<GameObject> grassPrefabs = new List<GameObject>();

    [Header("Prefabs - Path")]
    [Tooltip("Ex : ground_pathOpen.prefab, ground_pathStraight.prefab, ground_pathTile.prefab")]
    [SerializeField] private List<GameObject> pathPrefabs = new List<GameObject>();

    [Header("Prefabs – River")]
    [Tooltip("Ex : ground_riverOpen.prefab, ground_riverStraight.prefab, ground_riverTile.prefab")]
    [SerializeField] private List<GameObject> riverPrefabs = new List<GameObject>();

    // ─────────────────────────────────────────────
    //  PREFABS - DÉCORATIONS
    // ─────────────────────────────────────────────
    [Header("Prefabs - Trees")]
    [Tooltip("Ex : tree_default.prefab, tree_oak.prefab, tree_pine*.prefab…")]
    [SerializeField] private List<GameObject> treePrefabs = new List<GameObject>();

    [Header("Prefabs – Rocks")]
    [Tooltip("Ex : rock_largeA.prefab, rock_smallA.prefab, stone_largeA.prefab…")]
    [SerializeField] private List<GameObject> rockPrefabs = new List<GameObject>();

    [Header("Prefabs – Plants / Flowers")]
    [Tooltip("Ex : plant_bush.prefab, flower_redA.prefab, grass.prefab…")]
    [SerializeField] private List<GameObject> plantPrefabs = new List<GameObject>();

    [Header("Prefabs – Stumps / Logs")]
    [Tooltip("Ex : stump_round.prefab, log.prefab (placés rarement sur l'herbe)")]
    [SerializeField] private List<GameObject> stumpPrefabs = new List<GameObject>();

    // ─────────────────────────────────────────────
    //  OPTIONS VISUELLES
    // ─────────────────────────────────────────────
    [Header("Random Y rotation")]
    [Tooltip("If true, each decoration rotates randomly around Y for the variety.")]
    [SerializeField] private bool randomYRotation = true;

    [Header("Variation d'échelle")]
    [Tooltip("Random scale variation applied to decorations (e.g., 0.1 -> +/-10%).")]
    [Range(0f, 0.5f)]
    [SerializeField] private float scaleVariation = 0.15f;

    // ─────────────────────────────────────────────
    //  INTERNAL STATE
    // ─────────────────────────────────────────────
    private List<GameObject> spawnedObjects = new List<GameObject>();
    private System.Random rng;

    // ─────────────────────────────────────────────
    //  PUBLIC ENTRANCE
    // ─────────────────────────────────────────────

    private void Start()
    {
        Generate();
    }

    // <summary>Calls this method from a UI button to re-generate. </summary>
    public void Generate()
    {
        Clear();
        StartCoroutine(GenerateCoroutine());
    }

    // <summary>Deletes all generated objects. </summary>
    public void Clear()
    {
        StopAllCoroutines();
        foreach (var go in spawnedObjects)
            if (go != null) Destroy(go);
        spawnedObjects.Clear();
    }

    // ─────────────────────────────────────────────
    //  GENERATION COROUTINE
    // ─────────────────────────────────────────────

    private IEnumerator GenerateCoroutine()
    {
        rng = new System.Random(seed);

        // Calcule un offset basé sur la graine pour varier les deux cartes
        float seedOffX = (float)(rng.NextDouble() * 10000);
        float seedOffZ = (float)(rng.NextDouble() * 10000);

        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                // ── Échantillonnage du bruit ──────────────────────────
                float terrainValue = SampleNoise(
                    x, z, noiseScale, offsetX + seedOffX, offsetZ + seedOffZ);

                float decoValue = SampleNoise(
                    x, z, decoNoiseScale, decoOffsetX + seedOffX, decoOffsetZ + seedOffZ);

                // ── Position mondiale de la tuile ──────────────────────────
                Vector3 tilePos = transform.position + new Vector3(x * tileSize, 0f, z * tileSize);

                // ── Choix et placement du sol ─────────────────────────
                BiomeType biome = GetBiome(terrainValue);
                SpawnTile(biome, tilePos);

                // ── Placement de la décoration ────────────────────────
                if (biome == BiomeType.Grass)
                    SpawnDecoration(decoValue, tilePos);
            }

            // Yield all rows to avoid freezing the editor
            yield return null;
        }
    }

    // ─────────────────────────────────────────────
    //  HELPERS
    // ─────────────────────────────────────────────

    private enum BiomeType { River, Path, Grass }

    private BiomeType GetBiome(float value)
    {
        if (value < riverThreshold) return BiomeType.River;
        if (value < pathThreshold) return BiomeType.Path;
        return BiomeType.Grass;
    }

    /// <summary>Bruit de Perlin normalisé entre 0 et 1.</summary>
    private float SampleNoise(int x, int z, float scale, float offX, float offZ)
    {
        float nx = (x + offX) / scale;
        float nz = (z + offZ) / scale;
        // Mathf.PerlinNoise returns ~[0,1]
        return Mathf.Clamp01(Mathf.PerlinNoise(nx, nz));
    }

    private void SpawnTile(BiomeType biome, Vector3 pos)
    {
        List<GameObject> pool = biome switch
        {
            BiomeType.River => riverPrefabs,
            BiomeType.Path => pathPrefabs,
            _ => grassPrefabs,
        };

        if (pool == null || pool.Count == 0) return;

        GameObject prefab = pool[rng.Next(pool.Count)];
        if (prefab == null) return;

        var go = Instantiate(prefab, pos, Quaternion.identity, transform);
        spawnedObjects.Add(go);
    }

    private void SpawnDecoration(float decoValue, Vector3 tilePos)
    {
        // The decoration value determines which type of object to place.
        // Division of the domain [0.1]:
        //   [0 ... treeDensity)   → tree
        //   [treeDensity ... treeDensity+rockDensity)  → rock
        //   [... + plantDensity)   → plant / strain
        //   remains   → nothing

        float cursor = 0f;
        List<GameObject> chosenPool = null;

        cursor += treeDensity;
        if (decoValue < cursor)
        {
            chosenPool = treePrefabs;
            goto place;
        }

        cursor += rockDensity;
        if (decoValue < cursor)
        {
            chosenPool = rockPrefabs;
            goto place;
        }

        cursor += plantDensity;
        if (decoValue < cursor)
        {
            // 50-50 between plant and strain
            chosenPool = ((float)rng.NextDouble() < 0.5f && stumpPrefabs.Count > 0)
                ? stumpPrefabs : plantPrefabs;
        }

    place:
        if (chosenPool == null || chosenPool.Count == 0) return;

        GameObject prefab = chosenPool[rng.Next(chosenPool.Count)];
        if (prefab == null) return;

        // Slight variation in position to break the grid
        float jitterX = (float)(rng.NextDouble() - 0.5) * tileSize * 0.4f;
        float jitterZ = (float)(rng.NextDouble() - 0.5) * tileSize * 0.4f;
        Vector3 decoPos = tilePos + new Vector3(jitterX, 0f, jitterZ);

        // Random Y rotation
        Quaternion rot = randomYRotation
            ? Quaternion.Euler(0f, (float)(rng.NextDouble() * 360f), 0f)
            : Quaternion.identity;

        var go = Instantiate(prefab, decoPos, rot, transform);

        // Variation in scale
        if (scaleVariation > 0f)
        {
            float s = 1f + (float)(rng.NextDouble() * 2 - 1) * scaleVariation;
            go.transform.localScale *= s;
        }

        spawnedObjects.Add(go);
    }

#if UNITY_EDITOR
    // Displays the grid in the Scene view for easy placement
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.2f, 0.8f, 0.2f, 0.25f);
        for (int x = 0; x < gridWidth; x++)
            for (int z = 0; z < gridHeight; z++)
            {
                Vector3 center = transform.position
                    + new Vector3(x * tileSize + tileSize * 0.5f, 0f, z * tileSize + tileSize * 0.5f);
                Gizmos.DrawWireCube(center, new Vector3(tileSize, 0.05f, tileSize));
            }
    }
#endif
}