using Microsoft.MixedReality.Toolkit.Utilities;
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
    public KeyFrameListsForAllHandJoints leftJointLists;
    public KeyFrameListsForAllHandJoints rightJointLists;

    public AllKeyFrames()
    {
        this.leftJointLists = new KeyFrameListsForAllHandJoints(Handedness.Left);
        this.rightJointLists = new KeyFrameListsForAllHandJoints(Handedness.Right);
    }
}

[System.Serializable]
public class KeyFrameListsForAllHandJoints
{
    public PoseKeyframeLists root;
    public PoseKeyframeLists hand;
    public PoseKeyframeLists main;
    public PoseKeyframeLists wrist;
    public PoseKeyframeLists middle;
    public PoseKeyframeLists middle1;
    public PoseKeyframeLists middle2;
    public PoseKeyframeLists middle3;
    public PoseKeyframeLists middle3End;
    public PoseKeyframeLists pinky;
    public PoseKeyframeLists pinky1;
    public PoseKeyframeLists pinky2;
    public PoseKeyframeLists pinky3;
    public PoseKeyframeLists pinky3End;
    public PoseKeyframeLists point;
    public PoseKeyframeLists point1;
    public PoseKeyframeLists point2;
    public PoseKeyframeLists point3;
    public PoseKeyframeLists point3End;
    public PoseKeyframeLists ring;
    public PoseKeyframeLists ring1;
    public PoseKeyframeLists ring2;
    public PoseKeyframeLists ring3;
    public PoseKeyframeLists ring3End;
    public PoseKeyframeLists thumb1;
    public PoseKeyframeLists thumb2;
    public PoseKeyframeLists thumb3;
    public PoseKeyframeLists thumb3End;

    public KeyFrameListsForAllHandJoints(Handedness handedness)
    {
        string LorR = (handedness == Handedness.Left) ? "L" : "R";

        this.root = new PoseKeyframeLists("");
        this.hand = new PoseKeyframeLists($"{LorR}_Hand");
        this.main = new PoseKeyframeLists($"{LorR}_Hand/Main{LorR}_JNT");
        this.wrist = new PoseKeyframeLists($"{LorR}_Hand/Main{LorR}_JNT/Wrist{LorR}_JNT");
        this.middle = new PoseKeyframeLists($"{LorR}_Hand/Main{LorR}_JNT/Wrist{LorR}_JNT/Middle{LorR}_JNT");
        this.middle1 = new PoseKeyframeLists($"{LorR}_Hand/Main{LorR}_JNT/Wrist{LorR}_JNT/Middle{LorR}_JNT/Middle{LorR}_JNT1");
        this.middle2 = new PoseKeyframeLists($"{LorR}_Hand/Main{LorR}_JNT/Wrist{LorR}_JNT/Middle{LorR}_JNT/Middle{LorR}_JNT1/Middel{LorR}_JNT2");
        this.middle3 = new PoseKeyframeLists($"{LorR}_Hand/Main{LorR}_JNT/Wrist{LorR}_JNT/Middle{LorR}_JNT/Middle{LorR}_JNT1/Middel{LorR}_JNT2/Middle{LorR}_JNT3");
        this.middle3End = new PoseKeyframeLists($"{LorR}_Hand/Main{LorR}_JNT/Wrist{LorR}_JNT/Middle{LorR}_JNT/Middle{LorR}_JNT1/Middel{LorR}_JNT2/Middle{LorR}_JNT3/Middle{LorR}_JNT3_end");
        this.pinky = new PoseKeyframeLists($"{LorR}_Hand/Main{LorR}_JNT/Wrist{LorR}_JNT/Pinky{LorR}_JNT");
        this.pinky1 = new PoseKeyframeLists($"{LorR}_Hand/Main{LorR}_JNT/Wrist{LorR}_JNT/Pinky{LorR}_JNT/Pinky{LorR}_JNT1");
        this.pinky2 = new PoseKeyframeLists($"{LorR}_Hand/Main{LorR}_JNT/Wrist{LorR}_JNT/Pinky{LorR}_JNT/Pinky{LorR}_JNT1/Pinky{LorR}_JNT2");
        this.pinky3 = new PoseKeyframeLists($"{LorR}_Hand/Main{LorR}_JNT/Wrist{LorR}_JNT/Pinky{LorR}_JNT/Pinky{LorR}_JNT1/Pinky{LorR}_JNT2/Pinky{LorR}_JNT3");
        this.pinky3End = new PoseKeyframeLists($"{LorR}_Hand/Main{LorR}_JNT/Wrist{LorR}_JNT/Pinky{LorR}_JNT/Pinky{LorR}_JNT1/Pinky{LorR}_JNT2/Pinky{LorR}_JNT3/Pinky{LorR}_JNT3_end");
        this.point = new PoseKeyframeLists($"{LorR}_Hand/Main{LorR}_JNT/Wrist{LorR}_JNT/Point{LorR}_JNT");
        this.point1 = new PoseKeyframeLists($"{LorR}_Hand/Main{LorR}_JNT/Wrist{LorR}_JNT/Point{LorR}_JNT/Point{LorR}_JNT1");
        this.point2 = new PoseKeyframeLists($"{LorR}_Hand/Main{LorR}_JNT/Wrist{LorR}_JNT/Point{LorR}_JNT/Point{LorR}_JNT1/Point{LorR}_JNT2");
        this.point3 = new PoseKeyframeLists($"{LorR}_Hand/Main{LorR}_JNT/Wrist{LorR}_JNT/Point{LorR}_JNT/Point{LorR}_JNT1/Point{LorR}_JNT2/Point{LorR}_JNT3");
        this.point3End = new PoseKeyframeLists($"{LorR}_Hand/Main{LorR}_JNT/Wrist{LorR}_JNT/Point{LorR}_JNT/Point{LorR}_JNT1/Point{LorR}_JNT2/Point{LorR}_JNT3/Point{LorR}_JNT3_end");
        this.ring = new PoseKeyframeLists($"{LorR}_Hand/Main{LorR}_JNT/Wrist{LorR}_JNT/Ring{LorR}_JNT");
        this.ring1 = new PoseKeyframeLists($"{LorR}_Hand/Main{LorR}_JNT/Wrist{LorR}_JNT/Ring{LorR}_JNT/Ring{LorR}_JNT1");
        this.ring2 = new PoseKeyframeLists($"{LorR}_Hand/Main{LorR}_JNT/Wrist{LorR}_JNT/Ring{LorR}_JNT/Ring{LorR}_JNT1/Ring{LorR}_JNT2");
        this.ring3 = new PoseKeyframeLists($"{LorR}_Hand/Main{LorR}_JNT/Wrist{LorR}_JNT/Ring{LorR}_JNT/Ring{LorR}_JNT1/Ring{LorR}_JNT2/Ring{LorR}_JNT3");
        this.ring3End = new PoseKeyframeLists($"{LorR}_Hand/Main{LorR}_JNT/Wrist{LorR}_JNT/Ring{LorR}_JNT/Ring{LorR}_JNT1/Ring{LorR}_JNT2/Ring{LorR}_JNT3/Ring{LorR}_JNT3_end");
        this.thumb1 = new PoseKeyframeLists($"{LorR}_Hand/Main{LorR}_JNT/Wrist{LorR}_JNT/Thumb{LorR}_JNT1");
        this.thumb2 = new PoseKeyframeLists($"{LorR}_Hand/Main{LorR}_JNT/Wrist{LorR}_JNT/Thumb{LorR}_JNT1/Thumb{LorR}_JNT2");
        this.thumb3 = new PoseKeyframeLists($"{LorR}_Hand/Main{LorR}_JNT/Wrist{LorR}_JNT/Thumb{LorR}_JNT1/Thumb{LorR}_JNT2/Thumb{LorR}_JNT3");
        this.thumb3End = new PoseKeyframeLists($"{LorR}_Hand/Main{LorR}_JNT/Wrist{LorR}_JNT/Thumb{LorR}_JNT1/Thumb{LorR}_JNT2/Thumb{LorR}_JNT3/Thumb{LorR}_JNT3_end");
    }
}

