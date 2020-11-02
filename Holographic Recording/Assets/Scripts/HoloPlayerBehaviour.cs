using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using TMPro;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


public class HoloPlayerBehaviour : MonoBehaviour
{

    public GameObject hands;
    public GameObject debugLogsObject;

    private GameObject instantiatedHands;
    private Animation instantiatedHandsAnimation;
    private float lengthOfAnimation;


    public void PutHoloRecordingIntoPlayer(HoloRecording recording)
    {
        debugLogsObject.GetComponent<TextMeshPro>().text += "PutHoloRecordingIntoPlayer" + System.Environment.NewLine;
        InstantiateHandsAndSetInactive();

        AnimationClip animationClip = GetAnimationClipFromPath(recording.pathToAnimationClip);
        debugLogsObject.GetComponent<TextMeshPro>().text += "AnimationClip was loaded" + System.Environment.NewLine;
        instantiatedHandsAnimation.AddClip(animationClip, "test");
    }

    private void InstantiateHandsAndSetInactive()
    {
        debugLogsObject.GetComponent<TextMeshPro>().text += "InstantiateHandsAndSetInactive" + System.Environment.NewLine;
        Quaternion rotationToInstantiate = Quaternion.identity;
        Vector3 positionToInstantiate = Vector3.zero;
        instantiatedHands = Instantiate(original: hands, position: positionToInstantiate, rotation: rotationToInstantiate);
        instantiatedHandsAnimation = instantiatedHands.GetComponent<Animation>();
        instantiatedHands.SetActive(false);
    }


    public void Play()
    {
        debugLogsObject.GetComponent<TextMeshPro>().text += "Play" + System.Environment.NewLine;
        instantiatedHands.SetActive(true);
        instantiatedHandsAnimation.Play("test");
        StartCoroutine(SetInstanceInactive());
    }

    IEnumerator SetInstanceInactive()
    {
        debugLogsObject.GetComponent<TextMeshPro>().text += "SetInstanceInactive coroutine was called" + System.Environment.NewLine;
        yield return new WaitForSeconds(lengthOfAnimation);
        instantiatedHands.SetActive(false);
    }


    private AnimationClip GetAnimationClipFromPath(string path)
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream fileStream = File.Open(path, FileMode.Open);
        AllKeyFrames allKeyFrames = (AllKeyFrames)binaryFormatter.Deserialize(fileStream);
        fileStream.Close();

        SetLengthOfAnimation(allKeyFrames);
        return GetAnimationClipFromRecordedKeyframes(allKeyFrames);
    }

    private void SetLengthOfAnimation(AllKeyFrames allKeyFrames)
    {
        List<SerializableKeyframe> randomListOfLeftKeyframes = allKeyFrames.leftPalmPoses.keyframesPositionX;
        float leftTime = randomListOfLeftKeyframes[randomListOfLeftKeyframes.Count - 1].time;

        List<SerializableKeyframe> randomListOfRandomKeyframes = allKeyFrames.leftPalmPoses.keyframesPositionX;
        float rightTime = randomListOfRandomKeyframes[randomListOfRandomKeyframes.Count - 1].time;

        lengthOfAnimation = Math.Max(leftTime, rightTime);
    }


    private AnimationClip GetAnimationClipFromRecordedKeyframes(AllKeyFrames allKeyFrames)
    {
        AnimationClip newClip = new AnimationClip();
        newClip.legacy = true;

        AddAnimationCurvesToPathInAnimationClip("HandRig_L", allKeyFrames.leftPalmPoses, ref newClip);
        AddAnimationCurvesToPathInAnimationClip("HandRig_R", allKeyFrames.rightPalmPoses, ref newClip);

        newClip.EnsureQuaternionContinuity();

        return newClip;
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
