using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit;
using TMPro;

// this class is supposed to disable the hand visualization by default and only enable it when we are recording
// it doesn't work yet
public class ChangeHandVisibility : MonoBehaviour
{

    public bool isHandMeshVisible = false;
    public bool isHandJointVisible = false;


    private void Start()
    {
        UpdateHandVisibility();
    }


    private void UpdateHandVisibility()
    {
        MixedRealityHandTrackingProfile handTrackingProfile = CoreServices.InputSystem?.InputSystemProfile?.HandTrackingProfile;
        if (handTrackingProfile != null)
        {
            handTrackingProfile.EnableHandMeshVisualization = isHandMeshVisible;
            handTrackingProfile.EnableHandJointVisualization = isHandJointVisible;
        }
    }

    // called when Start Recording is pressed
    public void TurnOnHandMesh()
    {
        isHandMeshVisible = true;
        UpdateHandVisibility();
    }

    // called when Stop or Cancel Recording is pressed
    public void TurnOffHandMesh()
    {
        isHandMeshVisible = false;
        UpdateHandVisibility();
    }

}
