using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PrototypeResultManager : MonoBehaviour
{
    [Header("Result UI")]
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private TextMeshProUGUI resultTitleText;
    [SerializeField] private TextMeshProUGUI resultMessageText;

    [Header("Buttons")]
    [SerializeField] private Button restartDayButton;
    [SerializeField] private Button mainMenuButton;

    [Header("Scene Settings")]
    [SerializeField] private string mainMenuSceneName = "00_MainMenu";

    [Header("References")]
    [SerializeField] private DayNightCycleManager dayNightCycleManager;

    private void Awake()
    {
        if (dayNightCycleManager == null)
        {
            dayNightCycleManager = FindFirstObjectByType<DayNightCycleManager>();
        }

        if (restartDayButton != null)
        {
            restartDayButton.onClick.RemoveListener(RestartCurrentDay);
            restartDayButton.onClick.AddListener(RestartCurrentDay);
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.RemoveListener(GoToMainMenu);
            mainMenuButton.onClick.AddListener(GoToMainMenu);
        }

        HideResult();
    }

    public void ShowVictory()
    {
        ShowResult(
            "You Won!",
            "You survived all nights and protected the hive!",
            showRestartButton: false,
            showMainMenuButton: true
        );
    }

    public void ShowDefeat(string defeatMessage)
    {
        ShowResult(
            "You Lost!",
            defeatMessage,
            showRestartButton: true,
            showMainMenuButton: false
        );
    }

    public void HideResult()
    {
        if (resultPanel != null)
        {
            resultPanel.SetActive(false);
        }
    }

    private void ShowResult(
        string title,
        string message,
        bool showRestartButton,
        bool showMainMenuButton
    )
    {
        if (resultPanel != null)
        {
            resultPanel.SetActive(true);
        }

        if (resultTitleText != null)
        {
            resultTitleText.text = title;
        }

        if (resultMessageText != null)
        {
            resultMessageText.text = message;
        }

        if (restartDayButton != null)
        {
            restartDayButton.gameObject.SetActive(showRestartButton);
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.gameObject.SetActive(showMainMenuButton);
        }
    }

    private void RestartCurrentDay()
    {
        if (dayNightCycleManager != null)
        {
            dayNightCycleManager.RestartCurrentDay();
        }
    }

    private void GoToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}