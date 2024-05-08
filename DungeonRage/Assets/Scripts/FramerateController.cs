using UnityEngine;

public class FramerateController : MonoBehaviour
{
    public int target = 60;

    void Start()
    {
        // Lock the frame rate to a specific value
        Application.targetFrameRate = target;

        // Use VSync to sync the frame rate with the monitor's refresh rate
        QualitySettings.vSyncCount = 1; // 0 for no sync, 1 for sync with monitor
    }
}
