using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit;

public static class HoloRecorder
{
    private static IMixedRealityInputSystem inputSystem;
    private static InputRecordingService animationRecorder;
    private static AudioClip currentAudioClip;
    private const string MICROPHONE_NAME = null;


    public static void Initialize()
    {
        if (!MixedRealityServiceRegistry.TryGetService<IMixedRealityInputSystem>(out inputSystem))
        {
            Debug.Log("Failed to acquire the input system inside HoloRecorder. It may not have been registered");
        }
        animationRecorder = new InputRecordingService(inputSystem);
    }

    public static void StartRecording()
    {
        currentAudioClip = Microphone.Start(MICROPHONE_NAME, true, 10, 44100);
        animationRecorder.StartRecording();
    }

    public static HoloRecording StopRecording()
    {
        Microphone.End(MICROPHONE_NAME);
        animationRecorder.StopRecording();
        string animationClipFilePath = animationRecorder.SaveInputAnimation("recorded-animation");
        Debug.Log($"The file path of the animation is: {animationClipFilePath}");
        return new HoloRecording(currentAudioClip, animationClipFilePath);
    }

    public static void CancelRecording()
    {
        animationRecorder.DiscardRecordedInput();
        Microphone.End(MICROPHONE_NAME);
        currentAudioClip = null;
    }
}