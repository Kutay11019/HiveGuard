using TMPro;
using UnityEngine;

public class DayNightCycleManager : MonoBehaviour
{
    private enum GamePhase
    {
        Day,
        Night,
        DemoComplete
    }

    [Header("Phase Durations")]
    [SerializeField] private float dayDuration = 25f;
    [SerializeField] private float nightDuration = 20f;

    [Header("Enemy Setup")]
    [SerializeField] private GameObject enemyBear;
    [SerializeField] private Transform bearSpawnPoint;

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
    }

    private void Start()
    {
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

        if (enemyBear != null)
        {
            enemyBear.SetActive(false);
        }

        if (directionalLight != null)
        {
            directionalLight.intensity = dayLightIntensity;
        }

        UpdatePhaseUI("DAY", "Collect pollen and deliver it to the hive!");
        UpdateTimerUI();

        if (dayObjectiveUI != null)
        {
            dayObjectiveUI.ShowDayObjective();
        }

        Debug.Log("Day phase started.");
    }

    private void StartNightPhase()
    {
        currentPhase = GamePhase.Night;
        currentTimer = nightDuration;
        isTimerRunning = true;

        if (enemyBear != null)
        {
            if (bearSpawnPoint != null)
            {
                enemyBear.transform.position = bearSpawnPoint.position;
                enemyBear.transform.rotation = bearSpawnPoint.rotation;
            }

            enemyBear.SetActive(true);
        }

        if (directionalLight != null)
        {
            directionalLight.intensity = nightLightIntensity;
        }

        UpdatePhaseUI("NIGHT", "Defend the hive from bears!");
        UpdateTimerUI();

        if (dayObjectiveUI != null)
        {
            dayObjectiveUI.ShowNightObjective();
        }

        Debug.Log("Night phase started.");
    }

    private void CompleteDemoPhase()
    {
        currentPhase = GamePhase.DemoComplete;
        isTimerRunning = false;

        if (timerText != null)
        {
            timerText.text = "Time: 0";
        }

        if (dayObjectiveUI != null)
        {
            dayObjectiveUI.HideObjective();
        }

        if (resultManager != null)
        {
            resultManager.ShowNightSurvived();
        }
        else
        {
            Debug.Log("Demo complete. Night survived.");
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
            CompleteDemoPhase();
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