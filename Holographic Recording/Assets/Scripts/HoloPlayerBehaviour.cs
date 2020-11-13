using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using TMPro;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine.Networking;
using System.Threading.Tasks;
using UnityEngine.UI;

public class HoloPlayerBehaviour : MonoBehaviour
{

    public GameObject leftHand;
    public GameObject rightHand;
    public TextMeshPro debugLogTmPro;
    public GameObject firstRepresentation;
    public TextMeshPro titleOfRepresentation;
    public GameObject buttons;
    public GameObject secondRepresentation;
    public GameObject instructionObject;
    public RawImage screenshotRawImage;
    public AudioSource audioSource;

    private GameObject instantiatedLeftHand;
    private GameObject instantiatedRightHand;
    private float lengthOfAnimation;
    private TouchScreenKeyboard keyboard;
    private string keyboardText;

    private void Update()
    {
        if (keyboard != null)
        {
            keyboardText = keyboard.text;
            titleOfRepresentation.text = keyboardText;

            if (keyboard.status == TouchScreenKeyboard.Status.Done)
            {
                buttons.SetActive(true);
                instructionObject.SetActive(false);
            }
        }
    }

    public void OpenSystemKeyboard()
    {
        keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, false, false);
    }

    public void PutHoloRecordingIntoPlayer(HoloRecording recording)
    {
        StartCoroutine(AddScreenshotToRepresentation(recording.pathToScreenshot));
        audioSource.clip = recording.audioClip;
        debugLogTmPro.GetComponent<TextMeshPro>().text += "PutHoloRecordingIntoPlayer" + System.Environment.NewLine;
        OpenSystemKeyboard();
        instructionObject.SetActive(true);
        buttons.SetActive(false);
        InstantiateHand(leftHand, ref instantiatedLeftHand);
        InstantiateHand(rightHand, ref instantiatedRightHand);
        (AnimationClip leftHandClip, AnimationClip rightHandClip) = GetAnimationClipsFromAllKeyFrames(recording.allKeyFrames);
        debugLogTmPro.GetComponent<TextMeshPro>().text += "AnimationClips were loaded" + System.Environment.NewLine;
        instantiatedLeftHand.GetComponent<Animation>().AddClip(leftHandClip, "leftHand");
        instantiatedRightHand.GetComponent<Animation>().AddClip(rightHandClip, "rightHand");
    }

    IEnumerator AddScreenshotToRepresentation(string pathToScreenshot)
    {
        WWW www = new WWW(pathToScreenshot);
        while (!www.isDone)
            yield return null;
        screenshotRawImage.texture = www.texture;
    }


    private void InstantiateHand(GameObject hand, ref GameObject instantiatedHand)
    {
        Quaternion rotationToInstantiate = Quaternion.identity;
        Vector3 positionToInstantiate = Vector3.zero;
        instantiatedHand = Instantiate(original: hand, position: positionToInstantiate, rotation: rotationToInstantiate);
        instantiatedHand.SetActive(false);
    }

    public void DeleteRecording()
    {
        Destroy(gameObject);
    }


    public void Play()
    {       
        firstRepresentation.SetActive(false);
        secondRepresentation.SetActive(true);
        audioSource.Play();
        debugLogTmPro.GetComponent<TextMeshPro>().text += "Play" + System.Environment.NewLine;
        instantiatedLeftHand.SetActive(true);
        instantiatedRightHand.SetActive(true);
        instantiatedLeftHand.GetComponent<Animation>().Play("leftHand");
        instantiatedRightHand.GetComponent<Animation>().Play("rightHand");

        StartCoroutine(ResetRecording());
    }

    IEnumerator ResetRecording()
    {
        debugLogTmPro.GetComponent<TextMeshPro>().text += "SetInstanceInactive coroutine was called" + System.Environment.NewLine;
        yield return new WaitForSeconds(lengthOfAnimation);
        instantiatedLeftHand.SetActive(false);
        instantiatedRightHand.SetActive(false);
        firstRepresentation.SetActive(true);
        secondRepresentation.SetActive(false);
        audioSource.Stop();
    }


    private (AnimationClip, AnimationClip) GetAnimationClipsFromPath(string path)
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream fileStream = File.Open(path, FileMode.Open);
        AllKeyFrames allKeyFrames = (AllKeyFrames)binaryFormatter.Deserialize(fileStream);
        fileStream.Close();
        return GetAnimationClipsFromAllKeyFrames(allKeyFrames);
    }

    private (AnimationClip, AnimationClip) GetAnimationClipsFromAllKeyFrames(AllKeyFrames allKeyFrames)
    {
        SetLengthOfAnimation(allKeyFrames);
        AnimationClip leftClip = GetAnimationClipForOneHandFromRecordedKeyframes(allKeyFrames, Handedness.Left);
        AnimationClip rightClip = GetAnimationClipForOneHandFromRecordedKeyframes(allKeyFrames, Handedness.Right);
        return (leftClip, rightClip);
    }

    private void SetLengthOfAnimation(AllKeyFrames allKeyFrames)
    {
        List<SerializableKeyframe> randomListOfLeftKeyframes = allKeyFrames.leftJointLists.root.keyframesPositionX;
        float leftTime = randomListOfLeftKeyframes[randomListOfLeftKeyframes.Count - 1].time;
        List<SerializableKeyframe> randomListOfRandomKeyframes = allKeyFrames.rightJointLists.root.keyframesPositionX;
        float rightTime = randomListOfRandomKeyframes[randomListOfRandomKeyframes.Count - 1].time;
        lengthOfAnimation = Math.Max(leftTime, rightTime);
    }


    private AnimationClip GetAnimationClipForOneHandFromRecordedKeyframes(AllKeyFrames allKeyFrames, Handedness handedness)
    {
        AnimationClip animationClip = new AnimationClip();
        animationClip.legacy = true;
        if (handedness == Handedness.Left)
        {
            AddAnimationCurvesForAllJointsToClip(allKeyFrames.leftJointLists, ref animationClip);
        }
        else
        {
            AddAnimationCurvesForAllJointsToClip(allKeyFrames.rightJointLists, ref animationClip);
        }
        animationClip.EnsureQuaternionContinuity();
        return animationClip;
    }

    private void AddAnimationCurvesForAllJointsToClip(KeyFrameListsForAllHandJoints keyframesForJoints, ref AnimationClip animationClip)
    {
        AddAnimationCurvesForJointToAnimationClip(keyframesForJoints.root, ref animationClip);
        AddAnimationCurvesForJointToAnimationClip(keyframesForJoints.hand, ref animationClip);
        AddAnimationCurvesForJointToAnimationClip(keyframesForJoints.main, ref animationClip);
        AddAnimationCurvesForJointToAnimationClip(keyframesForJoints.wrist, ref animationClip);
        AddAnimationCurvesForJointToAnimationClip(keyframesForJoints.middle, ref animationClip);
        AddAnimationCurvesForJointToAnimationClip(keyframesForJoints.middle1, ref animationClip);
        AddAnimationCurvesForJointToAnimationClip(keyframesForJoints.middle2, ref animationClip);
        AddAnimationCurvesForJointToAnimationClip(keyframesForJoints.middle3, ref animationClip);
        AddAnimationCurvesForJointToAnimationClip(keyframesForJoints.middle3End, ref animationClip);
        AddAnimationCurvesForJointToAnimationClip(keyframesForJoints.pinky, ref animationClip);
        AddAnimationCurvesForJointToAnimationClip(keyframesForJoints.pinky1, ref animationClip);
        AddAnimationCurvesForJointToAnimationClip(keyframesForJoints.pinky2, ref animationClip);
        AddAnimationCurvesForJointToAnimationClip(keyframesForJoints.pinky3, ref animationClip);
        AddAnimationCurvesForJointToAnimationClip(keyframesForJoints.pinky3End, ref animationClip);
        AddAnimationCurvesForJointToAnimationClip(keyframesForJoints.point, ref animationClip);
        AddAnimationCurvesForJointToAnimationClip(keyframesForJoints.point1, ref animationClip);
        AddAnimationCurvesForJointToAnimationClip(keyframesForJoints.point2, ref animationClip);
        AddAnimationCurvesForJointToAnimationClip(keyframesForJoints.point3, ref animationClip);
        AddAnimationCurvesForJointToAnimationClip(keyframesForJoints.point3End, ref animationClip);
        AddAnimationCurvesForJointToAnimationClip(keyframesForJoints.ring, ref animationClip);
        AddAnimationCurvesForJointToAnimationClip(keyframesForJoints.ring1, ref animationClip);
        AddAnimationCurvesForJointToAnimationClip(keyframesForJoints.ring2, ref animationClip);
        AddAnimationCurvesForJointToAnimationClip(keyframesForJoints.ring3, ref animationClip);
        AddAnimationCurvesForJointToAnimationClip(keyframesForJoints.ring3End, ref animationClip);
        AddAnimationCurvesForJointToAnimationClip(keyframesForJoints.thumb1, ref animationClip);
        AddAnimationCurvesForJointToAnimationClip(keyframesForJoints.thumb2, ref animationClip);
        AddAnimationCurvesForJointToAnimationClip(keyframesForJoints.thumb3, ref animationClip);
        AddAnimationCurvesForJointToAnimationClip(keyframesForJoints.thumb3End, ref animationClip);
    }

    private void AddAnimationCurvesForJointToAnimationClip(PoseKeyframeLists poseKeyframeLists, ref AnimationClip animationClip)
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
        animationClip.SetCurve(poseKeyframeLists.path, typeof(Transform), "localPosition.x", translateX);
        animationClip.SetCurve(poseKeyframeLists.path, typeof(Transform), "localPosition.y", translateY);
        animationClip.SetCurve(poseKeyframeLists.path, typeof(Transform), "localPosition.z", translateZ);
        animationClip.SetCurve(poseKeyframeLists.path, typeof(Transform), "localRotation.x", rotateX);
        animationClip.SetCurve(poseKeyframeLists.path, typeof(Transform), "localRotation.y", rotateY);
        animationClip.SetCurve(poseKeyframeLists.path, typeof(Transform), "localRotation.z", rotateZ);
        animationClip.SetCurve(poseKeyframeLists.path, typeof(Transform), "localRotation.w", rotateW);
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

    private async Task<AudioClip> LoadClip(string path)
    {
        AudioClip clip = null;
        using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.WAV))
        {
            await uwr.SendWebRequest();
            // wrap tasks in try/catch, otherwise it'll fail silently
            try
            {
                while (!uwr.isDone) await Task.Delay(5);
                if (uwr.isNetworkError || uwr.isHttpError) Debug.Log($"{uwr.error}");
                else
                {
                    clip = DownloadHandlerAudioClip.GetContent(uwr);
                }
            }
            catch (Exception err)
            {
                debugLogTmPro.text += $"{err.Message}, {err.StackTrace}";
            }
        }
        return clip;
    }

    public void Stop()
    {
        secondRepresentation.SetActive(false);
        firstRepresentation.SetActive(true);
        instantiatedLeftHand.SetActive(false);
        instantiatedRightHand.SetActive(false);
        instantiatedLeftHand.GetComponent<Animation>().Stop();
        instantiatedRightHand.GetComponent<Animation>().Stop();
        audioSource.Stop();
    }
}
