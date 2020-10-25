using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit;
using TMPro;

public class HoloRecorder
{
    private IMixedRealityInputSystem inputSystem;
    public InputRecordingService animationRecorder;
    private AudioClip currentAudioClip;
    private const string MICROPHONE_NAME = null;

    public string animationClipFilePath;

    public void Initialize()
    {
        if (!MixedRealityServiceRegistry.TryGetService<IMixedRealityInputSystem>(out inputSystem))
        {
            Debug.Log("Failed to acquire the input system inside HoloRecorder. It may not have been registered");
        }
        animationRecorder = new InputRecordingService(inputSystem);
        animationRecorder.Enable();
        animationRecorder.UseBufferTimeLimit = false;
    }

    public void StartRecording()
    {
        //currentAudioClip = Microphone.Start(MICROPHONE_NAME, true, 10, 44100);
        animationRecorder.StartRecording();
        Debug.Log("StartRecording");
    }

    public HoloRecording StopRecording()
    {
        //Microphone.End(MICROPHONE_NAME);
        animationRecorder.StopRecording();
        animationClipFilePath = animationRecorder.SaveInputAnimation();

        Debug.Log($"The file path of the animation is: {animationClipFilePath}");
        return new HoloRecording(currentAudioClip, animationClipFilePath);
    }

    public void CancelRecording()
    {
        animationRecorder.DiscardRecordedInput();
        //Microphone.End(MICROPHONE_NAME);
        //currentAudioClip = null;
    }
}