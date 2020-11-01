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
}


[System.Serializable]
public class SerializableSnapshots
{
    public List<Snapshot> snapshots;

    public SerializableSnapshots(List<Snapshot> snapshots)
    {
        this.snapshots = snapshots;
    }
}

[System.Serializable]
public class Snapshot
{
    public float time;

    public JointPose leftPalm;
    public JointPose rightPalm;

    public Snapshot(float time, JointPose leftPalm, JointPose rightPalm)
    {
        this.time = time;
        this.leftPalm = leftPalm;
        this.rightPalm = rightPalm;
    }
}


[System.Serializable]
public class JointPose
{
    public float positionX;
    public float positionY;
    public float positionZ;
                 
    public float rotationX;
    public float rotationY;
    public float rotationZ;
    public float rotationW;

    public JointPose(float positionX, float positionY, float positionZ, float rotationX, float rotationY, float rotationZ, float rotationW)
    {
        this.positionX = positionX;
        this.positionY = positionY;
        this.positionZ = positionZ;
        this.rotationX = rotationX;
        this.rotationY = rotationY;
        this.rotationZ = rotationZ;
        this.rotationW = rotationW;
    }
}