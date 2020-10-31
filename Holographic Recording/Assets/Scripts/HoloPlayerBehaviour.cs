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
        Vector3 positionToInstantiate = Camera.main.transform.position + 0.3f * Camera.main.transform.forward;
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
        List<Keyframe> keyframesX = GetKeyframes(allKeyFrames.palmPoses.keyframesPositionX);
        List<Keyframe> keyframesY = GetKeyframes(allKeyFrames.palmPoses.keyframesPositionY);
        List<Keyframe> keyframesZ = GetKeyframes(allKeyFrames.palmPoses.keyframesPositionZ);
        debugLogsObject.GetComponent<TextMeshPro>().text += "keyframesX Count = " + keyframesX.Count + System.Environment.NewLine;

        lengthOfAnimationInSeconds = keyframesX[keyframesX.Count-1].time;
        debugLogsObject.GetComponent<TextMeshPro>().text += "lengthOfAnimationInSeconds = " + lengthOfAnimationInSeconds + System.Environment.NewLine;

        AnimationCurve translateX = new AnimationCurve(keyframesX.ToArray());
        AnimationCurve translateY = new AnimationCurve(keyframesY.ToArray());
        AnimationCurve translateZ = new AnimationCurve(keyframesZ.ToArray());
        AnimationClip newClip = new AnimationClip();
        newClip.legacy = true;
        string pathToPalm = "";
        newClip.SetCurve(pathToPalm, typeof(Transform), "localPosition.x", translateX);
        newClip.SetCurve(pathToPalm, typeof(Transform), "localPosition.y", translateY);
        newClip.SetCurve(pathToPalm, typeof(Transform), "localPosition.z", translateZ);
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
