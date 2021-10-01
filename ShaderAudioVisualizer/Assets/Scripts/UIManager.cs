using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Text.RegularExpressions;

public class UIManager : MonoBehaviour
{
    public InputField ytLink;
    public VideoPlayer videoPlayer;
    public Slider volumeSlider;
    public Button loadButton;
    public Text errorText;

    string baseUrl = "https://unity-youtube-dl-server.herokuapp.com/watch?v=";

    bool shouldPause = false;

    private void Start()
    {
        volumeSlider.value = videoPlayer.GetDirectAudioVolume(0);
        volumeSlider.onValueChanged.AddListener((value) => setNewVolume(value));
        videoPlayer.playOnAwake = false;

        videoPlayer.errorReceived += OnVideoError;
        videoPlayer.prepareCompleted += OnVideoPrepared;

        errorText.enabled = false;
    }

    void setNewVolume(float value)
    {
        volumeSlider.value = value;
        videoPlayer.SetDirectAudioVolume(0, value);
    }

    public void LoadVideo()
    {
        errorText.enabled = false;

        loadButton.GetComponentInChildren<Text>().text = "Loading video...";
        loadButton.interactable = false;

        videoPlayer.Pause();

        string url = baseUrl + ytLink.text;

        videoPlayer.url = url;
        videoPlayer.Prepare();
    }

    private void Update()
    {
        if (videoPlayer.isPrepared && !videoPlayer.isPlaying && !shouldPause)
        {
            PlayVideo();
        }
    }

    void OnVideoPrepared(VideoPlayer source)
    {
        PlayVideo();
    }

    void PlayVideo()
    {
        loadButton.GetComponentInChildren<Text>().text = "Load";
        loadButton.interactable = true;
        videoPlayer.Play();
    }

    public void TogglePlayPause()
    {
        if (videoPlayer.isPlaying)
        {
            shouldPause = true;
            videoPlayer.Pause();
        }

        else if (!videoPlayer.isPlaying && shouldPause)
        {
            shouldPause = false;
            videoPlayer.Play();
        }
    }

    void OnVideoError(VideoPlayer source, string message)
    {
        errorText.enabled = true;
        loadButton.GetComponentInChildren<Text>().text = "Load";
        loadButton.interactable = true;
    }


}
