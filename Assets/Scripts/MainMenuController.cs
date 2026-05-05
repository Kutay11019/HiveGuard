using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string gameplaySceneName = "GameScene";

    [Header("UI Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject howToPlayPanel;

    private void Start()
    {
        ShowMainMenu();
    }

    public void StartGame()
    {
        SceneManager.LoadScene(gameplaySceneName);
    }

    public void ShowHowToPlay()
    {
        mainMenuPanel.SetActive(false);
        howToPlayPanel.SetActive(true);
    }

    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        howToPlayPanel.SetActive(false);
    }

    public void ExitGame()
    {
        Debug.Log("Exit button pressed.");

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}