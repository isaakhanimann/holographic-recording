using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit;
using UnityEngine.UI;
using TMPro;

public class HoloPlayerBehaviour : MonoBehaviour
{

    private AudioSource audioSource;
    private IMixedRealityInputSystem inputSystem;
    private InputPlaybackService animationPlayer;

    public GameObject RightHand;
    public GameObject LeftHand;

    public GameObject UIManager;

    UIRecorderFunctions uirecorderFunctions;

    void Start()
    {
        InitializeHoloPlayer();

        uirecorderFunctions = UIManager.GetComponent<UIRecorderFunctions>();
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
        //audioSource.clip = recording.audioClip;
        if (animationPlayer.LoadInputAnimation(recording.pathToAnimationFile))
        {
            uirecorderFunctions.DebugPanel.GetComponent<TextMeshPro>().text = "PutHoloRecordingIntoPlayer works!!";
            Debug.Log("Loading Input Animation works!!");
        }

        else
        {
            uirecorderFunctions.DebugPanel.GetComponent<TextMeshPro>().text = "PutHoloRecordingIntoPlayer doens't work!!";
            Debug.Log("Loading Input Animation doens't work!!");
        }
    }

    public void Play()
    {
        //audioSource.Play();
        animationPlayer.Play();
        uirecorderFunctions.DebugPanel.GetComponent<TextMeshPro>().text = animationPlayer.IsPlaying + "";

        GameObject RH = Instantiate(RightHand, transform.position + new Vector3(0.125f, -0.25f, 0.5f), Quaternion.identity);
        GameObject LH = Instantiate(LeftHand, transform.position + new Vector3(-0.125f, -0.25f, 0.5f), Quaternion.identity);

        DestroyHand(RH, LH, 5);

        uirecorderFunctions.DebugPanel.GetComponent<TextMeshPro>().text = "Play";
        Debug.Log("Play" + animationPlayer);
    }

    public void Pause()
    {
        //audioSource.Pause();
        animationPlayer.Pause();
    }

    public void Seek(float timeToJumpToInSeconds)
    {
        //audioSource.time = timeToJumpToInSeconds;
        animationPlayer.LocalTime = timeToJumpToInSeconds;
    }

    public void Stop()
    {
        //audioSource.Stop();
        animationPlayer.Stop();
    }

    public void DestroyHand(GameObject RH, GameObject LH, float time)
    {
        Destroy(RH, time);
        Destroy(LH, time);
    }
}
