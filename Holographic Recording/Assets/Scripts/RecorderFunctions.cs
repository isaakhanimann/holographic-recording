using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit;

public class RecorderFunctions : MonoBehaviour
{
    public GameObject recordingRepresentationPrefab;

    private GameObject recordingRepresentationInstance;

    private IMixedRealityHandJointService handJointService;

    void Start()
    {
        handJointService = CoreServices.GetInputSystemDataProvider<IMixedRealityHandJointService>();
    }


    public void StartRecordingAndInstantiateRepresentation()
    {
        StartRecording();
        InstantiateRecordingRepresentationAtPalm();
    }

    private void InstantiateRecordingRepresentationAtPalm()
    {
        Vector3 positionToInstantiate;
        Quaternion rotationToInstantiate = Quaternion.identity;
        if (HandJointUtils.TryGetJointPose(TrackedHandJoint.Palm, Handedness.Left, out MixedRealityPose pose))
        {
            positionToInstantiate = pose.Position;
        }
        else
        {
            positionToInstantiate = Camera.main.transform.position + 0.5f * Vector3.forward;
        }
        recordingRepresentationInstance = Instantiate(original: recordingRepresentationPrefab, position: positionToInstantiate, rotation: rotationToInstantiate);
    }

    public void StopRecordingAndPutRecordingIntoRepresentation()
    {
        HoloRecording newRecording = StopRecording();
        HoloPlayerBehaviour playerComponent = recordingRepresentationInstance.GetComponent<HoloPlayerBehaviour>();
        playerComponent.PutHoloRecordingIntoPlayer(newRecording);
    }

    public void CancelRecordingAndRemoveRepresentation()
    {
        CancelRecording();
        Destroy(recordingRepresentationInstance);
    }


    private bool isRecording;

    public void StartRecording()
    {
        isRecording = true;
    }

    public void CancelRecording()
    {
        isRecording = false;
        ResetRecorder();
    }

    public HoloRecording StopRecording()
    {
        isRecording = false;
        HoloRecording newRecording = SaveRecording();
        ResetRecorder();
        return newRecording;
    }

    private HoloRecording SaveRecording()
    {
        string animationClipName = "AnimationClip" + GetRandomNumberBetween1and100000();
        string pathToAnimationClip = SaveKeyframes(animationClipName);
        Debug.Log($"AnimationClip saved under {pathToAnimationClip}");
        HoloRecording newRecording = new HoloRecording(pathToAnimationClip, animationClipName);
        return newRecording;
    }


    private string SaveKeyframes(string filename)
    {
        string path = Application.persistentDataPath + $"/{filename}.txt";
        PoseKeyframeLists leftPalmPoses = new PoseKeyframeLists(leftPalmKeyframesX, leftPalmKeyframesY, leftPalmKeyframesZ, leftPalmKeyframesRotationX, leftPalmKeyframesRotationY, leftPalmKeyframesRotationZ, leftPalmKeyframesRotationW);
        PoseKeyframeLists rightPalmPoses = new PoseKeyframeLists(rightPalmKeyframesX, rightPalmKeyframesY, rightPalmKeyframesZ, rightPalmKeyframesRotationX, rightPalmKeyframesRotationY, rightPalmKeyframesRotationZ, rightPalmKeyframesRotationW);
        AllKeyFrames allKeyFrames = new AllKeyFrames(leftPalmPoses, rightPalmPoses);
        string keyframesAsJson = JsonUtility.ToJson(allKeyFrames, true);
        File.WriteAllText(path, keyframesAsJson);
        return path;
    }


    private int GetRandomNumberBetween1and100000()
    {
        System.Random random = new System.Random();
        int randomInt = random.Next(1, 100001);
        return randomInt;
    }

    void LateUpdate()
    {
        if (isRecording)
        {
            CaptureKeyFrames();
        }
    }

    private float timeOfLastUpdate = 0.0f;
    private List<SerializableKeyframe> leftPalmKeyframesX = new List<SerializableKeyframe>();
    private List<SerializableKeyframe> leftPalmKeyframesY = new List<SerializableKeyframe>();
    private List<SerializableKeyframe> leftPalmKeyframesZ = new List<SerializableKeyframe>();
    private List<SerializableKeyframe> leftPalmKeyframesRotationX = new List<SerializableKeyframe>();
    private List<SerializableKeyframe> leftPalmKeyframesRotationY = new List<SerializableKeyframe>();
    private List<SerializableKeyframe> leftPalmKeyframesRotationZ = new List<SerializableKeyframe>();
    private List<SerializableKeyframe> leftPalmKeyframesRotationW = new List<SerializableKeyframe>();

