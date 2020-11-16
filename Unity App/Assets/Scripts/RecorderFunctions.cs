using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System;
using Unity.Jobs;
using Unity.Collections;
using System.Collections;

public class RecorderFunctions : MonoBehaviour
{
    public GameObject recordingRepresentationPrefab;
    public int captureFrequencyInFrames = 1;
    public GameObject preRecordingMenu;
    public GameObject whileRecordingMenu;


    private GameObject recordingRepresentationInstance;
    private Nullable<JobHandle> saveJobHandle;
    private bool isRecording;
    private bool isHandDetected;
    private int numberOfRecording;
    private string pathToScreenshot;
    private AudioClip audioClip;


    void Update()
    {
        if (saveJobHandle != null)
        {
            //debugLogTmPro.text = $"is job completed = {saveJobHandle?.IsCompleted}";
        }
    }


    public void StartRecording()
    {
        preRecordingMenu.SetActive(false);
        audioClip = Microphone.Start(null, false, 60, 44100);
        numberOfRecording = GetRandomNumberBetween1and100000();
        StartCoroutine(SetWhileRecordingMenuActiveAfterNSeconds());
        allKeyFrames = new AllKeyFrames();
        allKeyFrames.leftJointLists = new KeyFrameListsForAllHandJoints(Handedness.Left);
        allKeyFrames.rightJointLists = new KeyFrameListsForAllHandJoints(Handedness.Right);
        isRecording = true;
        StartCoroutine(MakeScreenshotAfterNSeconds());
    }

    IEnumerator MakeScreenshotAfterNSeconds()
    {
        Debug.Log($"MakeScreenshotAfterNSeconds called");
        yield return new WaitForSeconds(2);
        string fileName = $"Screenshot{numberOfRecording}";
        pathToScreenshot = Application.persistentDataPath + $"/{fileName}.png";
        ScreenCapture.CaptureScreenshot(pathToScreenshot);
        Debug.Log($"CaptureScreenshot executed, pathToScreenshot = {pathToScreenshot}");
    }

    IEnumerator SetWhileRecordingMenuActiveAfterNSeconds()
    {
        yield return new WaitForSeconds(2);
        if (isHandDetected)
        {
            whileRecordingMenu.SetActive(true);
        }
    }

    public void OnHandDetected()
    {
        Debug.Log("OnHandDetected is called");

        isHandDetected = true;

        Debug.Log($"isRecording = {isRecording}");
        

        if (isRecording)
        {
            preRecordingMenu.SetActive(false);
            whileRecordingMenu.SetActive(true);
        } else
        {
            preRecordingMenu.SetActive(true);
            whileRecordingMenu.SetActive(false);
        }
        Debug.Log($"preRecordingMenu active = {preRecordingMenu.active}");
        Debug.Log($"whileRecordingMenu active = {whileRecordingMenu.active}");
    }

    public void OnHandLost()
    {
        Debug.Log($"OnHandLost is called");
        isHandDetected = false;
        preRecordingMenu.SetActive(false);
        whileRecordingMenu.SetActive(false);
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
        whileRecordingMenu.SetActive(false);
        preRecordingMenu.SetActive(true);

        HoloRecording newRecording = StopRecording();

        InstantiateRecordingRepresentationAtPalm();

        HoloPlayerBehaviour playerComponent = recordingRepresentationInstance.GetComponent<HoloPlayerBehaviour>();
        playerComponent.PutHoloRecordingIntoPlayer(newRecording);
    }

    public void CancelRecordingAndRemoveRepresentation()
    {
        whileRecordingMenu.SetActive(false);
        preRecordingMenu.SetActive(true);

        CancelRecording();
        Destroy(recordingRepresentationInstance);
    }

    private void CancelRecording()
    {
        isRecording = false;
        Microphone.End(null);
        ResetRecorder();
    }

    private HoloRecording StopRecording()
    {
        isRecording = false;
        Microphone.End(null);
        HoloRecording newRecording = SaveRecording();
        ResetRecorder();
        return newRecording;
    }

