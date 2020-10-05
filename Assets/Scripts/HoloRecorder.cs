using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;

public class HoloRecorder : MonoBehaviour
{

    public static HoloRecorder instance;

    private IMixedRealityInputSystem inputSystem = null;
    private InputRecordingService animationRecorder;
    private AudioClip currentAudioClip;
    private const string MICROPHONE_NAME = "Built-in Microphone";

    public void Initialize()
    {
        instance = FindObjectOfType(typeof(HoloRecorder)) as HoloRecorder;
        if (!MixedRealityServiceRegistry.TryGetService<IMixedRealityInputSystem>(out inputSystem))
        {
            Debug.Log("Failed to acquire the input system. It may not have been registered");
        }
        animationRecorder = new InputRecordingService(inputSystem);
    }

    public void StartRecording()
    {
        currentAudioClip = Microphone.Start(MICROPHONE_NAME, true, 10, 44100);
        animationRecorder.StartRecording();
    }

    public HoloRecording StopRecording()
    {
        Microphone.End(MICROPHONE_NAME);
        animationRecorder.StopRecording();
        string animationClipFilePath = animationRecorder.SaveInputAnimation("recorded-animation");
        Debug.Log($"The file path of the animation is: {animationClipFilePath}");
        return new HoloRecording(currentAudioClip, animationClipFilePath);
    }

    public void CancelRecording()
    {
        animationRecorder.DiscardRecordedInput();
        Microphone.End(MICROPHONE_NAME);
        currentAudioClip = null;
    }
}