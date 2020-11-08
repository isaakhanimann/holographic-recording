using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit;
using TMPro;

public class ChangeHandVisibility : MonoBehaviour
{

    public bool isHandMeshVisible = false;
    public bool isHandJointVisible = false;

    public TextMeshPro debugLogTmPro;


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


    public void TurnOnHandMesh()
    {
        isHandMeshVisible = true;
        UpdateHandVisibility();
    }

    public void TurnOffHandMesh()
    {
        isHandMeshVisible = false;
        UpdateHandVisibility();
    }

}
