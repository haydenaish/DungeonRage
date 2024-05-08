using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField] string videoFileName;

    // Start is called before the first frame update
    void Start()
    {
        playVideo();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void playVideo()
    {
        VideoPlayer videoPlayer = GetComponent<VideoPlayer>();

        if (videoPlayer)
        {
            string videopath = System.IO.Path.Combine(Application.streamingAssetsPath, videoFileName);
            Debug.Log(videopath);
            videoPlayer.url = videopath;

            // Subscribe to the loopPointReached event
            videoPlayer.loopPointReached += OnVideoFinished;

            videoPlayer.Play();
        }
    }

    // Event handler for when the video is finished playing
    private void OnVideoFinished(VideoPlayer vp)
    {
        // Unsubscribe from the event to avoid multiple calls
        vp.loopPointReached -= OnVideoFinished;

        // Load the Tut+Level1 scene
        SceneManager.LoadScene("Tut+Level1");
    }
}
