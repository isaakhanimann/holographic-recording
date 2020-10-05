using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRecorderFunctions : MonoBehaviour
{
    public GameObject recorderPrefab;


    public void StartRecording()
    {
        HoloRecorder.instance.StartRecording();
    }

    public void StopRecording()
    {
        HoloRecording newRecording = HoloRecorder.instance.StopRecording();

        // todo instantiate new prefab and assign the holorecording to it.
        GameObject recorderObject = Instantiate(recorderPrefab);
        HoloPlayer playerComponent = recorderObject.GetComponent<HoloPlayer>();
        playerComponent.PutHoloRecordingIntoPlayer(newRecording);
    }

    public void CancelRecording()
    {
        HoloRecorder.instance.CancelRecording();

    }
}
