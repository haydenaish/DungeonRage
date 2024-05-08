using UnityEngine;
using UnityEngine.UI;

public class MusicVolumeController : MonoBehaviour
{
    public Slider volumeSlider; // Reference to the UI slider for controlling volume
    public AudioSource musicSource; // Reference to the AudioSource playing the music

    private void Start()
    {
        // Initialize the slider value to match the current music volume
        volumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.5f); // Load saved volume or default to 0.5f
        musicSource.volume = volumeSlider.value; // Set the initial music volume
        volumeSlider.onValueChanged.AddListener(ChangeVolume); // Listen for changes to the slider value
    }

    // Method called when the slider value changes
    private void ChangeVolume(float newVolume)
    {
        musicSource.volume = newVolume; // Set the music volume to the new value
        PlayerPrefs.SetFloat("MusicVolume", newVolume); // Save the new volume to PlayerPrefs
        PlayerPrefs.Save(); // Save PlayerPrefs to disk
    }
}
