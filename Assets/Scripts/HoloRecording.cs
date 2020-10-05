using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoloRecording
{
    public AudioClip audioClip;
    public string pathToAnimationFile;
    public HoloRecording(AudioClip audioClip, string pathToAnimationFile)
    {
        this.audioClip = audioClip;
        this.pathToAnimationFile = pathToAnimationFile;
    }
}
