using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct HoloRecording
{
    //public AudioClip audioClip;
    public string pathToAnimationFile;
    public HoloRecording(AudioClip audioClip, string pathToAnimationFile)
    {
        //this.audioClip = audioClip;
        this.pathToAnimationFile = pathToAnimationFile;
    }

    public override string ToString() => $"( path to animation file: {pathToAnimationFile})";

}
