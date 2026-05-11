using System.Collections.Generic;
using UnityEngine;

public class WaspSpawnManager : MonoBehaviour
{
    [Header("Wasp")]
    [SerializeField] private GameObject waspPrefab;
    [SerializeField] private Transform[] spawnPoints;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnInterval = 4f;
    [SerializeField] private int maxWaspsPerNight = 5;

    private float spawnTimer;
    private int spawnedThisNight;
    private bool isNightActive;

    private readonly List<GameObject> spawnedWasps = new List<GameObject>();

    private void Update()
    {
        if (!isNightActive)
        {
            return;
        }

        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0f && spawnedThisNight < maxWaspsPerNight)
        {
            SpawnWasp();
            spawnTimer = spawnInterval;
        }
    }

    public void StartNightSpawning()
    {
        isNightActive = true;
        spawnedThisNight = 0;
        spawnTimer = 0f;

        Debug.Log("Wasp spawning started.");
    }

    public void StopNightSpawning()
    {
        isNightActive = false;

        DestroyAllSpawnedWasps();

        Debug.Log("Wasp spawning stopped.");
    }

    private void SpawnWasp()
    {
        if (waspPrefab == null)
        {
            Debug.LogWarning("Wasp prefab is not assigned.");
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("No wasp spawn points assigned.");
            return;
        }

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        GameObject newWasp = Instantiate(
            waspPrefab,
            spawnPoint.position,
            spawnPoint.rotation
        );

        spawnedWasps.Add(newWasp);
        spawnedThisNight++;

        Debug.Log("Wasp spawned at: " + spawnPoint.name);
    }

    private void DestroyAllSpawnedWasps()
    {
        for (int i = spawnedWasps.Count - 1; i >= 0; i--)
        {
            if (spawnedWasps[i] != null)
            {
                Destroy(spawnedWasps[i]);
            }
        }

        spawnedWasps.Clear();
    }
}