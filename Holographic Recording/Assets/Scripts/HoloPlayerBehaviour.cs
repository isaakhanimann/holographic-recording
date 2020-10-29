using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit;
using System.Collections;
using System;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Utilities;





public class HoloPlayerBehaviour : MonoBehaviour
{ 

    public GameObject hands;

    private GameObject instantiatedHands;
    private Animator instantiatedHandsAnimator;
    private float lengthOfAnimationInSeconds;

    private IMixedRealityInputPlaybackService inputPlaybackService = null;
    public IMixedRealityInputPlaybackService PlaybackService
    {
        get
        {
            if (inputPlaybackService == null)
            {
                inputPlaybackService = CoreServices.GetInputSystemDataProvider<IMixedRealityInputPlaybackService>();
            }

            return inputPlaybackService;
        }
    }

    public void PutHoloRecordingIntoPlayer(HoloRecording recording)
    {
        Debug.Log("PutHoloRecordingIntoPlayer");
        InstantiateHandsAndSetInactive();
        if (!PlaybackService.LoadInputAnimation(recording.pathToInputAnimation))
        {
            throw new System.Exception("Input Animation could not be loaded");
        }

        InputAnimation inputAnimation = PlaybackService.Animation;
        AnimationClip animationClip = CreateAnimationClip(inputAnimation);

        // There needs to be an AnimatorOverrideController for every animation clip to be played on the object with the Animator
        AnimatorOverrideController animatorOverrideController = new AnimatorOverrideController(instantiatedHandsAnimator.runtimeAnimatorController);
        animatorOverrideController["Recorded"] = animationClip;
        instantiatedHandsAnimator.runtimeAnimatorController = animatorOverrideController;
        lengthOfAnimationInSeconds = animationClip.length;
    }

    private void InstantiateHandsAndSetInactive()
    {
        Debug.Log("InstantiateRecordedObjectAndSetInactive");
        Quaternion rotationToInstantiate = Quaternion.identity;
        Vector3 positionToInstantiate = Camera.main.transform.position + 0.3f * Vector3.forward;
        instantiatedHands = Instantiate(original: hands, position: positionToInstantiate, rotation: rotationToInstantiate);
        instantiatedHandsAnimator = instantiatedHands.GetComponent<Animator>();
        instantiatedHands.SetActive(false);
    }


    public void Play()
    {
        Debug.Log("Play");
        instantiatedHands.SetActive(true);
        instantiatedHandsAnimator.SetTrigger("Play");
        StartCoroutine(SetInstanceInactive());
    }

    IEnumerator SetInstanceInactive()
    {
        yield return new WaitForSeconds(lengthOfAnimationInSeconds);
        instantiatedHands.SetActive(false);

    }

    public void Pause()
    {
        Debug.Log("Pause is not implemented yet");
    }

    public void Stop()
    {
        Debug.Log("Stop is not implemented yet");
    }


    public AnimationClip CreateAnimationClip(InputAnimation inputAnimation)
    {
        Debug.Log("Creating Animation Clip");
        AnimationClip outputClip = new AnimationClip();
        GenerateData(inputAnimation, Handedness.Left, ref outputClip);
        GenerateData(inputAnimation, Handedness.Right, ref outputClip);
        return outputClip;
    }

    private void GenerateData(InputAnimation inputAnimation, Handedness hand, ref AnimationClip animationClip)
    {
        Dictionary<string, TrackedHandJoint> joints = GenerateJointDict(hand);

        foreach (KeyValuePair<string, TrackedHandJoint> entry in joints)
        {
            string jointPath = entry.Key;
            TrackedHandJoint joint = GeneratePath(jointPath, hand);

            InputAnimation.PoseCurves poseCurves = new InputAnimation.PoseCurves();
            inputAnimation.TryGetHandJointCurves(hand, joint, out poseCurves);

            animationClip.SetCurve(jointPath, typeof(Transform), "position.x", poseCurves.PositionX);
            animationClip.SetCurve(jointPath, typeof(Transform), "position.y", poseCurves.PositionY);
            animationClip.SetCurve(jointPath, typeof(Transform), "position.z", poseCurves.PositionZ);
            animationClip.SetCurve(jointPath, typeof(Transform), "rotation.x", poseCurves.RotationX);
            animationClip.SetCurve(jointPath, typeof(Transform), "rotation.y", poseCurves.RotationY);
            animationClip.SetCurve(jointPath, typeof(Transform), "rotation.z", poseCurves.RotationZ);
            animationClip.SetCurve(jointPath, typeof(Transform), "rotation.w", poseCurves.RotationW);
        }
    }

    private TrackedHandJoint GeneratePath(string path, Handedness hand)
    {
        Dictionary<string, TrackedHandJoint> joints = GenerateJointDict(hand);
        return joints[path];
    }

