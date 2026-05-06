using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PollenSpawnManager3D : MonoBehaviour
{
    [Header("Pollen Setup")]
    [SerializeField] private GameObject pollenPrefab;
    [SerializeField] private Transform spawnedPollenParent;

    [Header("Spawn Point Setup")]
    [SerializeField] private Transform spawnPointsParent;
    [SerializeField] private Transform[] spawnPoints;

    [Header("Wave Settings")]
    [SerializeField] private int pollenPerWave = 4;
    [SerializeField] private float pollenLifetime = 10f;
    [SerializeField] private float delayBetweenWaves = 1.5f;
    [SerializeField] private float spawnYOffset = 0.5f;
    [SerializeField] private bool startSpawningAutomatically = false;

    [Header("Debug")]
    [SerializeField] private bool debugKeepPollenAlive = false;

    private readonly List<GameObject> activePollenObjects = new List<GameObject>();
    private Coroutine spawnRoutine;

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
            SpawnPollenWave();

            if (debugKeepPollenAlive)
            {
                yield break;
            }

            yield return new WaitForSeconds(pollenLifetime);

            DespawnActivePollen();

            yield return new WaitForSeconds(delayBetweenWaves);
        }
    }

    private void RefreshSpawnPointsFromParent()
    {
        if (spawnPointsParent == null)
        {
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
    }

    private void SpawnPollenWave()
    {
        if (pollenPrefab == null)
        {
            Debug.LogWarning("Pollen prefab is missing.");
            return;
        }

        List<Transform> validSpawnPoints = GetValidSpawnPoints();

        if (validSpawnPoints.Count == 0)
        {
            Debug.LogWarning("No valid pollen spawn points found.");
            return;
        }

        int spawnCount = Mathf.Min(pollenPerWave, validSpawnPoints.Count);

        ShuffleSpawnPoints(validSpawnPoints);

        for (int i = 0; i < spawnCount; i++)
        {
            Transform selectedSpawnPoint = validSpawnPoints[i];

            Vector3 spawnPosition = selectedSpawnPoint.position + Vector3.up * spawnYOffset;

            GameObject spawnedPollen = Instantiate(
                pollenPrefab,
                spawnPosition,
                selectedSpawnPoint.rotation,
                spawnedPollenParent
            );

            spawnedPollen.name = "Spawned_Pollen_" + (i + 1);
            spawnedPollen.SetActive(true);

            activePollenObjects.Add(spawnedPollen);

            Debug.Log("Pollen spawned: " + spawnedPollen.name + " at " + spawnPosition);
        }

        Debug.Log("Spawned pollen wave. Count: " + spawnCount);
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