using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct HoloRecording
{
    public string pathToAnimationClip;
    public string animationClipName;
    public HoloRecording(string pathToAnimationClip, string animationClipName)
    {
        this.pathToAnimationClip = pathToAnimationClip;
        this.animationClipName = animationClipName;
    }

    public override string ToString() => $"HoloRecording: animation clip is called {animationClipName}";

}

[System.Serializable]
public class AllKeyFrames
{
    public PoseKeyframeLists palmPoses;

    public AllKeyFrames(PoseKeyframeLists palmPoses)
    {
        this.palmPoses = palmPoses;
    }
}

[System.Serializable]
public class PoseKeyframeLists
{
    public List<SerializableKeyframe> keyframesPositionX;
    public List<SerializableKeyframe> keyframesPositionY;
    public List<SerializableKeyframe> keyframesPositionZ;

    public PoseKeyframeLists(List<SerializableKeyframe> keyframesPositionX, List<SerializableKeyframe> keyframesPositionY, List<SerializableKeyframe> keyframesPositionZ)
    {
        this.keyframesPositionX = keyframesPositionX;
        this.keyframesPositionY = keyframesPositionY;
        this.keyframesPositionZ = keyframesPositionZ;
    }
}


[System.Serializable]
public struct SerializableKeyframe
{
    public SerializableKeyframe(float time, float value)
    {
        this.time = time;
        this.value = value;
    }

    public Keyframe GetKeyframe()
    {
        Keyframe keyframe = new Keyframe(this.time, this.value);
        return keyframe;
    }


    public float time;
    public float value;

}