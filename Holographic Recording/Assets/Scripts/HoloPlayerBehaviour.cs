using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using TMPro;
using System.IO;


public class HoloPlayerBehaviour : MonoBehaviour
{

    public GameObject hands;
    public GameObject debugLogsObject;

    private GameObject instantiatedHands;
    private Animation instantiatedHandsAnimation;
    private float lengthOfAnimationInSeconds;


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
        yield return new WaitForSeconds(lengthOfAnimationInSeconds);
        instantiatedHands.SetActive(false);
    }


    private AnimationClip GetAnimationClipFromPath(string path)
    {
        string keyframesAsJson = File.ReadAllText(path);
        AllKeyFrames allKeyFrames = JsonUtility.FromJson<AllKeyFrames>(keyframesAsJson);
        return GetAnimationClipFromRecordedKeyframes(allKeyFrames);
    }


    private AnimationClip GetAnimationClipFromRecordedKeyframes(AllKeyFrames allKeyFrames)
    {
        List<Keyframe> leftKeyframesX = GetKeyframes(allKeyFrames.leftPalmPoses.keyframesPositionX);
        List<Keyframe> leftKeyframesY = GetKeyframes(allKeyFrames.leftPalmPoses.keyframesPositionY);
        List<Keyframe> leftKeyframesZ = GetKeyframes(allKeyFrames.leftPalmPoses.keyframesPositionZ);
        List<Keyframe> leftKeyframesRotationX = GetKeyframes(allKeyFrames.leftPalmPoses.keyframesRotationX);
        List<Keyframe> leftKeyframesRotationY = GetKeyframes(allKeyFrames.leftPalmPoses.keyframesRotationY);
        List<Keyframe> leftKeyframesRotationZ = GetKeyframes(allKeyFrames.leftPalmPoses.keyframesRotationZ);
        List<Keyframe> leftKeyframesRotationW = GetKeyframes(allKeyFrames.leftPalmPoses.keyframesRotationW);

        AnimationCurve leftTranslateX = new AnimationCurve(leftKeyframesX.ToArray());
        AnimationCurve leftTranslateY = new AnimationCurve(leftKeyframesY.ToArray());
        AnimationCurve leftTranslateZ = new AnimationCurve(leftKeyframesZ.ToArray());
        AnimationCurve leftRotateX = new AnimationCurve(leftKeyframesRotationX.ToArray());
        AnimationCurve leftRotateY = new AnimationCurve(leftKeyframesRotationY.ToArray());
        AnimationCurve leftRotateZ = new AnimationCurve(leftKeyframesRotationZ.ToArray());
        AnimationCurve leftRotateW = new AnimationCurve(leftKeyframesRotationW.ToArray());

        List<Keyframe> rightKeyframesX = GetKeyframes(allKeyFrames.rightPalmPoses.keyframesPositionX);
        List<Keyframe> rightKeyframesY = GetKeyframes(allKeyFrames.rightPalmPoses.keyframesPositionY);
        List<Keyframe> rightKeyframesZ = GetKeyframes(allKeyFrames.rightPalmPoses.keyframesPositionZ);
        List<Keyframe> rightKeyframesRotationX = GetKeyframes(allKeyFrames.rightPalmPoses.keyframesRotationX);
        List<Keyframe> rightKeyframesRotationY = GetKeyframes(allKeyFrames.rightPalmPoses.keyframesRotationY);
        List<Keyframe> rightKeyframesRotationZ = GetKeyframes(allKeyFrames.rightPalmPoses.keyframesRotationZ);
        List<Keyframe> rightKeyframesRotationW = GetKeyframes(allKeyFrames.rightPalmPoses.keyframesRotationW);

        AnimationCurve rightTranslateX = new AnimationCurve(rightKeyframesX.ToArray());
        AnimationCurve rightTranslateY = new AnimationCurve(rightKeyframesY.ToArray());
        AnimationCurve rightTranslateZ = new AnimationCurve(rightKeyframesZ.ToArray());
        AnimationCurve rightRotateX = new AnimationCurve(rightKeyframesRotationX.ToArray());
        AnimationCurve rightRotateY = new AnimationCurve(rightKeyframesRotationY.ToArray());
        AnimationCurve rightRotateZ = new AnimationCurve(rightKeyframesRotationZ.ToArray());
        AnimationCurve rightRotateW = new AnimationCurve(rightKeyframesRotationW.ToArray());

        lengthOfAnimationInSeconds = leftKeyframesX[leftKeyframesX.Count - 1].time;

        AnimationClip newClip = new AnimationClip();
        newClip.legacy = true;
        string pathToLeftPalm = "HandRig_L";
        newClip.SetCurve(pathToLeftPalm, typeof(Transform), "localPosition.x", leftTranslateX);
        newClip.SetCurve(pathToLeftPalm, typeof(Transform), "localPosition.y", leftTranslateY);
        newClip.SetCurve(pathToLeftPalm, typeof(Transform), "localPosition.z", leftTranslateZ);
        newClip.SetCurve(pathToLeftPalm, typeof(Transform), "localRotation.x", leftRotateX);
        newClip.SetCurve(pathToLeftPalm, typeof(Transform), "localRotation.y", leftRotateY);
        newClip.SetCurve(pathToLeftPalm, typeof(Transform), "localRotation.z", leftRotateZ);
        newClip.SetCurve(pathToLeftPalm, typeof(Transform), "localRotation.w", leftRotateW);

        string pathToRightPalm = "HandRig_R";
        newClip.SetCurve(pathToRightPalm, typeof(Transform), "localPosition.x", rightTranslateX);
        newClip.SetCurve(pathToRightPalm, typeof(Transform), "localPosition.y", rightTranslateY);
        newClip.SetCurve(pathToRightPalm, typeof(Transform), "localPosition.z", rightTranslateZ);
        newClip.SetCurve(pathToRightPalm, typeof(Transform), "localRotation.x", rightRotateX);
        newClip.SetCurve(pathToRightPalm, typeof(Transform), "localRotation.y", rightRotateY);
        newClip.SetCurve(pathToRightPalm, typeof(Transform), "localRotation.z", rightRotateZ);
        newClip.SetCurve(pathToRightPalm, typeof(Transform), "localRotation.w", rightRotateW);
        return newClip;
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
