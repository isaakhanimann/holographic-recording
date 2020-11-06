using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct HoloRecording
{
    public string pathToAnimationClip;
    public string animationClipName;
    public AllKeyFrames allKeyFrames;
    public HoloRecording(string pathToAnimationClip, string animationClipName, AllKeyFrames allKeyFrames)
    {
        this.pathToAnimationClip = pathToAnimationClip;
        this.animationClipName = animationClipName;
        this.allKeyFrames = allKeyFrames;
    }

    public override string ToString() => $"HoloRecording: animation clip is called {animationClipName}";

}


[System.Serializable]
public struct AllKeyFrames
{
    public KeyFrameListsForAllHandJoints leftJointLists;
    public KeyFrameListsForAllHandJoints rightJointLists;
}

[System.Serializable]
public struct KeyFrameListsForAllHandJoints
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
        this.main = new PoseKeyframeLists($"{LorR}_Hand/MainL_JNT");
        this.wrist = new PoseKeyframeLists($"{LorR}_Hand/MainL_JNT/WristL_JNT");
        this.middle = new PoseKeyframeLists($"{LorR}_Hand/MainL_JNT/WristL_JNT/MiddleL_JNT");
        this.middle1 = new PoseKeyframeLists($"{LorR}_Hand/MainL_JNT/WristL_JNT/MiddleL_JNT/MiddleL_JNT1");
        this.middle2 = new PoseKeyframeLists($"{LorR}_Hand/MainL_JNT/WristL_JNT/MiddleL_JNT/MiddleL_JNT1/MiddelL_JNT2");
        this.middle3 = new PoseKeyframeLists($"{LorR}_Hand/MainL_JNT/WristL_JNT/MiddleL_JNT/MiddleL_JNT1/MiddelL_JNT2/MiddleL_JNT3");
        this.middle3End = new PoseKeyframeLists($"{LorR}_Hand/MainL_JNT/WristL_JNT/MiddleL_JNT/MiddleL_JNT1/MiddelL_JNT2/MiddleL_JNT3/MiddleL_JNT3_end");
        this.pinky = new PoseKeyframeLists($"{LorR}_Hand/MainL_JNT/WristL_JNT/PinkyL_JNT");
        this.pinky1 = new PoseKeyframeLists($"{LorR}_Hand/MainL_JNT/WristL_JNT/PinkyL_JNT/PinkyL_JNT1");
        this.pinky2 = new PoseKeyframeLists($"{LorR}_Hand/MainL_JNT/WristL_JNT/PinkyL_JNT/PinkyL_JNT1/PinkyL_JNT2");
        this.pinky3 = new PoseKeyframeLists($"{LorR}_Hand/MainL_JNT/WristL_JNT/PinkyL_JNT/PinkyL_JNT1/PinkyL_JNT2/PinkyL_JNT3");
        this.pinky3End = new PoseKeyframeLists($"{LorR}_Hand/MainL_JNT/WristL_JNT/PinkyL_JNT/PinkyL_JNT1/PinkyL_JNT2/PinkyL_JNT3/PinkyL_JNT3_end");
        this.point = new PoseKeyframeLists($"{LorR}_Hand/MainL_JNT/WristL_JNT/PointL_JNT");
        this.point1 = new PoseKeyframeLists($"{LorR}_Hand/MainL_JNT/WristL_JNT/PointL_JNT/PointL_JNT1");
        this.point2 = new PoseKeyframeLists($"{LorR}_Hand/MainL_JNT/WristL_JNT/PointL_JNT/PointL_JNT1/PointL_JNT2");
        this.point3 = new PoseKeyframeLists($"{LorR}_Hand/MainL_JNT/WristL_JNT/PointL_JNT/PointL_JNT1/PointL_JNT2/PointL_JNT3");
        this.point3End = new PoseKeyframeLists($"{LorR}_Hand/MainL_JNT/WristL_JNT/PointL_JNT/PointL_JNT1/PointL_JNT2/PointL_JNT3/PointL_JNT3_end");
        this.ring = new PoseKeyframeLists($"{LorR}_Hand/MainL_JNT/WristL_JNT/RingL_JNT");
        this.ring1 = new PoseKeyframeLists($"{LorR}_Hand/MainL_JNT/WristL_JNT/RingL_JNT/RingL_JNT1");
        this.ring2 = new PoseKeyframeLists($"{LorR}_Hand/MainL_JNT/WristL_JNT/RingL_JNT/RingL_JNT1/RingL_JNT2");
        this.ring3 = new PoseKeyframeLists($"{LorR}_Hand/MainL_JNT/WristL_JNT/RingL_JNT/RingL_JNT1/RingL_JNT2/RingL_JNT3");
        this.ring3End = new PoseKeyframeLists($"{LorR}_Hand/MainL_JNT/WristL_JNT/RingL_JNT/RingL_JNT1/RingL_JNT2/RingL_JNT3/RingL_JNT3_end");
        this.thumb1 = new PoseKeyframeLists($"{LorR}_Hand/MainL_JNT/WristL_JNT/ThumbL_JNT1");
        this.thumb2 = new PoseKeyframeLists($"{LorR}_Hand/MainL_JNT/WristL_JNT/ThumbL_JNT1/ThumbL_JNT2");
        this.thumb3 = new PoseKeyframeLists($"{LorR}_Hand/MainL_JNT/WristL_JNT/ThumbL_JNT1/ThumbL_JNT2/ThumbL_JNT3");
        this.thumb3End = new PoseKeyframeLists($"{LorR}_Hand/MainL_JNT/WristL_JNT/ThumbL_JNT1/ThumbL_JNT2/ThumbL_JNT3/ThumbL_JNT3_end");
    }
}

[System.Serializable]
public struct PoseKeyframeLists
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