using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DayNightCycleManager : MonoBehaviour
{
    private enum GamePhase
    {
        Day,
        Night,
        GameOver,
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

    [Header("Player and Hive")]
    [SerializeField] private BeeHealth beeHealth;
    [SerializeField] private HiveHealth hiveHealth;
    [SerializeField] private Transform playerBeeTransform;
    [SerializeField] private Transform playerDayStartPoint;

    [Header("Pollen Spawning")]
    [SerializeField] private PollenSpawnManager3D pollenSpawnManager;
    [SerializeField] private PollenInventory pollenInventory;

    [Header("Lighting Optional")]
    [SerializeField] private Light directionalLight;
    [SerializeField] private float dayLightIntensity = 1.2f;
    [SerializeField] private float nightLightIntensity = 0.35f;

    [Header("Ambient")]
    [SerializeField] private float dayAmbientIntensity = 1f;
    [SerializeField] private float nightAmbientIntensity = 0.25f;
    [SerializeField] private float dayReflectionIntensity = 1f;
    [SerializeField] private float nightReflectionIntensity = 0.25f;
    [SerializeField] private Color dayAmbientColor = new Color(0.55f, 0.55f, 0.55f, 1f);
    [SerializeField] private Color nightAmbientColor = new Color(0.10f, 0.14f, 0.28f, 1f);

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI phaseText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI statusText;

    [Header("Objective UI")]
    [SerializeField] private DayObjectiveUI dayObjectiveUI;

    [Header("Result Manager")]
    [SerializeField] private PrototypeResultManager resultManager;

    [Header("Night Transition Panel")]
    public NightTransitionPanel nightTransitionPanel;

    private GamePhase currentPhase;
    private float currentTimer;
    private bool isTimerRunning;
    private bool isWaitingForNightConfirmation;

    private int beeHealthAtDayStart;
    private int hiveHealthAtDayStart;
    private int beeMaxHealthAtDayStart;
    private int hiveMaxHealthAtDayStart;

    private int carriedPollenAtDayStart;
    private int storedPollenAtDayStart;

    private bool hasDayStartCheckpoint;

    private Dictionary<UpgradeType, int> upgradeLevelsAtDayStart;

    public int CurrentDay => currentDay;
    public bool IsDayPhase => currentPhase == GamePhase.Day;
    public bool IsNightPhase => currentPhase == GamePhase.Night;
    public bool CanDeliverPollen => currentPhase == GamePhase.Day && isTimerRunning;

    private void Awake()
    {
        if (nightEnemyWaveSpawner == null)
        {
            nightEnemyWaveSpawner = FindFirstObjectByType<NightEnemyWaveSpawner>();
        }

        if (beeHealth == null)
        {
            beeHealth = FindFirstObjectByType<BeeHealth>();
        }

        if (hiveHealth == null)
        {
            hiveHealth = FindFirstObjectByType<HiveHealth>();
        }

        if (playerBeeTransform == null && beeHealth != null)
        {
            playerBeeTransform = beeHealth.transform;
        }

        if (pollenSpawnManager == null)
        {
            pollenSpawnManager = FindFirstObjectByType<PollenSpawnManager3D>();
        }

        if (pollenInventory == null)
        {
            pollenInventory = FindFirstObjectByType<PollenInventory>();
        }

        if (dayObjectiveUI == null)
        {
            dayObjectiveUI = FindFirstObjectByType<DayObjectiveUI>();
        }

        if (resultManager == null)
        {
            resultManager = FindFirstObjectByType<PrototypeResultManager>();
        }

        if (nightTransitionPanel == null)
        {
            nightTransitionPanel = FindFirstObjectByType<NightTransitionPanel>();
        }

        if (nightTransitionPanel != null)
        {
            nightTransitionPanel.OnContinueRequested -= HandleNightTransitionContinue;
            nightTransitionPanel.OnContinueRequested += HandleNightTransitionContinue;
        }
    }

    private void OnDestroy()
    {
        if (nightTransitionPanel != null)
        {
            nightTransitionPanel.OnContinueRequested -= HandleNightTransitionContinue;
        }
    }

    private void Start()
    {
        currentDay = Mathf.Clamp(currentDay, 1, totalDays);

        // Skybox SH yerine elle kontrol edebilmek için ambient'i Flat moda al.
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;

        StartDayPhase(true);
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

        if (currentPhase == GamePhase.Night && IsNightWaveCleared())
        {
            Debug.Log("All night enemies defeated. Finishing night early.");
            FinishNightPhase();
            return;
        }

        if (currentTimer <= 0f)
        {
            HandlePhaseTimerFinished();
        }
    }

    private void StartDayPhase(bool saveCheckpointForThisDay)
    {
        currentPhase = GamePhase.Day;
        currentTimer = dayDuration;
        isTimerRunning = true;
        isWaitingForNightConfirmation = false;

        if (resultManager != null)
        {
            resultManager.HideResult();
        }

        ClearNightEnemies();
        MovePlayerToDayStartPoint();

        if (saveCheckpointForThisDay)
        {
            SaveDayStartCheckpoint();
        }

        if (pollenSpawnManager != null)
        {
            pollenSpawnManager.StartSpawning();
        }

        if (directionalLight != null)
        {
            directionalLight.intensity = dayLightIntensity;
        }

        RenderSettings.ambientIntensity = dayAmbientIntensity;
        RenderSettings.ambientLight = dayAmbientColor;
        RenderSettings.reflectionIntensity = dayReflectionIntensity;

        UpdatePhaseUI(
            "DAY " + currentDay,
            "Collect pollen and deliver it to the hive!"
        );

        UpdateTimerUI();

        if (dayObjectiveUI != null)
        {
            dayObjectiveUI.ShowDayObjective();
        }

        Debug.Log(
            "Day " + currentDay + " started. " +
            "Bee checkpoint HP: " + beeHealthAtDayStart +
            ", Hive checkpoint HP: " + hiveHealthAtDayStart +
            ", Carried Pollen checkpoint: " + carriedPollenAtDayStart +
            ", Stored Pollen checkpoint: " + storedPollenAtDayStart
        );
    }

    private void StartNightPhase()
    {
        currentPhase = GamePhase.Night;
        currentTimer = nightDuration;
        isTimerRunning = true;
        isWaitingForNightConfirmation = false;

        if (pollenSpawnManager != null)
        {
            pollenSpawnManager.StopSpawning();
        }

        SpawnNightWave();

        if (directionalLight != null)
        {
            directionalLight.intensity = nightLightIntensity;
        }

        RenderSettings.ambientIntensity = nightAmbientIntensity;
        RenderSettings.ambientLight = nightAmbientColor;
        RenderSettings.reflectionIntensity = nightReflectionIntensity;

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

    private bool IsNightWaveCleared()
    {
        if (nightEnemyWaveSpawner == null)
        {
            return false;
        }

        return nightEnemyWaveSpawner.IsCurrentWaveCleared();
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
            TryShowNightTransitionPanel();
        }
        else if (currentPhase == GamePhase.Night)
        {
            FinishNightPhase();
        }
    }

    private void TryShowNightTransitionPanel()
    {
        if (isWaitingForNightConfirmation)
        {
            return;
        }

        if (nightTransitionPanel == null)
        {
            StartNightPhase();
            return;
        }

        isWaitingForNightConfirmation = true;
        isTimerRunning = false;

        nightTransitionPanel.Show();
    }

    private void HandleNightTransitionContinue()
    {
        if (!isWaitingForNightConfirmation)
        {
            return;
        }

        isWaitingForNightConfirmation = false;
        StartNightPhase();
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

        // Yeni gün başlıyor.
        // Gece sonunda ne kaldıysa, yeni günün başlangıç checkpoint'i o oluyor.
        // Burada carried pollen de kaydedilecek.
        StartDayPhase(true);
    }

    private void CompleteGame()
    {
        currentPhase = GamePhase.GameComplete;
        isTimerRunning = false;
        isWaitingForNightConfirmation = false;

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
            resultManager.ShowVictory();
        }

        Debug.Log("Game complete. Player survived " + totalDays + " days.");
    }

    public void GameOverBecauseBeeDied()
    {
        TriggerGameOver("Your bee was defeated. Restart the day and try again!");
    }

    public void GameOverBecauseHiveDestroyed()
    {
        TriggerGameOver("The hive was destroyed. Restart the day and protect it better!");
    }

    private void TriggerGameOver(string defeatMessage)
    {
        if (currentPhase == GamePhase.GameOver || currentPhase == GamePhase.GameComplete)
        {
            return;
        }

        currentPhase = GamePhase.GameOver;
        isTimerRunning = false;
        isWaitingForNightConfirmation = false;

        ClearNightEnemies();

        if (pollenSpawnManager != null)
        {
            pollenSpawnManager.StopSpawning();
        }

        if (phaseText != null)
        {
            phaseText.text = "GAME OVER";
        }

        if (statusText != null)
        {
            statusText.text = defeatMessage;
        }

        if (dayObjectiveUI != null)
        {
            dayObjectiveUI.HideObjective();
        }

        if (resultManager != null)
        {
            resultManager.ShowDefeat(defeatMessage);
        }

        Debug.Log("Game over on day " + currentDay + ". " + defeatMessage);
    }

    public void RestartCurrentDay()
    {
        Debug.Log("Restarting day " + currentDay);

        ClearNightEnemies();

        RestoreDayStartCheckpoint();

        // Aynı günü yeniden başlatıyoruz ama checkpoint'i tekrar kaydetmiyoruz.
        // Çünkü checkpoint zaten o günün başındaki değerleri temsil ediyor.
        StartDayPhase(false);
    }

    private void SaveDayStartCheckpoint()
    {
        if (beeHealth != null)
        {
            beeHealthAtDayStart = beeHealth.CurrentHealth;
            beeMaxHealthAtDayStart = beeHealth.MaxHealth;
        }

        if (hiveHealth != null)
        {
            hiveHealthAtDayStart = hiveHealth.CurrentHealth;
            hiveMaxHealthAtDayStart = hiveHealth.MaxHealth;
        }

        if (UpgradeManager.Instance != null)
        {
            upgradeLevelsAtDayStart = UpgradeManager.Instance.SnapshotLevels();
        }

        if (pollenInventory != null)
        {
            carriedPollenAtDayStart = pollenInventory.CurrentPollen;
            storedPollenAtDayStart = pollenInventory.StoredPollen;
        }

        hasDayStartCheckpoint = true;

        Debug.Log(
            "Saved day start checkpoint. " +
            "Day: " + currentDay +
            ", Bee HP: " + beeHealthAtDayStart +
            ", Hive HP: " + hiveHealthAtDayStart +
            ", Bee MaxHP: " + beeMaxHealthAtDayStart +
            ", Hive MaxHP: " + hiveMaxHealthAtDayStart +
            ", Carried Pollen: " + carriedPollenAtDayStart +
            ", Hive Pollen: " + storedPollenAtDayStart
        );
    }

    private void RestoreDayStartCheckpoint()
    {
        if (!hasDayStartCheckpoint)
        {
            SaveDayStartCheckpoint();
        }

        if (beeHealth != null)
        {
            beeHealth.SetMaxHealth(beeMaxHealthAtDayStart);
            beeHealth.RestoreHealth(beeHealthAtDayStart);
        }

        if (hiveHealth != null)
        {
            hiveHealth.SetMaxHealth(hiveMaxHealthAtDayStart);
            hiveHealth.RestoreHealth(hiveHealthAtDayStart);
        }

        if (UpgradeManager.Instance != null)
        {
            UpgradeManager.Instance.RestoreLevels(upgradeLevelsAtDayStart);
        }

        if (pollenInventory != null)
        {
            pollenInventory.SetCurrentPollen(carriedPollenAtDayStart);
            pollenInventory.SetStoredPollen(storedPollenAtDayStart);
        }

        Debug.Log(
            "Restored day start checkpoint. " +
            "Day: " + currentDay +
            ", Bee HP: " + beeHealthAtDayStart +
            ", Hive HP: " + hiveHealthAtDayStart +
            ", Bee MaxHP: " + beeMaxHealthAtDayStart +
            ", Hive MaxHP: " + hiveMaxHealthAtDayStart +
            ", Carried Pollen: " + carriedPollenAtDayStart +
            ", Hive Pollen: " + storedPollenAtDayStart
        );
    }

    private void MovePlayerToDayStartPoint()
    {
        if (playerBeeTransform != null && playerDayStartPoint != null)
        {
            playerBeeTransform.position = playerDayStartPoint.position;
            playerBeeTransform.rotation = playerDayStartPoint.rotation;
        }
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