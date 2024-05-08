using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeathHandler : MonoBehaviour
{
    // Reference to the WaveController
    public WaveController waveController;
    public void HandlePlayerDeath()
    {
        SceneManager.LoadScene("EndlessMode");

    }
}
