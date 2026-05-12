using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PrototypeResultManager : MonoBehaviour
{
    [Header("Result UI")]
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private TextMeshProUGUI resultTitleText;
    [SerializeField] private TextMeshProUGUI resultMessageText;
    [SerializeField] private Button restartDayButton;

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

        HideResult();
    }

    public void ShowVictory()
    {
        ShowResult(
            "You Won!",
            "You survived all nights and protected the hive!",
            false
        );
    }

    public void ShowDefeat(string defeatMessage)
    {
        ShowResult(
            "You Lost!",
            defeatMessage,
            true
        );
    }

    public void HideResult()
    {
        if (resultPanel != null)
        {
            resultPanel.SetActive(false);
        }
    }

    private void ShowResult(string title, string message, bool showRestartButton)
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
    }

    private void RestartCurrentDay()
    {
        if (dayNightCycleManager != null)
        {
            dayNightCycleManager.RestartCurrentDay();
        }
    }
}