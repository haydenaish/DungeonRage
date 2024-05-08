using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static int score = 0;
    public TextMeshProUGUI scoreText;

    // Update the score text whenever it changes
    void Update()
    {
        scoreText.text = "Score: " + score.ToString();
    }
}
