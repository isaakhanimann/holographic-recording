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
    public PoseKeyframeLists leftPalmPoses;
    public PoseKeyframeLists rightPalmPoses;

    public AllKeyFrames(PoseKeyframeLists leftPalmPoses, PoseKeyframeLists rightPalmPoses)
    {
        this.leftPalmPoses = leftPalmPoses;
        this.rightPalmPoses = rightPalmPoses;
    }
}

[System.Serializable]
public class PoseKeyframeLists
{
    public List<SerializableKeyframe> keyframesPositionX;
    public List<SerializableKeyframe> keyframesPositionY;
    public List<SerializableKeyframe> keyframesPositionZ;
    public List<SerializableKeyframe> keyframesRotationX;
    public List<SerializableKeyframe> keyframesRotationY;
    public List<SerializableKeyframe> keyframesRotationZ;


    public PoseKeyframeLists(List<SerializableKeyframe> keyframesPositionX, List<SerializableKeyframe> keyframesPositionY, List<SerializableKeyframe> keyframesPositionZ, List<SerializableKeyframe> keyframesRotationX, List<SerializableKeyframe> keyframesRotationY, List<SerializableKeyframe> keyframesRotationZ)
    {
        this.keyframesPositionX = keyframesPositionX;
        this.keyframesPositionY = keyframesPositionY;
        this.keyframesPositionZ = keyframesPositionZ;
        this.keyframesRotationX = keyframesRotationX;
        this.keyframesRotationY = keyframesRotationY;
        this.keyframesRotationZ = keyframesRotationZ;
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