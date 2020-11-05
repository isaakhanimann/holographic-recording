using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;

public class RecorderFunctions : MonoBehaviour
{
    public GameObject recordingRepresentationPrefab;
    public TextMeshPro debugLogTmPro;

    private GameObject recordingRepresentationInstance;

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

    private void StartRecording()
    {
        isRecording = true;
    }

    private void CancelRecording()
    {
        isRecording = false;
        ResetRecorder();
    }

    private HoloRecording StopRecording()
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

    private AllKeyFrames allKeyFrames = new AllKeyFrames();

    private GameObject leftHand;
    private GameObject rightHand;


    private void CaptureKeyFrames()
    {
        if (leftHand != null)
        {
            debugLogTmPro.text = "leftHand is not null" + System.Environment.NewLine;
            AddAllJointPoses(Handedness.Left);
        }
        else
        {
            debugLogTmPro.text = "leftHand is null" + System.Environment.NewLine;
            leftHand = GameObject.Find("Left_OurRiggedHandLeft(Clone)");
        }

        if(rightHand != null)
        {
            debugLogTmPro.text += "rightHand is not null" + System.Environment.NewLine;
            AddAllJointPoses(Handedness.Right);
        }
        else
        {
            debugLogTmPro.text += "rightHand is null" + System.Environment.NewLine;
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
