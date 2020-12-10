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
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Runtime.Serialization;
using Microsoft.Azure.SpatialAnchors;
using Microsoft.Azure.SpatialAnchors.Unity;
using Microsoft.Azure.SpatialAnchors.Unity.Examples;
using UnityEngine.XR.WSA;

public class RecorderFunctions : AnchorManager 
{
    public GameObject recordingRepresentationPrefab;
    public GameObject audioRecorderInstance;
    public GameObject timerInstance;

    public int captureFrequencyInFrames = 1;
    public GameObject preRecordingMenu;
    public GameObject whileRecordingMenu;

    private GameObject recordingRepresentationInstance;
    private AudioRecorder audioRecorder;
    private Nullable<JobHandle> saveJobHandle;
    private bool isRecording;
    private bool isHandDetected;
    private int numberOfRecording;
    private string pathToScreenshot;

    public override async void Start()
    {
        base.Start();
        audioRecorder = audioRecorderInstance.GetComponent<AudioRecorder>();

        await InitAnchorSession();
        FindAnchors();
    }

    public override void Update()
    {
        // check saveJobHandle to know when the saving of the recording is done
        if (saveJobHandle != null)
        {
            Debug.Log("is job completed = {saveJobHandle?.IsCompleted}");
        }

        base.Update();
    }

    public override void OnCloudAnchorLocated(AnchorLocatedEventArgs args)
    {
        CloudSpatialAnchor cloudAnchor = args.Anchor;

        // callback is sometimes called multiple times for the same anchor, so we ensure only one object is instantiated per anchor ID
        if (!instantiatedAnchorIds.Contains(args.Identifier))
        {
            // load recording
            string recordingId = anchorStore.GetRecordingId(args.Identifier);
            HoloRecording loadedRecording = LoadHoloRecording(recordingId);

            // Instantiate anchored object - will be the parent object of hands recording and representation
            GameObject anchoredObject = new GameObject();
            WorldAnchor wa = anchoredObject.AddComponent<WorldAnchor>();
            wa.SetNativeSpatialAnchorPtr(cloudAnchor.LocalAnchor);

            // instantiate representation and pass the recording to it
            Vector3 newRepLocation = new Vector3(loadedRecording.positionXRep, loadedRecording.positionYRep, loadedRecording.positionZRep);
            InstantiateRecordingRepresentation(ref anchoredObject, atLocation: newRepLocation, atPalm: false);

            HoloPlayerBehaviour playerComponent = recordingRepresentationInstance.GetComponent<HoloPlayerBehaviour>();
            playerComponent.PutHoloRecordingIntoPlayer(recordingId, loadedRecording, anchoredObject, anchorStore, openKeyboard: false);

            // Mark already instantiated Anchor Ids so that nothing is instantiated more than once.
            instantiatedAnchorIds.Add(args.Identifier);
        }
    }

    public HoloRecording LoadHoloRecording(string recordingId) {

        string filePath = Application.persistentDataPath + "/" + "holorecording_" + recordingId + ".bin";
        FileStream fs = File.Open(filePath, FileMode.Open);
        BinaryFormatter bf = new BinaryFormatter();
        HoloRecording holoRecording = new HoloRecording();
    	try
        {
            holoRecording = (HoloRecording) bf.Deserialize(fs);
        }
        catch (SerializationException e)
        {
            Debug.Log(" Failed to deserialize. Reason: " + e.Message);
            throw;
        }
        finally
        {
            fs.Close();
            Debug.Log("close filestream successfully \n");

        }
        return holoRecording;
    }

    public void StartRecording()
    {
        // update the UI
        preRecordingMenu.SetActive(false);
        StartCoroutine(SetWhileRecordingMenuActiveAfterNSeconds()); // delay the activation of next menu so buttons are not pressed accidentally

        // start recording audio and keyframes
        numberOfRecording = GetRandomNumberBetween1and100000();
        audioRecorder.StartRecording();

        allKeyFrames = new AllKeyFrames();
        allKeyFrames.leftJointLists = new KeyFrameListsForAllHandJoints(Handedness.Left);
        allKeyFrames.rightJointLists = new KeyFrameListsForAllHandJoints(Handedness.Right);
        isRecording = true;

        // make a screenshot for the recording representation
        StartCoroutine(MakeScreenshotAfterNSeconds());
    }

