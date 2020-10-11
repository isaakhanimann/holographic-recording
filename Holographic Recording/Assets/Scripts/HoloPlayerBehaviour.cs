﻿using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit;
using UnityEngine.UI;
using TMPro;

public class HoloPlayerBehaviour : MonoBehaviour
{

    private AudioSource audioSource;
    private IMixedRealityInputSystem inputSystem;
    private InputPlaybackService animationPlayer;

    public GameObject Debugger;
    void Start()
    {
        Debugger = GameObject.FindGameObjectWithTag("Respawn");
        InitializeHoloPlayer();
    }

    private void InitializeHoloPlayer()
    {
        audioSource = GetComponent<AudioSource>();
        if (!MixedRealityServiceRegistry.TryGetService<IMixedRealityInputSystem>(out inputSystem))
        {
            Debug.Log("Failed to acquire the input system inside Holoplayer. It may not have been registered");
        }
        animationPlayer = new InputPlaybackService(inputSystem);
    }

    public void PutHoloRecordingIntoPlayer(HoloRecording recording)
    {
        //audioSource.clip = recording.audioClip;
        if (animationPlayer.LoadInputAnimation(recording.pathToAnimationFile))
        {
            Debugger.GetComponent<TextMeshPro>().text = "Loading Input Animation works!!";
            Debug.Log("Loading Input Animation works!!");
        }

        else
        {
            Debugger.GetComponent<TextMeshPro>().text = "Loading Input Animation doens't work!!";
            Debug.Log("Loading Input Animation doens't work!!");
        }





    }

    public void Play()
    {
        //audioSource.Play();
        animationPlayer.Play();
        Debug.Log("Play" + animationPlayer);
    }

    public void Pause()
    {
        //audioSource.Pause();
        animationPlayer.Pause();
    }

    public void Seek(float timeToJumpToInSeconds)
    {
        //audioSource.time = timeToJumpToInSeconds;
        animationPlayer.LocalTime = timeToJumpToInSeconds;
    }

    public void Stop()
    {
        //audioSource.Stop();
        animationPlayer.Stop();
    }
}
