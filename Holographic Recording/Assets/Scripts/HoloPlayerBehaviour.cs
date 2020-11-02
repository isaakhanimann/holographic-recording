using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using TMPro;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


public class HoloPlayerBehaviour : MonoBehaviour
{

    public GameObject leftHand;
    public GameObject rightHand;
    public GameObject debugLogsObject;

    private GameObject instantiatedLeftHand;
    private GameObject instantiatedRightHand;
    private float lengthOfAnimation;


    public void PutHoloRecordingIntoPlayer(HoloRecording recording)
    {
        debugLogsObject.GetComponent<TextMeshPro>().text += "PutHoloRecordingIntoPlayer" + System.Environment.NewLine;
        InstantiateHand(leftHand, ref instantiatedLeftHand);
        InstantiateHand(rightHand, ref instantiatedRightHand);

        (AnimationClip leftHandClip, AnimationClip rightHandClip) = GetAnimationClipsFromPath(recording.pathToAnimationClip);
        debugLogsObject.GetComponent<TextMeshPro>().text += "AnimationClips were loaded" + System.Environment.NewLine;

        instantiatedLeftHand.GetComponent<Animation>().AddClip(leftHandClip, "leftHand");
        instantiatedRightHand.GetComponent<Animation>().AddClip(rightHandClip, "rightHand");
    }


    private void InstantiateHand(GameObject hand, ref GameObject instantiatedHand)
    {
        Quaternion rotationToInstantiate = Quaternion.identity;
        Vector3 positionToInstantiate = Vector3.zero;
        instantiatedHand = Instantiate(original: hand, position: positionToInstantiate, rotation: rotationToInstantiate);
        instantiatedHand.SetActive(false);
    }


    public void Play()
    {
        debugLogsObject.GetComponent<TextMeshPro>().text += "Play" + System.Environment.NewLine;
        instantiatedLeftHand.SetActive(true);
        instantiatedRightHand.SetActive(true);
        instantiatedLeftHand.GetComponent<Animation>().Play("leftHand");
        instantiatedRightHand.GetComponent<Animation>().Play("rightHand");
        StartCoroutine(SetInstancesInactive());
    }

    IEnumerator SetInstancesInactive()
    {
        debugLogsObject.GetComponent<TextMeshPro>().text += "SetInstanceInactive coroutine was called" + System.Environment.NewLine;
        yield return new WaitForSeconds(lengthOfAnimation);
        instantiatedLeftHand.SetActive(false);
        instantiatedRightHand.SetActive(false);
    }


    private (AnimationClip, AnimationClip) GetAnimationClipsFromPath(string path)
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream fileStream = File.Open(path, FileMode.Open);
        AllKeyFrames allKeyFrames = (AllKeyFrames)binaryFormatter.Deserialize(fileStream);
        fileStream.Close();

        SetLengthOfAnimation(allKeyFrames);
        AnimationClip leftClip = GetLeftAnimationClipFromRecordedKeyframes(allKeyFrames);
        AnimationClip rightClip = GetRightAnimationClipFromRecordedKeyframes(allKeyFrames);
        return (leftClip, rightClip);
    }

    private void SetLengthOfAnimation(AllKeyFrames allKeyFrames)
    {
        List<SerializableKeyframe> randomListOfLeftKeyframes = allKeyFrames.leftPalmPoses.keyframesPositionX;
        float leftTime = randomListOfLeftKeyframes[randomListOfLeftKeyframes.Count - 1].time;

        List<SerializableKeyframe> randomListOfRandomKeyframes = allKeyFrames.leftPalmPoses.keyframesPositionX;
        float rightTime = randomListOfRandomKeyframes[randomListOfRandomKeyframes.Count - 1].time;

        lengthOfAnimation = Math.Max(leftTime, rightTime);
    }


    private AnimationClip GetLeftAnimationClipFromRecordedKeyframes(AllKeyFrames allKeyFrames)
    {
        AnimationClip leftClip = new AnimationClip();
        leftClip.legacy = true;

        AddAnimationCurvesToPathInAnimationClip("", allKeyFrames.leftPalmPoses, ref leftClip);

        leftClip.EnsureQuaternionContinuity();

        return leftClip;
    }

    private AnimationClip GetRightAnimationClipFromRecordedKeyframes(AllKeyFrames allKeyFrames)
    {
        AnimationClip rightClip = new AnimationClip();
        rightClip.legacy = true;

        AddAnimationCurvesToPathInAnimationClip("", allKeyFrames.rightPalmPoses, ref rightClip);

        rightClip.EnsureQuaternionContinuity();

        return rightClip;
    }

    private void AddAnimationCurvesToPathInAnimationClip(string path, PoseKeyframeLists poseKeyframeLists, ref AnimationClip animationClip)
    {
        List<Keyframe> keyframesX = GetKeyframes(poseKeyframeLists.keyframesPositionX);
        List<Keyframe> keyframesY = GetKeyframes(poseKeyframeLists.keyframesPositionY);
        List<Keyframe> keyframesZ = GetKeyframes(poseKeyframeLists.keyframesPositionZ);
        List<Keyframe> keyframesRotationX = GetKeyframes(poseKeyframeLists.keyframesRotationX);
        List<Keyframe> keyframesRotationY = GetKeyframes(poseKeyframeLists.keyframesRotationY);
        List<Keyframe> keyframesRotationZ = GetKeyframes(poseKeyframeLists.keyframesRotationZ);
        List<Keyframe> keyframesRotationW = GetKeyframes(poseKeyframeLists.keyframesRotationW);

        AnimationCurve translateX = new AnimationCurve(keyframesX.ToArray());
        AnimationCurve translateY = new AnimationCurve(keyframesY.ToArray());
        AnimationCurve translateZ = new AnimationCurve(keyframesZ.ToArray());
        AnimationCurve rotateX = new AnimationCurve(keyframesRotationX.ToArray());
        AnimationCurve rotateY = new AnimationCurve(keyframesRotationY.ToArray());
        AnimationCurve rotateZ = new AnimationCurve(keyframesRotationZ.ToArray());
        AnimationCurve rotateW = new AnimationCurve(keyframesRotationW.ToArray());

        animationClip.SetCurve(path, typeof(Transform), "localPosition.x", translateX);
        animationClip.SetCurve(path, typeof(Transform), "localPosition.y", translateY);
        animationClip.SetCurve(path, typeof(Transform), "localPosition.z", translateZ);
        animationClip.SetCurve(path, typeof(Transform), "localRotation.x", rotateX);
        animationClip.SetCurve(path, typeof(Transform), "localRotation.y", rotateY);
        animationClip.SetCurve(path, typeof(Transform), "localRotation.z", rotateZ);
        animationClip.SetCurve(path, typeof(Transform), "localRotation.w", rotateW);
    }

    private List<Keyframe> GetKeyframes(List<SerializableKeyframe> serializableKeyframes)
    {
        List<Keyframe> keyframes = new List<Keyframe>();
        for (int index = 0; index < serializableKeyframes.Count; index++)
        {
            SerializableKeyframe serializableKeyframe = serializableKeyframes[index];
            keyframes.Add(new Keyframe(serializableKeyframe.time, serializableKeyframe.value));
        }

        return keyframes;
    }


}