    private List<SerializableKeyframe> rightPalmKeyframesX = new List<SerializableKeyframe>();
    private List<SerializableKeyframe> rightPalmKeyframesY = new List<SerializableKeyframe>();
    private List<SerializableKeyframe> rightPalmKeyframesZ = new List<SerializableKeyframe>();
    private List<SerializableKeyframe> rightPalmKeyframesRotationX = new List<SerializableKeyframe>();
    private List<SerializableKeyframe> rightPalmKeyframesRotationY = new List<SerializableKeyframe>();
    private List<SerializableKeyframe> rightPalmKeyframesRotationZ = new List<SerializableKeyframe>();
    private List<SerializableKeyframe> rightPalmKeyframesRotationW = new List<SerializableKeyframe>();



    private void CaptureKeyFrames()
    {
        float timeOfKeyFrame = timeOfLastUpdate + Time.deltaTime;

        Transform leftPalmTransform = handJointService.RequestJointTransform(TrackedHandJoint.Palm, Handedness.Left);
        SerializableKeyframe leftPalmKeyX = new SerializableKeyframe(timeOfKeyFrame, leftPalmTransform.localPosition.x);
        SerializableKeyframe leftPalmKeyY = new SerializableKeyframe(timeOfKeyFrame, leftPalmTransform.localPosition.y);
        SerializableKeyframe leftPalmKeyZ = new SerializableKeyframe(timeOfKeyFrame, leftPalmTransform.localPosition.z);
        SerializableKeyframe leftPalmKeyRotationX = new SerializableKeyframe(timeOfKeyFrame, leftPalmTransform.localRotation.x);
        SerializableKeyframe leftPalmKeyRotationY = new SerializableKeyframe(timeOfKeyFrame, leftPalmTransform.localRotation.y);
        SerializableKeyframe leftPalmKeyRotationZ = new SerializableKeyframe(timeOfKeyFrame, leftPalmTransform.localRotation.z);
        SerializableKeyframe leftPalmKeyRotationW = new SerializableKeyframe(timeOfKeyFrame, leftPalmTransform.localRotation.w);
        leftPalmKeyframesX.Add(leftPalmKeyX);
        leftPalmKeyframesY.Add(leftPalmKeyY);
        leftPalmKeyframesZ.Add(leftPalmKeyZ);
        leftPalmKeyframesRotationX.Add(leftPalmKeyRotationX);
        leftPalmKeyframesRotationY.Add(leftPalmKeyRotationY);
        leftPalmKeyframesRotationZ.Add(leftPalmKeyRotationZ);
        leftPalmKeyframesRotationW.Add(leftPalmKeyRotationW);

        Transform rightPalmTransform = handJointService.RequestJointTransform(TrackedHandJoint.Palm, Handedness.Right);
        SerializableKeyframe rightPalmKeyX = new SerializableKeyframe(timeOfKeyFrame, rightPalmTransform.localPosition.x);
        SerializableKeyframe rightPalmKeyY = new SerializableKeyframe(timeOfKeyFrame, rightPalmTransform.localPosition.y);
        SerializableKeyframe rightPalmKeyZ = new SerializableKeyframe(timeOfKeyFrame, rightPalmTransform.localPosition.z);
        SerializableKeyframe rightPalmKeyRotationX = new SerializableKeyframe(timeOfKeyFrame, rightPalmTransform.localRotation.x);
        SerializableKeyframe rightPalmKeyRotationY = new SerializableKeyframe(timeOfKeyFrame, rightPalmTransform.localRotation.y);
        SerializableKeyframe rightPalmKeyRotationZ = new SerializableKeyframe(timeOfKeyFrame, rightPalmTransform.localRotation.z);
        SerializableKeyframe rightPalmKeyRotationW = new SerializableKeyframe(timeOfKeyFrame, rightPalmTransform.localRotation.w);
        rightPalmKeyframesX.Add(rightPalmKeyX);
        rightPalmKeyframesY.Add(rightPalmKeyY);
        rightPalmKeyframesZ.Add(rightPalmKeyZ);
        rightPalmKeyframesRotationX.Add(rightPalmKeyRotationX);
        rightPalmKeyframesRotationY.Add(rightPalmKeyRotationY);
        rightPalmKeyframesRotationZ.Add(rightPalmKeyRotationZ);
        rightPalmKeyframesRotationW.Add(rightPalmKeyRotationW);

        timeOfLastUpdate += Time.deltaTime;
    }


    private void ResetRecorder()
    {
        leftPalmKeyframesX = new List<SerializableKeyframe>();
        timeOfLastUpdate = 0.0f;
    }

}
