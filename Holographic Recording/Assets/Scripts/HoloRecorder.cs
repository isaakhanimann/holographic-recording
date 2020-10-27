using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit;
using TMPro;

public class HoloRecorder
{
    private AudioClip currentAudioClip;
    private const string MICROPHONE_NAME = null;

    public string animationClipFilePath;

    public IMixedRealityInputRecordingService recordingService = null;
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

    public void Initialize()
    {

    }

    public void StartRecording()
    {
        //currentAudioClip = Microphone.Start(MICROPHONE_NAME, true, 10, 44100);
        RecordingService.StartRecording();
    }

    public HoloRecording StopRecording()
    {
        //Microphone.End(MICROPHONE_NAME);
        RecordingService.StopRecording();
        RecordingService.SaveInputAnimation();

        return new HoloRecording(currentAudioClip, animationClipFilePath);
    }

    public void CancelRecording()
    {
        RecordingService.DiscardRecordedInput();
        //Microphone.End(MICROPHONE_NAME);
        //currentAudioClip = null;
    }
}