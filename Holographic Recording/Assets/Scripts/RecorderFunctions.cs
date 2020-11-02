using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit;
using System.Runtime.Serialization.Formatters.Binary;

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
        AllKeyFrames allKeyFrames = new AllKeyFrames(leftPalmPoses, rightPalmPoses);

        BinaryFormatter binaryFormatter = new BinaryFormatter();
        string path = Application.persistentDataPath + $"/{filename}.animationClip";
        FileStream fileStream = File.Create(path);
        binaryFormatter.Serialize(fileStream, allKeyFrames);
        fileStream.Close();

        return path;
    }


    private int GetRandomNumberBetween1and100000()
    {
        System.Random random = new System.Random();
        int randomInt = random.Next(1, 100001);
        return randomInt;
    }

    private int captureFrequencyInFrames = 10;
    private float timeSinceStartOfRecording = 0.0f;


    void LateUpdate()
    {

        if (isRecording)
        {
            timeSinceStartOfRecording += Time.deltaTime;
        }

        if (isRecording && Time.frameCount % captureFrequencyInFrames == 0)
        {
            CaptureKeyFrames();
        }
    }

    private PoseKeyframeLists leftPalmPoses = new PoseKeyframeLists();
    private PoseKeyframeLists rightPalmPoses = new PoseKeyframeLists();
    

    private void CaptureKeyFrames()
    {
        Transform leftPalmTransform = handJointService.RequestJointTransform(TrackedHandJoint.Palm, Handedness.Left);
        AddPose(timeSinceStartOfRecording, leftPalmTransform, leftPalmPoses);

        Transform rightPalmTransform = handJointService.RequestJointTransform(TrackedHandJoint.Palm, Handedness.Right);
        AddPose(timeSinceStartOfRecording, rightPalmTransform, rightPalmPoses);

    }

    private void AddPose(float time, Transform jointTransform, PoseKeyframeLists listToAddTo)
    {
        SerializableKeyframe keyX = new SerializableKeyframe(time, jointTransform.localPosition.x);
        SerializableKeyframe keyY = new SerializableKeyframe(time, jointTransform.localPosition.y);
        SerializableKeyframe keyZ = new SerializableKeyframe(time, jointTransform.localPosition.z);

        SerializableKeyframe keyRotationX = new SerializableKeyframe(time, jointTransform.localRotation.x);
        SerializableKeyframe keyRotationY = new SerializableKeyframe(time, jointTransform.localRotation.y);
        SerializableKeyframe keyRotationZ = new SerializableKeyframe(time, jointTransform.localRotation.z);
        SerializableKeyframe keyRotationW = new SerializableKeyframe(time, jointTransform.localRotation.w);

        listToAddTo.keyframesPositionX.Add(keyX);
        listToAddTo.keyframesPositionY.Add(keyY);
        listToAddTo.keyframesPositionZ.Add(keyZ);

        listToAddTo.keyframesRotationX.Add(keyRotationX);
        listToAddTo.keyframesRotationY.Add(keyRotationY);
        listToAddTo.keyframesRotationZ.Add(keyRotationZ);
        listToAddTo.keyframesRotationW.Add(keyRotationW);
    }


    private void ResetRecorder()
    {
        leftPalmPoses = new PoseKeyframeLists();
        rightPalmPoses = new PoseKeyframeLists();
        timeSinceStartOfRecording = 0.0f;
    }

}
