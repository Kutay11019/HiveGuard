using TMPro;
using UnityEngine;

public class PrototypeResultManager : MonoBehaviour
{
    [Header("Result UI")]
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private TextMeshProUGUI resultTitleText;
    [SerializeField] private TextMeshProUGUI resultMessageText;

    [Header("Objects To Disable When Game Ends")]
    [SerializeField] private GameObject enemyBear;

    private bool hasGameEnded = false;

    private void Start()
    {
        Time.timeScale = 1f;

        if (resultPanel != null)
        {
            resultPanel.SetActive(false);
        }
    }

    public void ShowNightSurvived()
    {
        if (hasGameEnded)
        {
            return;
        }

        hasGameEnded = true;

        ShowResult(
            "Night Survived!",
            "Prototype Demo Complete"
        );

        Debug.Log("Night survived. Demo complete.");
    }

    public void ShowGameOver()
    {
        if (hasGameEnded)
        {
            return;
        }

        hasGameEnded = true;

        ShowResult(
            "Hive Collapsed!",
            "Game Over"
        );

        Debug.Log("Hive collapsed. Game over.");
    }

    private void ShowResult(string title, string message)
    {
        if (enemyBear != null)
        {
            enemyBear.SetActive(false);
        }

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

        Time.timeScale = 0f;
    }
}