    IEnumerator MakeScreenshotAfterNSeconds()
    {
        Debug.Log($"MakeScreenshotAfterNSeconds called");
        yield return new WaitForSeconds(2);
        string fileName = $"Screenshot{numberOfRecording}";
        pathToScreenshot = Application.persistentDataPath + $"/{fileName}.png";
        
        // hide timer so it doesn't appear in screenshot
        timerInstance.gameObject.GetComponentInChildren<MeshRenderer>().enabled = false;
        
        ScreenCapture.CaptureScreenshot(pathToScreenshot);
        
        // add short delay for the screenshot to be captured before rendering timer again
        yield return new WaitForSeconds((float)0.10);
        timerInstance.gameObject.GetComponentInChildren<MeshRenderer>().enabled = true;

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

    public void StopRecordingAndPutRecordingIntoRepresentation()
    {
        // update UI
        whileRecordingMenu.SetActive(false);
        preRecordingMenu.SetActive(true);

        // Instantiate anchored object - will be the parent object of hands recording and representation
        GameObject anchoredObject = InstantiateAnchoredObject();
        // instantiate representation and pass the recording to it
        InstantiateRecordingRepresentation(ref anchoredObject, atLocation: Vector3.zero, atPalm: true);

        // get final length of recording in seconds
        int recordingLength = timerInstance.GetComponent<TimerBehaviour>().GetCurrentRecordingTime();

        // stop recording and get the recording object
        HoloRecording newRecording = StopRecording(recordingLength, recordingRepresentationInstance.transform.localPosition);

        audioRecorder.StopAndSaveRecording(numberOfRecording.ToString());

        HoloPlayerBehaviour playerComponent = recordingRepresentationInstance.GetComponent<HoloPlayerBehaviour>();
        playerComponent.PutHoloRecordingIntoPlayer(numberOfRecording.ToString(), newRecording, anchoredObject, anchorStore);

        // Save anchor to cloud
        SaveObjectAnchorToCloud(anchoredObject, numberOfRecording.ToString());
    }

    private GameObject InstantiateAnchoredObject()
    {
        GameObject anchoredObject = new GameObject();
        AddCloudNativeAnchorToObject(ref anchoredObject);
        return anchoredObject;
    }

    private void InstantiateRecordingRepresentation(ref GameObject anchoredObject, Vector3 atLocation, bool atPalm = true)
    {
        // get palm position
        Vector3 positionToInstantiate = new Vector3(0, 0, 0);
        Quaternion rotationToInstantiate = Quaternion.identity;

        if (atPalm)
        {
            if (HandJointUtils.TryGetJointPose(TrackedHandJoint.Palm, Handedness.Left, out MixedRealityPose pose))
            {
                positionToInstantiate = pose.Position;
            }
            else
            {
                positionToInstantiate = Camera.main.transform.position + 0.5f * Vector3.forward;
            }
        } 
        // instantiate representation at palm or specified location
        recordingRepresentationInstance = Instantiate(original: recordingRepresentationPrefab, position: positionToInstantiate, rotation: rotationToInstantiate, parent: anchoredObject.transform);
        if (!atPalm) {
            recordingRepresentationInstance.transform.localPosition = atLocation;
        }
    }


    public void CancelRecordingAndRemoveRepresentation()
    {
        CancelRecording();

        // update UI
        whileRecordingMenu.SetActive(false);
        preRecordingMenu.SetActive(true);
        Destroy(recordingRepresentationInstance);
    }

    private void CancelRecording()
    {
        isRecording = false;
        Microphone.End(null);
        ResetRecorder();
    }

    private HoloRecording StopRecording(int recordingLength, Vector3 posRep)
    {
        isRecording = false;
        Microphone.End(null);
        HoloRecording newRecording = SaveRecording(recordingLength, posRep);
        ResetRecorder();
        return newRecording;
    }

    private HoloRecording SaveRecording(int recordingLength, Vector3 posRep)
    {
        Debug.Log($"SaveRecording called");
        string animationClipName = "AnimationClip" + numberOfRecording;
        string pathToAnimationClip = Application.persistentDataPath + $"/{animationClipName}.animationClip";
        // saving is uncommented to unclutter the hololens
        //SaveKeyframesAsynchronously(pathToAnimationClip);
        HoloRecording newRecording = new HoloRecording(pathToAnimationClip, animationClipName, allKeyFrames, pathToScreenshot, recordingLength, "", posRep);
        return newRecording;
    }


    private void SaveKeyframesAsynchronously(string path)
    {
        Debug.Log($"SaveKeyframesAsynchronously");
        // create a job to be executed asynchronously
        SaveKeyframesJob saveJob = new SaveKeyframesJob();

        // pass arguments to that job
        NativeArray<AllKeyFrames> allKeyframesContainer = new NativeArray<AllKeyFrames>(1, Allocator.Persistent);
        allKeyframesContainer[0] = allKeyFrames;
        saveJob.allKeyFramesContainer = allKeyframesContainer;
        NativeArray<NonNullString> pathContainer = new NativeArray<NonNullString>(1, Allocator.Persistent);
        NonNullString nonNullPath = new NonNullString(path);
        pathContainer[0] = nonNullPath;
        saveJob.pathContainer = pathContainer;

        // start the job
        saveJobHandle = saveJob.Schedule();
        Debug.Log($"Schedule called");
    }

    // needed because a normal string can be null and something nullable can't be passed as an argument to a job
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

    // this number defines the filenames of all the files associated with a recording
    private int GetRandomNumberBetween1and100000()
    {
        System.Random random = new System.Random();
        int randomInt = random.Next(1, 100001);
        return randomInt;
    }

    // to keep track of the time for each keyframe
    private float timeSinceStartOfRecording = 0.0f;

    void LateUpdate()
    {
        // only capture keyframes if we are recording
        if (isRecording)
        {
            timeSinceStartOfRecording += Time.deltaTime;
        }

        if (isRecording && Time.frameCount % captureFrequencyInFrames == 0)
        {
            CaptureKeyFrames();
        }
    }

    // while recording this object is updated, it stores all the keyframes that are serialized/deserialized
    private AllKeyFrames allKeyFrames = new AllKeyFrames();

    private GameObject leftHand;
    private GameObject rightHand;

    private void CaptureKeyFrames()
    {
        // only if the hand is visible we can record keyframes, else we have to find it.
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

    #region Hand Menu Activation

    public void OnHandDetected()
    {

        isHandDetected = true;

        if (isRecording)
        {
            preRecordingMenu.SetActive(false);
            whileRecordingMenu.SetActive(true);
        }
        else
        {
            preRecordingMenu.SetActive(true);
            whileRecordingMenu.SetActive(false);
        }
    }

    public void OnHandLost()
    {
        isHandDetected = false;
        preRecordingMenu.SetActive(false);
        whileRecordingMenu.SetActive(false);
    }

    #endregion

}
