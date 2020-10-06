using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;

public class UIRecorderFunctions : MonoBehaviour
{
    public GameObject recordingRepresentationPrefab;

    private GameObject recordingRepresentationInstance;

    public void StartRecordingAndInstantiateRepresentation()
    {
        HoloRecorder.instance.StartRecording();
        InstantiateRecordingRepresentationAtPalm();
    }

    private void InstantiateRecordingRepresentationAtPalm()
    {
        Vector3 positionToInstantiate;
        Quaternion rotationToInstantiate;
        if (HandJointUtils.TryGetJointPose(TrackedHandJoint.Palm, Handedness.Left, out MixedRealityPose pose))
        {
            positionToInstantiate = pose.Position;
            rotationToInstantiate = pose.Rotation;
        }
        else
        {
            positionToInstantiate = Camera.main.transform.position + 0.5f * Vector3.forward;
            rotationToInstantiate = Camera.main.transform.rotation;
        }
        recordingRepresentationInstance = Instantiate(original: recordingRepresentationPrefab, position: positionToInstantiate, rotation: rotationToInstantiate);
    }

    public void StopRecordingAndPutRecordingIntoRepresentation()
    {
        HoloRecording newRecording = HoloRecorder.instance.StopRecording();

        HoloPlayer playerComponent = recordingRepresentationInstance.GetComponent<HoloPlayer>();
        playerComponent.PutHoloRecordingIntoPlayer(newRecording);
    }

    public void CancelRecordingAndRemoveRepresentation()
    {
        HoloRecorder.instance.CancelRecording();
        Destroy(recordingRepresentationInstance);

    }
}
