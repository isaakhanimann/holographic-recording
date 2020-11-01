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
        SerializableSnapshots serializableSnapshots = JsonUtility.FromJson<SerializableSnapshots>(keyframesAsJson);
        return GetAnimationClipFromRecordedKeyframes(serializableSnapshots);
    }


    private AnimationClip GetAnimationClipFromRecordedKeyframes(SerializableSnapshots serializableSnapshots)
    {
        int numberOfKeyframes = serializableSnapshots.snapshots.Count;
        lengthOfAnimationInSeconds = serializableSnapshots.snapshots[numberOfKeyframes - 1].time;

        Keyframe[] leftKeyframesX = new Keyframe[numberOfKeyframes];
        Keyframe[] leftKeyframesY = new Keyframe[numberOfKeyframes];
        Keyframe[] leftKeyframesZ = new Keyframe[numberOfKeyframes];
        Keyframe[] leftKeyframesRotationX = new Keyframe[numberOfKeyframes];
        Keyframe[] leftKeyframesRotationY = new Keyframe[numberOfKeyframes];
        Keyframe[] leftKeyframesRotationZ = new Keyframe[numberOfKeyframes];
        Keyframe[] leftKeyframesRotationW = new Keyframe[numberOfKeyframes];

        Keyframe[] rightKeyframesX = new Keyframe[numberOfKeyframes];
        Keyframe[] rightKeyframesY = new Keyframe[numberOfKeyframes];
        Keyframe[] rightKeyframesZ = new Keyframe[numberOfKeyframes];
        Keyframe[] rightKeyframesRotationX = new Keyframe[numberOfKeyframes];
        Keyframe[] rightKeyframesRotationY = new Keyframe[numberOfKeyframes];
        Keyframe[] rightKeyframesRotationZ = new Keyframe[numberOfKeyframes];
        Keyframe[] rightKeyframesRotationW = new Keyframe[numberOfKeyframes];
        for (int i = 0; i < numberOfKeyframes; i++)
        {
            Snapshot snapshot = serializableSnapshots.snapshots[i];
            float time = snapshot.time;

            leftKeyframesX[i] = new Keyframe(time, snapshot.leftPalm.positionX);
            leftKeyframesY[i] = new Keyframe(time, snapshot.leftPalm.positionY);
            leftKeyframesZ[i] = new Keyframe(time, snapshot.leftPalm.positionZ);
            leftKeyframesRotationX[i] = new Keyframe(time, snapshot.leftPalm.rotationX);
            leftKeyframesRotationY[i] = new Keyframe(time, snapshot.leftPalm.rotationY);
            leftKeyframesRotationZ[i] = new Keyframe(time, snapshot.leftPalm.rotationZ);
            leftKeyframesRotationW[i] = new Keyframe(time, snapshot.leftPalm.rotationW);

            rightKeyframesX[i] = new Keyframe(time, snapshot.rightPalm.positionX);
            rightKeyframesY[i] = new Keyframe(time, snapshot.rightPalm.positionY);
            rightKeyframesZ[i] = new Keyframe(time, snapshot.rightPalm.positionZ);
            rightKeyframesRotationX[i] = new Keyframe(time, snapshot.rightPalm.rotationX);
            rightKeyframesRotationY[i] = new Keyframe(time, snapshot.rightPalm.rotationY);
            rightKeyframesRotationZ[i] = new Keyframe(time, snapshot.rightPalm.rotationZ);
            rightKeyframesRotationW[i] = new Keyframe(time, snapshot.rightPalm.rotationW);
        }
        

        AnimationCurve leftTranslateX = new AnimationCurve(leftKeyframesX);
        AnimationCurve leftTranslateY = new AnimationCurve(leftKeyframesY);
        AnimationCurve leftTranslateZ = new AnimationCurve(leftKeyframesZ);
        AnimationCurve leftRotateX = new AnimationCurve(leftKeyframesRotationX);
        AnimationCurve leftRotateY = new AnimationCurve(leftKeyframesRotationY);
        AnimationCurve leftRotateZ = new AnimationCurve(leftKeyframesRotationZ);
        AnimationCurve leftRotateW = new AnimationCurve(leftKeyframesRotationW);

        AnimationCurve rightTranslateX = new AnimationCurve(rightKeyframesX);
        AnimationCurve rightTranslateY = new AnimationCurve(rightKeyframesY);
        AnimationCurve rightTranslateZ = new AnimationCurve(rightKeyframesZ);
        AnimationCurve rightRotateX = new AnimationCurve(rightKeyframesRotationX);
        AnimationCurve rightRotateY = new AnimationCurve(rightKeyframesRotationY);
        AnimationCurve rightRotateZ = new AnimationCurve(rightKeyframesRotationZ);
        AnimationCurve rightRotateW = new AnimationCurve(rightKeyframesRotationW);


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



}
