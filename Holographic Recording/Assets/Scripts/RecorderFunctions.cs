using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit;

public class RecorderFunctions : MonoBehaviour
{
    public GameObject recordingRepresentationPrefab;

    private GameObject recordingRepresentationInstance;

    private IMixedRealityInputRecordingService recordingService = null;
    public IMixedRealityInputRecordingService RecordingService
    {
        get
        {
            if (recordingService == null)
            {
                recordingService = CoreServices.GetInputSystemDataProvider<IMixedRealityInputRecordingService>();
            }

            return recordingService;
        }
    }


    public void StartRecordingAndInstantiateRepresentation()
    {
        RecordingService.StartRecording();
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
        RecordingService.StopRecording();
        string inputAnimationFilePath = RecordingService.SaveInputAnimation();
        HoloRecording newRecording = new HoloRecording(inputAnimationFilePath);

        HoloPlayerBehaviour playerComponent = recordingRepresentationInstance.GetComponent<HoloPlayerBehaviour>();
        playerComponent.PutHoloRecordingIntoPlayer(newRecording);
    }

    public void CancelRecordingAndRemoveRepresentation()
    {
        RecordingService.DiscardRecordedInput();
        Destroy(recordingRepresentationInstance);
    }

}