    private HoloRecording SaveRecording()
    {
        Debug.Log($"SaveRecording called");
        string animationClipName = "AnimationClip" + numberOfRecording;
        string pathToAnimationClip = Application.persistentDataPath + $"/{animationClipName}.animationClip";
        //SaveKeyframesAsynchronously(pathToAnimationClip);
        HoloRecording newRecording = new HoloRecording(pathToAnimationClip, animationClipName, allKeyFrames, pathToScreenshot, audioClip);
        return newRecording;
    }


    private void SaveKeyframesAsynchronously(string path)
    {
        Debug.Log($"SaveKeyframesAsynchronously");
        SaveKeyframesJob saveJob = new SaveKeyframesJob();
        NativeArray<AllKeyFrames> allKeyframesContainer = new NativeArray<AllKeyFrames>(1, Allocator.Persistent);
        allKeyframesContainer[0] = allKeyFrames;
        saveJob.allKeyFramesContainer = allKeyframesContainer;
        NativeArray<NonNullString> pathContainer = new NativeArray<NonNullString>(1, Allocator.Persistent);
        NonNullString nonNullPath = new NonNullString(path);
        pathContainer[0] = nonNullPath;
        saveJob.pathContainer = pathContainer;
        saveJobHandle = saveJob.Schedule();
        Debug.Log($"Schedule called");
    }

    public struct NonNullString
    {
        public NonNullString(string value)
            : this()
        {
            Value = value ?? "N/A";
        }

        public string Value
        {
            get;
            private set;
        }

        public static implicit operator NonNullString(string value)
        {
            return new NonNullString(value);
        }

        public static implicit operator string(NonNullString value)
        {
            return value.Value;
        }
    }


    public struct SaveKeyframesJob : IJob
    {
        public NativeArray<AllKeyFrames> allKeyFramesContainer;
        public NativeArray<NonNullString> pathContainer;

