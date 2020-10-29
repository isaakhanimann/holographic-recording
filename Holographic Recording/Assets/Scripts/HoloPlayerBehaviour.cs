using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit;
using System.Collections;



public class HoloPlayerBehaviour : MonoBehaviour
{ 

    public GameObject hands;

    private GameObject instantiatedHands;
    private Animator instantiatedHandsAnimator;
    private float lengthOfAnimationInSeconds;
    private IMixedRealityInputSystem inputSystem;
    private InputPlaybackService animationPlayer;

    void Start()
    {
        //InitializeHoloPlayer();

    }

    private void InitializeHoloPlayer()
    {
        /*if (!MixedRealityServiceRegistry.TryGetService<IMixedRealityInputSystem>(out inputSystem))
        {
            Debug.Log("Failed to acquire the input system inside Holoplayer. It may not have been registered");
        }
        animationPlayer = new InputPlaybackService(inputSystem);*/
    }

    public void PutHoloRecordingIntoPlayer(HoloRecording recording)
    {
        /*Debug.Log("PutHoloRecordingIntoPlayer");
        InstantiateHandsAndSetInactive();
        if (!animationPlayer.LoadInputAnimation(recording.pathToInputAnimation))
        {
            throw new System.Exception("Input Animation could not be loaded");
        }

        InputAnimation inputAnimation = animationPlayer.Animation;
        AnimationClip animationClip = null;

        // There needs to be an AnimatorOverrideController for every animation clip to be played on the object with the Animator
        AnimatorOverrideController animatorOverrideController = new AnimatorOverrideController(instantiatedHandsAnimator.runtimeAnimatorController);
        animatorOverrideController["Recorded"] = animationClip;
        instantiatedHandsAnimator.runtimeAnimatorController = animatorOverrideController;
        lengthOfAnimationInSeconds = animationClip.length;*/


    }

    private void InstantiateHandsAndSetInactive()
    {
       /* Debug.Log("InstantiateRecordedObjectAndSetInactive");
        Quaternion rotationToInstantiate = Quaternion.identity;
        Vector3 positionToInstantiate = Camera.main.transform.position + 0.3f * Vector3.forward;
        instantiatedHands = Instantiate(original: hands, position: positionToInstantiate, rotation: rotationToInstantiate);
        instantiatedHandsAnimator = instantiatedHands.GetComponent<Animator>();
        instantiatedHands.SetActive(false);*/
    }


    public void Play()
    {
       /* Debug.Log("Play");
        instantiatedHands.SetActive(true);
        instantiatedHandsAnimator.SetTrigger("Play");
        StartCoroutine(SetInstanceInactive());*/
    }

   /* IEnumerator SetInstanceInactive()
    {
        yield return new WaitForSeconds(lengthOfAnimationInSeconds);
        instantiatedHands.SetActive(false);

    }*/

    public void Pause()
    {
        Debug.Log("Pause is not implemented yet");
    }

    public void Stop()
    {
        Debug.Log("Stop is not implemented yet");
    }

}
