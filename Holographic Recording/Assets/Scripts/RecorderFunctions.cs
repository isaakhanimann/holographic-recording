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
        PoseKeyframeLists palmPoses = new PoseKeyframeLists(palmKeyframesX, palmKeyframesY, palmKeyframesZ);
        AllKeyFrames allKeyFrames = new AllKeyFrames(palmPoses);
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
    private List<SerializableKeyframe> palmKeyframesX = new List<SerializableKeyframe>();
    private List<SerializableKeyframe> palmKeyframesY = new List<SerializableKeyframe>();
    private List<SerializableKeyframe> palmKeyframesZ = new List<SerializableKeyframe>();


    private void CaptureKeyFrames()
    {
        float timeOfKeyFrame = timeOfLastUpdate + Time.deltaTime;
        Transform leftPalmTransform = handJointService.RequestJointTransform(TrackedHandJoint.Palm, Handedness.Left);
        SerializableKeyframe palmKeyX = new SerializableKeyframe(timeOfKeyFrame, leftPalmTransform.localPosition.x);
        SerializableKeyframe palmKeyY = new SerializableKeyframe(timeOfKeyFrame, leftPalmTransform.localPosition.y);
        SerializableKeyframe palmKeyZ = new SerializableKeyframe(timeOfKeyFrame, leftPalmTransform.localPosition.z);
        palmKeyframesX.Add(palmKeyX);
        palmKeyframesY.Add(palmKeyY);
        palmKeyframesZ.Add(palmKeyZ);
        timeOfLastUpdate += Time.deltaTime;
    }


    private void ResetRecorder()
    {
        palmKeyframesX = new List<SerializableKeyframe>();
        timeOfLastUpdate = 0.0f;
    }

}