[System.Serializable]
public class PoseKeyframeLists
{
    public string path;
    public List<SerializableKeyframe> keyframesPositionX;
    public List<SerializableKeyframe> keyframesPositionY;
    public List<SerializableKeyframe> keyframesPositionZ;

    public List<SerializableKeyframe> keyframesRotationX;
    public List<SerializableKeyframe> keyframesRotationY;
    public List<SerializableKeyframe> keyframesRotationZ;
    public List<SerializableKeyframe> keyframesRotationW;

    public PoseKeyframeLists(string path)
    {
        this.path = path;
        this.keyframesPositionX = new List<SerializableKeyframe>();
        this.keyframesPositionY = new List<SerializableKeyframe>();
        this.keyframesPositionZ = new List<SerializableKeyframe>();
        this.keyframesRotationX = new List<SerializableKeyframe>();
        this.keyframesRotationY = new List<SerializableKeyframe>();
        this.keyframesRotationZ = new List<SerializableKeyframe>();
        this.keyframesRotationW = new List<SerializableKeyframe>();
    }

    public PoseKeyframeLists(string path, List<SerializableKeyframe> keyframesPositionX, List<SerializableKeyframe> keyframesPositionY, List<SerializableKeyframe> keyframesPositionZ, List<SerializableKeyframe> keyframesRotationX, List<SerializableKeyframe> keyframesRotationY, List<SerializableKeyframe> keyframesRotationZ, List<SerializableKeyframe> keyframesRotationW)
    {
        this.path = path;
        this.keyframesPositionX = keyframesPositionX;
        this.keyframesPositionY = keyframesPositionY;
        this.keyframesPositionZ = keyframesPositionZ;
        this.keyframesRotationX = keyframesRotationX;
        this.keyframesRotationY = keyframesRotationY;
        this.keyframesRotationZ = keyframesRotationZ;
        this.keyframesRotationW = keyframesRotationW;
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