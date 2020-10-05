using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit;

public class HoloPlayer : MonoBehaviour
{

    private AudioSource audioSource;
    private IMixedRealityInputSystem inputSystem;
    private InputPlaybackService animationPlayer;
    void Start()
    {
        InitializeHoloPlayer();
    }

    private void InitializeHoloPlayer()
    {
        audioSource = GetComponent<AudioSource>();
        if (!MixedRealityServiceRegistry.TryGetService<IMixedRealityInputSystem>(out inputSystem))
        {
            Debug.Log("Failed to acquire the input system inside Holoplayer. It may not have been registered");
        }
        animationPlayer = new InputPlaybackService(inputSystem);
    }

    public void PutHoloRecordingIntoPlayer(HoloRecording recording)
    {
        audioSource.clip = recording.audioClip;
        animationPlayer.LoadInputAnimation(recording.pathToAnimationFile);
    }

    public void Play()
    {
        audioSource.Play();
        animationPlayer.Play();
    }

    public void Pause()
    {
        audioSource.Pause();
        animationPlayer.Pause();
    }

    public void Seek(float timeToJumpToInSeconds)
    {
        audioSource.time = timeToJumpToInSeconds;
        animationPlayer.LocalTime = timeToJumpToInSeconds;
    }

    public void Stop()
    {
        audioSource.Stop();
        animationPlayer.Stop();
    }
}
