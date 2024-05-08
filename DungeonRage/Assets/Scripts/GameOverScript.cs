using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverScene : MonoBehaviour
{
    public TextMeshProUGUI scoreText;

    // Start is called before the first frame update
    void Start()
    {
        // Retrieve the saved score from PlayerPrefs and display it
        int playerScore = PlayerPrefs.GetInt("PlayerScore", 0);
        scoreText.text = "Score: " + playerScore.ToString();
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void RestartEndlessMode()
    {
        SceneManager.LoadScene("EndlessMode");
    }
}
