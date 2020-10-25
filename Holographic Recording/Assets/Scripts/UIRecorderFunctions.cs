using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using TMPro;

public class UIRecorderFunctions : MonoBehaviour
{
    public GameObject recordingRepresentationPrefab;

    private GameObject recordingRepresentationInstance;

    public GameObject DebugPanel;

    private HoloRecorder holoRecorder;

    private void Start()
    {
        holoRecorder = new HoloRecorder();
        holoRecorder.Initialize();
    }

    public void StartRecordingAndInstantiateRepresentation()
    {
        DebugPanel.GetComponent<TextMeshPro>().text = "StartRecordingAndInstantiateRepresentation";
        DebugPanel.GetComponent<TextMeshPro>().text = holoRecorder.animationRecorder.StartTime + "";
        holoRecorder.StartRecording();
        InstantiateRecordingRepresentationAtPalm();
    }

    private void InstantiateRecordingRepresentationAtPalm()
    {
        //DebugPanel.GetComponent<TextMeshPro>().text = "InstantiateRecordingRepresentationAtPalm";
        Debug.Log("InstantiateRecordingRepresentationAtPalm");
        Vector3 positionToInstantiate;
        Quaternion rotationToInstantiate = Quaternion.identity;
        if (HandJointUtils.TryGetJointPose(TrackedHandJoint.Palm, Handedness.Left, out MixedRealityPose pose))
        {
            positionToInstantiate = pose.Position;
            //rotationToInstantiate = pose.Rotation;
        }
        else
        {
            positionToInstantiate = Camera.main.transform.position + 0.5f * Vector3.forward;
            //rotationToInstantiate = Camera.main.transform.rotation;
        }
        recordingRepresentationInstance = Instantiate(original: recordingRepresentationPrefab, position: positionToInstantiate, rotation: rotationToInstantiate);
    }

    public void StopRecordingAndPutRecordingIntoRepresentation()
    {
        DebugPanel.GetComponent<TextMeshPro>().text = "StopRecordingAndPutRecordingIntoRepresentation";
        HoloRecording newRecording = holoRecorder.StopRecording();

        DebugPanel.GetComponent<TextMeshPro>().text = holoRecorder.animationClipFilePath;
        DebugPanel.GetComponent<TextMeshPro>().text = holoRecorder.animationRecorder.StartTime + "";

        HoloPlayerBehaviour playerComponent = recordingRepresentationInstance.GetComponent<HoloPlayerBehaviour>();
        playerComponent.PutHoloRecordingIntoPlayer(newRecording);
    }

    public void CancelRecordingAndRemoveRepresentation()
    {
        DebugPanel.GetComponent<TextMeshPro>().text = "CancelRecordingAndRemoveRepresentation";
        holoRecorder.CancelRecording();
        Destroy(recordingRepresentationInstance);

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S)) Debug.Log(holoRecorder.animationRecorder.SaveInputAnimation());
    }
}
