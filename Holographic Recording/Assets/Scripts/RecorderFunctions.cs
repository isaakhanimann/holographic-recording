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
        SerializableSnapshots serializable = new SerializableSnapshots(snapshots);
        string keyframesAsJson = JsonUtility.ToJson(serializable, true);
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
            CreateSnapshot();
        }
    }

    private float timeOfLastUpdate = 0.0f;

    private List<Snapshot> snapshots = new List<Snapshot>();

    private void CreateSnapshot()
    {
        float timeOfKeyFrame = timeOfLastUpdate + Time.deltaTime;

        Transform leftPalmTransform = handJointService.RequestJointTransform(TrackedHandJoint.Palm, Handedness.Left);
        Vector3 pl = leftPalmTransform.localPosition;
        Quaternion rl = leftPalmTransform.localRotation;
        JointPose leftPalmPose = new JointPose(pl.x, pl.y, pl.z, rl.x, rl.y, rl.z, rl.w);

        Transform rightPalmTransform = handJointService.RequestJointTransform(TrackedHandJoint.Palm, Handedness.Right);
        Vector3 pr = rightPalmTransform.localPosition;
        Quaternion rr = rightPalmTransform.localRotation;
        JointPose rightPalmPose = new JointPose(pr.x, pr.y, pr.z, rr.x, rr.y, rr.z, rr.w);

        Snapshot snapshot = new Snapshot(timeOfKeyFrame, leftPalmPose, rightPalmPose);

        snapshots.Add(snapshot);

        timeOfLastUpdate += Time.deltaTime;
    }


    private void ResetRecorder()
    {
        snapshots = new List<Snapshot>();
        timeOfLastUpdate = 0.0f;
    }

}
