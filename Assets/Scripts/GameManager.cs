using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI References")]
    public GameObject pausePanel;
    public GameObject gameOverPanel;
    public GameObject pauseButton; 
    public TMP_Text scoreText;
    public TMP_Text finalScoreText;
    public TMP_Text highScoreText;


    [Header("Audio Settings")]
    public AudioSource musicSource;
    public AudioClip backgroundMusic;
    public AudioClip gameOverMusic;

    private bool isPaused = false;
    private bool gameOver = false;
    private int currentScore = 0;
    private int highScore = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    void Start()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);

        // ... (music setup) ...

        highScore = PlayerPrefs.GetInt("HighScore", 0);

        UpdateHighScoreUI();

        UpdateScoreUI();
    }

    void UpdateHighScoreUI()
    {
        if (highScoreText != null)
        {
            highScoreText.text = $"High Score: {highScore:D4}";
        }
    }

    public void ShowGameOverScreen()
    {
        if (gameOver) return;
        gameOver = true;

        Time.timeScale = 0f;


        if (CoinManager.Instance != null)
        {
            currentScore = CoinManager.Instance.coinCount;
        }

        if (finalScoreText != null) finalScoreText.text = $"Final Coins: {currentScore}";

        if (currentScore > highScore)
        {
            highScore = currentScore;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }

        if (gameOverPanel != null) gameOverPanel.SetActive(true);

        if (pauseButton != null) pauseButton.SetActive(false); //  HIDE PAUSE BUTTON AT GAME OVER

        if (finalScoreText != null) finalScoreText.text = $"Final Score: {currentScore}";

        if (highScoreText != null) highScoreText.text = $"High Score: {highScore}";

        if (musicSource != null)
        {
            musicSource.Stop();
            if (gameOverMusic != null)
            {
                musicSource.clip = gameOverMusic;
                musicSource.loop = false;
                musicSource.Play();
            }
        }

        UpdateHighScoreUI();

        // ... (music logic) ...
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !gameOver)
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    public void AddScore(int points)
    {
        currentScore += points;
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = $"Score: {currentScore:D4}";
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;

        if (pausePanel != null) pausePanel.SetActive(true);

        if (pauseButton != null) pauseButton.SetActive(false); //  HIDE PAUSE BUTTON

        if (musicSource != null) musicSource.Pause();
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;

        if (pausePanel != null) pausePanel.SetActive(false);

        if (pauseButton != null) pauseButton.SetActive(true); //  SHOW PAUSE BUTTON

        if (musicSource != null) musicSource.UnPause();
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    /*public void ShowGameOverScreen()
    {
        if (gameOver) return;
        gameOver = true;

        Time.timeScale = 0f;

        if (gameOverPanel != null) gameOverPanel.SetActive(true);

        if (pauseButton != null) pauseButton.SetActive(false); //  HIDE PAUSE BUTTON AT GAME OVER

        if (finalScoreText != null) finalScoreText.text = $"Final Score: {currentScore}";

        if (currentScore > highScore)
        {
            highScore = currentScore;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }

        if (highScoreText != null) highScoreText.text = $"High Score: {highScore}";

        if (musicSource != null)
        {
            musicSource.Stop();
            if (gameOverMusic != null)
            {
                musicSource.clip = gameOverMusic;
                musicSource.loop = false;
                musicSource.Play();
            }
        }
    }*/

    public bool IsGameOver()
    {
        return gameOver;
    }
}
