using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NightWaveConfig
{
    public int dayNumber;
    public int bearCount;
    public int waspCount;
}

public class NightEnemyWaveSpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject bearPrefab;
    [SerializeField] private GameObject waspPrefab;

    [Header("Spawn Points")]
    [SerializeField] private Transform[] bearSpawnPoints;
    [SerializeField] private Transform[] waspSpawnPoints;

    [Header("Wave Settings")]
    [SerializeField] private NightWaveConfig[] waves;

    [Header("Spawned Enemy Parent Optional")]
    [SerializeField] private Transform spawnedEnemiesParent;

    private readonly List<GameObject> spawnedEnemies = new List<GameObject>();

    public void SpawnWaveForDay(int dayNumber)
    {
        ClearSpawnedEnemies();

        NightWaveConfig selectedWave = GetWaveForDay(dayNumber);

        if (selectedWave == null)
        {
            Debug.LogWarning("No wave config found for day: " + dayNumber);
            return;
        }

        SpawnEnemies(bearPrefab, bearSpawnPoints, selectedWave.bearCount, "Bear");
        SpawnEnemies(waspPrefab, waspSpawnPoints, selectedWave.waspCount, "Wasp");

        Debug.Log(
            "Night wave spawned for day " + dayNumber +
            ". Bears: " + selectedWave.bearCount +
            ", Wasps: " + selectedWave.waspCount
        );
    }

    public void ClearSpawnedEnemies()
    {
        for (int i = spawnedEnemies.Count - 1; i >= 0; i--)
        {
            if (spawnedEnemies[i] != null)
            {
                Destroy(spawnedEnemies[i]);
            }
        }

        spawnedEnemies.Clear();
    }

    public bool IsCurrentWaveCleared()
    {
        if (spawnedEnemies.Count == 0)
        {
            return false;
        }

        for (int i = spawnedEnemies.Count - 1; i >= 0; i--)
        {
            GameObject enemy = spawnedEnemies[i];

            if (enemy == null)
            {
                spawnedEnemies.RemoveAt(i);
                continue;
            }

            BearHealth bearHealth = enemy.GetComponentInChildren<BearHealth>();

            if (bearHealth != null && !bearHealth.IsDead)
            {
                return false;
            }

            WaspHealth waspHealth = enemy.GetComponentInChildren<WaspHealth>();

            if (waspHealth != null && !waspHealth.IsDead)
            {
                return false;
            }

            if (bearHealth == null && waspHealth == null)
            {
                return false;
            }
        }

        return true;
    }

    private NightWaveConfig GetWaveForDay(int dayNumber)
    {
        if (waves == null || waves.Length == 0)
        {
            return null;
        }

        NightWaveConfig lastAvailableWave = null;

        foreach (NightWaveConfig wave in waves)
        {
            if (wave == null)
            {
                continue;
            }

            if (wave.dayNumber == dayNumber)
            {
                return wave;
            }

            if (wave.dayNumber < dayNumber)
            {
                if (lastAvailableWave == null || wave.dayNumber > lastAvailableWave.dayNumber)
                {
                    lastAvailableWave = wave;
                }
            }
        }

        // İleride totalDays 10 yapılır ama 6-10 wave girilmezse,
        // oyun patlamasın diye en son tanımlı wave'i kullanır.
        return lastAvailableWave;
    }

    private void SpawnEnemies(
        GameObject enemyPrefab,
        Transform[] spawnPoints,
        int enemyCount,
        string enemyName
    )
    {
        if (enemyCount <= 0)
        {
            return;
        }

        if (enemyPrefab == null)
        {
            Debug.LogWarning(enemyName + " prefab is not assigned.");
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning(enemyName + " spawn points are not assigned.");
            return;
        }

        for (int i = 0; i < enemyCount; i++)
        {
            Transform selectedSpawnPoint = spawnPoints[i % spawnPoints.Length];

            GameObject spawnedEnemy = Instantiate(
                enemyPrefab,
                selectedSpawnPoint.position,
                selectedSpawnPoint.rotation,
                spawnedEnemiesParent
            );

            spawnedEnemy.name = enemyName + "_DayWave_" + i;

            spawnedEnemies.Add(spawnedEnemy);
        }
    }
}