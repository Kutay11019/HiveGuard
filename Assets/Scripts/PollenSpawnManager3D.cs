using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PollenSpawnManager3D : MonoBehaviour
{
    [Header("Flower / Pollen Prefabs")]
    [SerializeField] private GameObject[] pollenPrefabs;
    [SerializeField] private Transform spawnedPollenParent;

    [Header("Spawn Point Setup")]
    [SerializeField] private Transform spawnPointsParent;
    [SerializeField] private Transform[] spawnPoints;

    [Header("Wave Settings")]
    [SerializeField] private int pollenPerWave = 4;
    [SerializeField] private float waveInterval = 4f;
    [SerializeField] private float spawnYOffset = 0f;
    [SerializeField] private bool startSpawningAutomatically = false;

    [Header("Random Visual Settings")]
    [SerializeField] private bool randomizeYRotation = true;

    [Header("Debug Settings")]
    [SerializeField] private bool logSpawnDetails = true;
    [SerializeField] private bool warnIfSpawnOutsideCamera = true;

    private readonly List<GameObject> activePollenObjects = new List<GameObject>();
    private Coroutine spawnRoutine;
    private int waveNumber = 0;

    private void Awake()
    {
        if (spawnedPollenParent == null)
        {
            spawnedPollenParent = transform;
        }

        RefreshSpawnPointsFromParent();
    }

    private void Start()
    {
        if (startSpawningAutomatically)
        {
            StartSpawning();
        }
    }

    public void StartSpawning()
    {
        RefreshSpawnPointsFromParent();

        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
        }

        waveNumber = 0;

        DespawnActivePollen();

        spawnRoutine = StartCoroutine(SpawnLoop());
    }

    public void StopSpawning()
    {
        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }

        DespawnActivePollen();
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            SpawnPollenGroup();

            yield return new WaitForSeconds(waveInterval);

            DespawnActivePollen();
        }
    }

    private void SpawnPollenGroup()
    {
        List<GameObject> validPrefabs = GetValidPrefabs();
        List<Transform> validSpawnPoints = GetValidSpawnPoints();

        if (validPrefabs.Count == 0)
        {
            Debug.LogWarning("No valid flower / pollen prefabs assigned.");
            return;
        }

        if (validSpawnPoints.Count == 0)
        {
            Debug.LogWarning("No valid pollen spawn points found.");
            return;
        }

        waveNumber++;

        int spawnCount = Mathf.Min(pollenPerWave, validSpawnPoints.Count);

        ShuffleSpawnPoints(validSpawnPoints);

        if (logSpawnDetails)
        {
            Debug.Log("----- Pollen Wave " + waveNumber + " started. Spawn Count: " + spawnCount + " -----");
        }

        for (int i = 0; i < spawnCount; i++)
        {
            Transform selectedSpawnPoint = validSpawnPoints[i];
            GameObject selectedPrefab = validPrefabs[Random.Range(0, validPrefabs.Count)];

            Vector3 spawnPosition = selectedSpawnPoint.position + Vector3.up * spawnYOffset;

            Quaternion spawnRotation = selectedSpawnPoint.rotation;

            if (randomizeYRotation)
            {
                spawnRotation = Quaternion.Euler(
                    0f,
                    Random.Range(0f, 360f),
                    0f
                );
            }

            GameObject spawnedPollen = Instantiate(
                selectedPrefab,
                spawnPosition,
                spawnRotation,
                spawnedPollenParent
            );

            spawnedPollen.name = selectedPrefab.name + "_Spawned_From_" + selectedSpawnPoint.name;

            activePollenObjects.Add(spawnedPollen);

            if (logSpawnDetails)
            {
                Debug.Log(
                    "Wave " + waveNumber +
                    " | Flower: " + selectedPrefab.name +
                    " | Spawn Point: " + selectedSpawnPoint.name +
                    " | Spawn Position: " + spawnPosition
                );
            }

            if (warnIfSpawnOutsideCamera)
            {
                WarnIfOutsideCamera(selectedSpawnPoint, spawnPosition);
            }
        }

        Debug.Log("Spawned pollen group. Count: " + spawnCount);
    }

    private void WarnIfOutsideCamera(Transform spawnPoint, Vector3 spawnPosition)
    {
        if (Camera.main == null)
        {
            return;
        }

        Vector3 viewportPosition = Camera.main.WorldToViewportPoint(spawnPosition);

        bool isBehindCamera = viewportPosition.z < 0f;
        bool isOutsideHorizontal = viewportPosition.x < 0f || viewportPosition.x > 1f;
        bool isOutsideVertical = viewportPosition.y < 0f || viewportPosition.y > 1f;

        if (isBehindCamera || isOutsideHorizontal || isOutsideVertical)
        {
            Debug.LogWarning(
                "OUTSIDE CAMERA VIEW -> Spawn Point: " + spawnPoint.name +
                " | World Position: " + spawnPosition +
                " | Viewport Position: " + viewportPosition +
                " | Check this spawn point position."
            );
        }
    }

    private void RefreshSpawnPointsFromParent()
    {
        if (spawnPointsParent == null)
        {
            Debug.LogWarning("Spawn Points Parent is not assigned.");
            return;
        }

        List<Transform> foundSpawnPoints = new List<Transform>();

        foreach (Transform child in spawnPointsParent)
        {
            if (child != null && child.gameObject.activeInHierarchy)
            {
                foundSpawnPoints.Add(child);
            }
        }

        spawnPoints = foundSpawnPoints.ToArray();

        Debug.Log("Pollen spawn points found: " + spawnPoints.Length);

        if (logSpawnDetails)
        {
            for (int i = 0; i < spawnPoints.Length; i++)
            {
                Debug.Log(
                    "Spawn Point Registered -> Index: " + i +
                    " | Name: " + spawnPoints[i].name +
                    " | Position: " + spawnPoints[i].position
                );
            }
        }
    }

    private List<GameObject> GetValidPrefabs()
    {
        List<GameObject> validPrefabs = new List<GameObject>();

        if (pollenPrefabs == null)
        {
            return validPrefabs;
        }

        foreach (GameObject prefab in pollenPrefabs)
        {
            if (prefab != null)
            {
                validPrefabs.Add(prefab);
            }
        }

        return validPrefabs;
    }

    private List<Transform> GetValidSpawnPoints()
    {
        List<Transform> validSpawnPoints = new List<Transform>();

        if (spawnPoints == null)
        {
            return validSpawnPoints;
        }

        foreach (Transform spawnPoint in spawnPoints)
        {
            if (spawnPoint != null && spawnPoint.gameObject.activeInHierarchy)
            {
                validSpawnPoints.Add(spawnPoint);
            }
        }

        return validSpawnPoints;
    }

    private void DespawnActivePollen()
    {
        for (int i = activePollenObjects.Count - 1; i >= 0; i--)
        {
            if (activePollenObjects[i] != null)
            {
                Destroy(activePollenObjects[i]);
            }
        }

        activePollenObjects.Clear();
    }

    private void ShuffleSpawnPoints(List<Transform> points)
    {
        for (int i = 0; i < points.Count; i++)
        {
            int randomIndex = Random.Range(i, points.Count);

            Transform temp = points[i];
            points[i] = points[randomIndex];
            points[randomIndex] = temp;
        }
    }

    private void OnDisable()
    {
        StopSpawning();
    }

    private void OnDrawGizmosSelected()
    {
        List<Transform> pointsToDraw = new List<Transform>();

        if (spawnPointsParent != null)
        {
            foreach (Transform child in spawnPointsParent)
            {
                pointsToDraw.Add(child);
            }
        }
        else if (spawnPoints != null)
        {
            pointsToDraw.AddRange(spawnPoints);
        }

        Gizmos.color = Color.yellow;

        foreach (Transform point in pointsToDraw)
        {
            if (point != null)
            {
                Gizmos.DrawWireSphere(point.position + Vector3.up * spawnYOffset, 0.25f);
            }
        }
    }
}