    private Dictionary<string, TrackedHandJoint> GenerateJointDict(Handedness hand)
    {
        string h = "L";
        if (hand == Handedness.Right)
        {
            h = "R";
        }
        Dictionary<string, TrackedHandJoint> joints = new Dictionary<string, TrackedHandJoint>()
        {
            { String.Format("HandRig_{0}/Main{0}_JNT", h), TrackedHandJoint.Palm },
            { String.Format("HandRig_{0}/Main{0}_JNT/Wrist{0}_JNT", h), TrackedHandJoint.Wrist },
            { String.Format("HandRig_{0}/Main{0}_JNT/Wrist{0}_JNT/Middle{0}_JNT", h), TrackedHandJoint.MiddleMetacarpal },
            { String.Format("HandRig_{0}/Main{0}_JNT/Wrist{0}_JNT/Middle{0}_JNT/Middle{0}_JNT1", h), TrackedHandJoint.MiddleKnuckle  },
            { String.Format("HandRig_{0}/Main{0}_JNT/Wrist{0}_JNT/Middle{0}_JNT/Middle{0}_JNT1/Middel{0}_JNT2", h), TrackedHandJoint.MiddleMiddleJoint },
            { String.Format("HandRig_{0}/Main{0}_JNT/Wrist{0}_JNT/Middle{0}_JNT/Middle{0}_JNT1/Middel{0}_JNT2/Middle{0}_JNT3", h), TrackedHandJoint.MiddleTip },
            { String.Format("HandRig_{0}/Main{0}_JNT/Wrist{0}_JNT/Pinky{0}_JNT", h), TrackedHandJoint.PinkyMetacarpal },
            { String.Format("HandRig_{0}/Main{0}_JNT/Wrist{0}_JNT/Pinky{0}_JNT/Pinky{0}_JNT1", h), TrackedHandJoint.PinkyKnuckle  },
            { String.Format("HandRig_{0}/Main{0}_JNT/Wrist{0}_JNT/Pinky{0}_JNT/Pinky{0}_JNT1/Pinky{0}_JNT2", h), TrackedHandJoint.PinkyMiddleJoint },
            { String.Format("HandRig_{0}/Main{0}_JNT/Wrist{0}_JNT/Pinky{0}_JNT/Pinky{0}_JNT1/Pinky{0}_JNT2/Pinky{0}_JNT3", h), TrackedHandJoint.PinkyTip },
            { String.Format("HandRig_{0}/Main{0}_JNT/Wrist{0}_JNT/Point{0}_JNT", h), TrackedHandJoint.IndexMetacarpal },
            { String.Format("HandRig_{0}/Main{0}_JNT/Wrist{0}_JNT/Point{0}_JNT/Point{0}_JNT1", h), TrackedHandJoint.IndexKnuckle },
            { String.Format("HandRig_{0}/Main{0}_JNT/Wrist{0}_JNT/Point{0}_JNT/Point{0}_JNT1/Point{0}_JNT2", h), TrackedHandJoint.IndexMiddleJoint },
            { String.Format("HandRig_{0}/Main{0}_JNT/Wrist{0}_JNT/Point{0}_JNT/Point{0}_JNT1/Point{0}_JNT2/Point{0}_JNT3", h), TrackedHandJoint.IndexTip },
            { String.Format("HandRig_{0}/Main{0}_JNT/Wrist{0}_JNT/Ring{0}_JNT", h), TrackedHandJoint.RingMetacarpal },
            { String.Format("HandRig_{0}/Main{0}_JNT/Wrist{0}_JNT/Ring{0}_JNT/Ring{0}_JNT1", h), TrackedHandJoint.RingKnuckle  },
            { String.Format("HandRig_{0}/Main{0}_JNT/Wrist{0}_JNT/Ring{0}_JNT/Ring{0}_JNT1/Ring{0}_JNT2", h), TrackedHandJoint.RingMiddleJoint },
            { String.Format("HandRig_{0}/Main{0}_JNT/Wrist{0}_JNT/Ring{0}_JNT/Ring{0}_JNT1/Ring{0}_JNT2/Ring{0}_JNT3", h), TrackedHandJoint.RingTip },
            { String.Format("HandRig_{0}/Main{0}_JNT/Wrist{0}_JNT/Thumb{0}_JNT1", h), TrackedHandJoint.ThumbMetacarpalJoint },
            { String.Format("HandRig_{0}/Main{0}_JNT/Wrist{0}_JNT/Thumb{0}_JNT1/Thumb{0}_JNT2", h), TrackedHandJoint.ThumbProximalJoint },
            { String.Format("HandRig_{0}/Main{0}_JNT/Wrist{0}_JNT/Thumb{0}_JNT1/Thumb{0}_JNT2/Thumb{0}_JNT3", h), TrackedHandJoint.ThumbTip }
        };
        return joints;
    }


}
