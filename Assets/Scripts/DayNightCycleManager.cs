using TMPro;
using UnityEngine;

public class DayNightCycleManager : MonoBehaviour
{
    private enum GamePhase
    {
        Day,
        Night,
        GameComplete
    }

    [Header("Phase Durations")]
    [SerializeField] private float dayDuration = 25f;
    [SerializeField] private float nightDuration = 20f;

    [Header("Level Progression")]
    [SerializeField] private int totalDays = 5;
    [SerializeField] private int currentDay = 1;

    [Header("Night Enemy Waves")]
    [SerializeField] private NightEnemyWaveSpawner nightEnemyWaveSpawner;

    [Header("Pollen Spawning")]
    [SerializeField] private PollenSpawnManager3D pollenSpawnManager;

    [Header("Lighting Optional")]
    [SerializeField] private Light directionalLight;
    [SerializeField] private float dayLightIntensity = 1.2f;
    [SerializeField] private float nightLightIntensity = 0.35f;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI phaseText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI statusText;

    [Header("Objective UI")]
    [SerializeField] private DayObjectiveUI dayObjectiveUI;

    [Header("Result Manager")]
    [SerializeField] private PrototypeResultManager resultManager;

    private GamePhase currentPhase;
    private float currentTimer;
    private bool isTimerRunning;

    private void Awake()
    {
        if (dayObjectiveUI == null)
        {
            dayObjectiveUI = FindFirstObjectByType<DayObjectiveUI>();
        }

        if (pollenSpawnManager == null)
        {
            pollenSpawnManager = FindFirstObjectByType<PollenSpawnManager3D>();
        }

        if (nightEnemyWaveSpawner == null)
        {
            nightEnemyWaveSpawner = FindFirstObjectByType<NightEnemyWaveSpawner>();
        }
    }

    private void Start()
    {
        currentDay = Mathf.Clamp(currentDay, 1, totalDays);
        StartDayPhase();
    }

    private void Update()
    {
        if (!isTimerRunning)
        {
            return;
        }

        currentTimer -= Time.deltaTime;

        if (currentTimer < 0f)
        {
            currentTimer = 0f;
        }

        UpdateTimerUI();

        if (currentTimer <= 0f)
        {
            HandlePhaseTimerFinished();
        }
    }

    private void StartDayPhase()
    {
        currentPhase = GamePhase.Day;
        currentTimer = dayDuration;
        isTimerRunning = true;

        ClearNightEnemies();

        if (pollenSpawnManager != null)
        {
            pollenSpawnManager.StartSpawning();
        }

        if (directionalLight != null)
        {
            directionalLight.intensity = dayLightIntensity;
        }

        UpdatePhaseUI(
            "DAY " + currentDay,
            "Collect pollen and deliver it to the hive!"
        );

        UpdateTimerUI();

        if (dayObjectiveUI != null)
        {
            dayObjectiveUI.ShowDayObjective();
        }

        Debug.Log("Day " + currentDay + " started.");
    }

    private void StartNightPhase()
    {
        currentPhase = GamePhase.Night;
        currentTimer = nightDuration;
        isTimerRunning = true;

        if (pollenSpawnManager != null)
        {
            pollenSpawnManager.StopSpawning();
        }

        SpawnNightWave();

        if (directionalLight != null)
        {
            directionalLight.intensity = nightLightIntensity;
        }

        UpdatePhaseUI(
            "NIGHT " + currentDay,
            "Defend the hive from bears and wasps!"
        );

        UpdateTimerUI();

        if (dayObjectiveUI != null)
        {
            dayObjectiveUI.ShowNightObjective();
        }

        Debug.Log("Night " + currentDay + " started.");
    }

    private void SpawnNightWave()
    {
        if (nightEnemyWaveSpawner == null)
        {
            Debug.LogWarning("NightEnemyWaveSpawner is not assigned.");
            return;
        }

        nightEnemyWaveSpawner.SpawnWaveForDay(currentDay);
    }

    private void ClearNightEnemies()
    {
        if (nightEnemyWaveSpawner != null)
        {
            nightEnemyWaveSpawner.ClearSpawnedEnemies();
        }
    }

    private void HandlePhaseTimerFinished()
    {
        if (currentPhase == GamePhase.Day)
        {
            StartNightPhase();
        }
        else if (currentPhase == GamePhase.Night)
        {
            FinishNightPhase();
        }
    }

    private void FinishNightPhase()
    {
        ClearNightEnemies();

        if (currentDay >= totalDays)
        {
            CompleteGame();
            return;
        }

        currentDay++;
        StartDayPhase();
    }

    private void CompleteGame()
    {
        currentPhase = GamePhase.GameComplete;
        isTimerRunning = false;

        ClearNightEnemies();

        if (pollenSpawnManager != null)
        {
            pollenSpawnManager.StopSpawning();
        }

        if (timerText != null)
        {
            timerText.text = "Time: 0";
        }

        if (phaseText != null)
        {
            phaseText.text = "VICTORY";
        }

        if (statusText != null)
        {
            statusText.text = "You survived all nights and protected the hive!";
        }

        if (dayObjectiveUI != null)
        {
            dayObjectiveUI.HideObjective();
        }

        if (resultManager != null)
        {
            resultManager.ShowNightSurvived();
        }

        Debug.Log("Game complete. Player survived " + totalDays + " days.");
    }

    private void UpdatePhaseUI(string phaseLabel, string statusMessage)
    {
        if (phaseText != null)
        {
            phaseText.text = phaseLabel;
        }

        if (statusText != null)
        {
            statusText.text = statusMessage;
        }
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int displayedTime = Mathf.CeilToInt(currentTimer);
            timerText.text = "Time: " + displayedTime;
        }
    }
}