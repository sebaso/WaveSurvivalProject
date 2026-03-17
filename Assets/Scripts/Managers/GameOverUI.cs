using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class GameOverUI : MonoBehaviour
{
    public static GameOverUI instance;

    [Header("UI Panels")]
    public GameObject gameOverPanel;
    public float delayBeforeShow = 2f;

    [Header("Statistics Text")]
    public TextMeshProUGUI killsText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI waveText;

    [Header("Buttons")]
    public Button restartButton;
    public Button mainMenuButton;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        gameOverPanel?.SetActive(false);

        restartButton?.onClick.AddListener(RestartGame);

        mainMenuButton?.onClick.AddListener(GoToMainMenu);
    }

    public void Show()
    {
        StartCoroutine(ShowRoutine());
    }

    private IEnumerator ShowRoutine()
    {
        yield return new WaitForSeconds(delayBeforeShow);

        if (gameOverPanel == null) yield break;

        if (killsText != null && ScoreManager.instance != null)
            killsText.text = "Enemies Killed: " + ScoreManager.instance.SessionKills;

        if (scoreText != null && ScoreManager.instance != null)
            scoreText.text = "Final Score: " + ScoreManager.instance.Score;

        if (waveText != null && WaveManager.instance != null)
            waveText.text = "Wave Reached: " + WaveManager.instance.currentWaveIndex;

        gameOverPanel.SetActive(true);
        TimeManager.instance.RequestPause(this);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (PlayerShootyManager.instance != null) PlayerShootyManager.instance.enabled = false;
    }

    public void RestartGame()
    {
        TimeManager.instance.RequestResume(this);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        TimeManager.instance.RequestResume(this);
        SceneManager.LoadScene("MainMenu");
    }
}