        public void Execute()
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream fileStream = File.Create(pathContainer[0].Value);
            binaryFormatter.Serialize(fileStream, allKeyFramesContainer[0]);
            fileStream.Close();
        }
    }


    private int GetRandomNumberBetween1and100000()
    {
        System.Random random = new System.Random();
        int randomInt = random.Next(1, 100001);
        return randomInt;
    }

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

    private AllKeyFrames allKeyFrames = new AllKeyFrames();

    private GameObject leftHand;
    private GameObject rightHand;


    private void CaptureKeyFrames()
    {
        if (leftHand != null)
        {
            AddAllJointPoses(Handedness.Left);
        }
        else
        {
            leftHand = GameObject.Find("Left_OurRiggedHandLeft(Clone)");
        }

        if (rightHand != null)
        {
            AddAllJointPoses(Handedness.Right);
        }
        else
        {
            rightHand = GameObject.Find("Right_OurRiggedHandRight(Clone)");
        }
    }

    private void AddAllJointPoses(Handedness handedness)
    {
        Transform rootTransform;
        KeyFrameListsForAllHandJoints jointLists;

        if (handedness == Handedness.Left)
        {
            rootTransform = leftHand.transform;
            jointLists = allKeyFrames.leftJointLists;
        }
        else
        {
            rootTransform = rightHand.transform;
            jointLists = allKeyFrames.rightJointLists;
        }

        Transform handTransform = rootTransform.Find(jointLists.hand.path);
        Transform mainTransform = rootTransform.Find(jointLists.main.path);
        Transform wristTransform = rootTransform.Find(jointLists.wrist.path);
        Transform middleTransform = rootTransform.Find(jointLists.middle.path);
        Transform middle1Transform = rootTransform.Find(jointLists.middle1.path);
        Transform middle2Transform = rootTransform.Find(jointLists.middle2.path);
        Transform middle3Transform = rootTransform.Find(jointLists.middle3.path);
        Transform middle3EndTransform = rootTransform.Find(jointLists.middle3End.path);
        Transform pinkyTransform = rootTransform.Find(jointLists.pinky.path);
        Transform pinky1Transform = rootTransform.Find(jointLists.pinky1.path);
        Transform pinky2Transform = rootTransform.Find(jointLists.pinky2.path);
        Transform pinky3Transform = rootTransform.Find(jointLists.pinky3.path);
        Transform pinky3EndTransform = rootTransform.Find(jointLists.pinky3End.path);
        Transform pointTransform = rootTransform.Find(jointLists.point.path);
        Transform point1Transform = rootTransform.Find(jointLists.point1.path);
        Transform point2Transform = rootTransform.Find(jointLists.point2.path);
        Transform point3Transform = rootTransform.Find(jointLists.point3.path);
        Transform point3EndTransform = rootTransform.Find(jointLists.point3End.path);
        Transform ringTransform = rootTransform.Find(jointLists.ring.path);
        Transform ring1Transform = rootTransform.Find(jointLists.ring1.path);
        Transform ring2Transform = rootTransform.Find(jointLists.ring2.path);
        Transform ring3Transform = rootTransform.Find(jointLists.ring3.path);
        Transform ring3EndTransform = rootTransform.Find(jointLists.ring3End.path);
        Transform thumb1Transform = rootTransform.Find(jointLists.thumb1.path);
        Transform thumb2Transform = rootTransform.Find(jointLists.thumb2.path);
        Transform thumb3Transform = rootTransform.Find(jointLists.thumb3.path);
        Transform thumb3EndTransform = rootTransform.Find(jointLists.thumb3End.path);

        AddPose(timeSinceStartOfRecording, rootTransform, jointLists.root);
        AddPose(timeSinceStartOfRecording, handTransform, jointLists.hand);
        AddPose(timeSinceStartOfRecording, mainTransform, jointLists.main);
        AddPose(timeSinceStartOfRecording, wristTransform, jointLists.wrist);
        AddPose(timeSinceStartOfRecording, middleTransform, jointLists.middle);
        AddPose(timeSinceStartOfRecording, middle1Transform, jointLists.middle1);
        AddPose(timeSinceStartOfRecording, middle2Transform, jointLists.middle2);
        AddPose(timeSinceStartOfRecording, middle3Transform, jointLists.middle3);
        AddPose(timeSinceStartOfRecording, middle3EndTransform, jointLists.middle3End);
        AddPose(timeSinceStartOfRecording, pinkyTransform, jointLists.pinky);
        AddPose(timeSinceStartOfRecording, pinky1Transform, jointLists.pinky1);
        AddPose(timeSinceStartOfRecording, pinky2Transform, jointLists.pinky2);
        AddPose(timeSinceStartOfRecording, pinky3Transform, jointLists.pinky3);
        AddPose(timeSinceStartOfRecording, pinky3EndTransform, jointLists.pinky3End);
        AddPose(timeSinceStartOfRecording, pointTransform, jointLists.point);
        AddPose(timeSinceStartOfRecording, point1Transform, jointLists.point1);
        AddPose(timeSinceStartOfRecording, point2Transform, jointLists.point2);
        AddPose(timeSinceStartOfRecording, point3Transform, jointLists.point3);
        AddPose(timeSinceStartOfRecording, point3EndTransform, jointLists.point3End);
        AddPose(timeSinceStartOfRecording, ringTransform, jointLists.ring);
        AddPose(timeSinceStartOfRecording, ring1Transform, jointLists.ring1);
        AddPose(timeSinceStartOfRecording, ring2Transform, jointLists.ring2);
        AddPose(timeSinceStartOfRecording, ring3Transform, jointLists.ring3);
        AddPose(timeSinceStartOfRecording, ring3EndTransform, jointLists.ring3End);
        AddPose(timeSinceStartOfRecording, thumb1Transform, jointLists.thumb1);
        AddPose(timeSinceStartOfRecording, thumb2Transform, jointLists.thumb2);
        AddPose(timeSinceStartOfRecording, thumb3Transform, jointLists.thumb3);
        AddPose(timeSinceStartOfRecording, thumb3EndTransform, jointLists.thumb3End);
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
        allKeyFrames = new AllKeyFrames();
        timeSinceStartOfRecording = 0.0f;
    }

}
