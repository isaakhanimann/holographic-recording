using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using TMPro;
using System.IO;
using JetBrains.Annotations;

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
        return CreateAnimationClipFromRecordedSnapshots(serializableSnapshots);
    }

    private AnimationClip CreateAnimationClipFromRecordedSnapshots(SerializableSnapshots serializableSnapshots)
    {
        int numberOfKeyframes = serializableSnapshots.snapshots.Count;
        lengthOfAnimationInSeconds = serializableSnapshots.snapshots[numberOfKeyframes - 1].time;

        CurvesForAllJoints curvesForAllJoints = CreateCurvesForAllJoints(serializableSnapshots.snapshots);

        return CreateAnimationClipFromCurves(curvesForAllJoints);
    }

    private AnimationClip CreateAnimationClipFromCurves(CurvesForAllJoints curvesForAllJoints)
    {
        AnimationClip newClip = new AnimationClip();
        newClip.legacy = true;
        string pathToLeftPalm = "HandRig_L";
        newClip.SetCurve(pathToLeftPalm, typeof(Transform), "localPosition.x", curvesForAllJoints.leftPalmCurves.translateX);
        newClip.SetCurve(pathToLeftPalm, typeof(Transform), "localPosition.y", curvesForAllJoints.leftPalmCurves.translateY);
        newClip.SetCurve(pathToLeftPalm, typeof(Transform), "localPosition.z", curvesForAllJoints.leftPalmCurves.translateZ);
        newClip.SetCurve(pathToLeftPalm, typeof(Transform), "localRotation.x", curvesForAllJoints.leftPalmCurves.rotateX);
        newClip.SetCurve(pathToLeftPalm, typeof(Transform), "localRotation.y", curvesForAllJoints.leftPalmCurves.rotateY);
        newClip.SetCurve(pathToLeftPalm, typeof(Transform), "localRotation.z", curvesForAllJoints.leftPalmCurves.rotateZ);
        newClip.SetCurve(pathToLeftPalm, typeof(Transform), "localRotation.w", curvesForAllJoints.leftPalmCurves.rotateW);

        string pathToRightPalm = "HandRig_R";
        newClip.SetCurve(pathToRightPalm, typeof(Transform), "localPosition.x", curvesForAllJoints.rightPalmCurves.translateX);
        newClip.SetCurve(pathToRightPalm, typeof(Transform), "localPosition.y", curvesForAllJoints.rightPalmCurves.translateY);
        newClip.SetCurve(pathToRightPalm, typeof(Transform), "localPosition.z", curvesForAllJoints.rightPalmCurves.translateZ);
        newClip.SetCurve(pathToRightPalm, typeof(Transform), "localRotation.x", curvesForAllJoints.rightPalmCurves.rotateX);
        newClip.SetCurve(pathToRightPalm, typeof(Transform), "localRotation.y", curvesForAllJoints.rightPalmCurves.rotateY);
        newClip.SetCurve(pathToRightPalm, typeof(Transform), "localRotation.z", curvesForAllJoints.rightPalmCurves.rotateZ);
        newClip.SetCurve(pathToRightPalm, typeof(Transform), "localRotation.w", curvesForAllJoints.rightPalmCurves.rotateW);
        return newClip;
    }

    private CurvesForAllJoints CreateCurvesForAllJoints(List<Snapshot> snapshots)
    {
        int numberOfKeyframes = snapshots.Count;

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
            Snapshot snapshot = snapshots[i];
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

        JointCurves leftPalmCurves = new JointCurves();
        leftPalmCurves.translateX = new AnimationCurve(leftKeyframesX);
        leftPalmCurves.translateY = new AnimationCurve(leftKeyframesY);
        leftPalmCurves.translateZ = new AnimationCurve(leftKeyframesZ);
        leftPalmCurves.rotateX = new AnimationCurve(leftKeyframesRotationX);
        leftPalmCurves.rotateY = new AnimationCurve(leftKeyframesRotationY);
        leftPalmCurves.rotateZ = new AnimationCurve(leftKeyframesRotationZ);
        leftPalmCurves.rotateW = new AnimationCurve(leftKeyframesRotationW);

        JointCurves rightPalmCurves = new JointCurves();
        rightPalmCurves.translateX = new AnimationCurve(rightKeyframesX);
        rightPalmCurves.translateY = new AnimationCurve(rightKeyframesY);
        rightPalmCurves.translateZ = new AnimationCurve(rightKeyframesZ);
        rightPalmCurves.rotateX = new AnimationCurve(rightKeyframesRotationX);
        rightPalmCurves.rotateY = new AnimationCurve(rightKeyframesRotationY);
        rightPalmCurves.rotateZ = new AnimationCurve(rightKeyframesRotationZ);
        rightPalmCurves.rotateW = new AnimationCurve(rightKeyframesRotationW);

        CurvesForAllJoints allCurves = new CurvesForAllJoints();

        allCurves.leftPalmCurves = leftPalmCurves;
        allCurves.rightPalmCurves = rightPalmCurves;
        return allCurves;
    }

    private class CurvesForAllJoints
    {
        public JointCurves leftPalmCurves;
        public JointCurves rightPalmCurves;
    }

    private class JointCurves
    {
        public AnimationCurve translateX;
        public AnimationCurve translateY;
        public AnimationCurve translateZ;

        public AnimationCurve rotateX;
        public AnimationCurve rotateY;
        public AnimationCurve rotateZ;
        public AnimationCurve rotateW;
    }



}



