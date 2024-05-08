using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class MainMenuManager : MonoBehaviour
{
    public VideoPlayer cutscenePlayer;
    public Canvas mainMenuCanvas; 

    private void Start()
    {
        // Ensure the VideoPlayer is not playing initially
        cutscenePlayer.Stop();
    }

    public void StartGame()
    {
        
        SceneManager.LoadScene("Cutscene");
    }

    public void OpenOptions()
    {
        SceneManager.LoadScene("Options");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        ScoreManager.score = 0;
    }
    public void EndlessMode()
    {
        SceneManager.LoadScene("EndlessMode");
        ScoreManager.score = 0;
    }
    //public void PlayCutscene()
    //{
    // Set the video to play
    // cutscenePlayer.Play();

    // Hide the canvas
    //mainMenuCanvas.enabled = false;

    // Wait for the cutscene to finish playing (convert double to float)
    //StartCoroutine(LoadLevelAfterCutscene((float)cutscenePlayer.clip.length));
    //}

    //IEnumerator LoadLevelAfterCutscene(float delay)
    //{
    //yield return new WaitForSeconds(delay);

    // Load the game level
    //SceneManager.LoadScene("Tut+Level1");
    //}
}
