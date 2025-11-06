using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Scene Settings")]
    public string gameSceneName = "GameScene"; // Change to your actual gameplay scene name

    [Header("Audio Settings")]
    public AudioSource musicSource;  // Assign in Inspector
    public AudioClip menuMusic;      // Assign your background music clip

    void Start()
    {
        // Play the menu music if it's not already playing
        if (musicSource != null && menuMusic != null)
        {
            musicSource.clip = menuMusic;
            musicSource.loop = true; // Loop music
            musicSource.Play();
        }
    }

    // Called when Play button is clicked
    public void PlayGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    // Called when Quit button is clicked
    public void QuitGame()
    {
        Debug.Log("Quit Game"); // This will show in the editor
        Application.Quit();    // Works in build only
    }
}
