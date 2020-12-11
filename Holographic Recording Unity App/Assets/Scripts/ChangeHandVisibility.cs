using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit;
using TMPro;

// this class is supposed to disable the hand visualization by default and only enable it when we are recording
public class ChangeHandVisibility : MonoBehaviour
{

    public bool isHandMeshVisible = false;
    public Material normalHandMaterial;
    public Material invisibleMaterial;

    private int FRAME_FREQUENCY = 10;

    private void Update()
    {
        if (Time.frameCount % FRAME_FREQUENCY == 0)
        {
            SetHandsVisibiliy();
        }
    }


    private void SetHandsVisibiliy()
    {
        GameObject leftHand = GameObject.Find("Left_OurRiggedHandLeft(Clone)");
        if (leftHand != null)
        {
            if (isHandMeshVisible)
            {
                leftHand.GetComponentInChildren<SkinnedMeshRenderer>().material = normalHandMaterial;
            }
            else
            {
                leftHand.GetComponentInChildren<SkinnedMeshRenderer>().material = invisibleMaterial;
            }
        }
        GameObject rightHand = GameObject.Find("Right_OurRiggedHandRight(Clone)");
        if (rightHand != null)
        {
            if (isHandMeshVisible)
            {
                rightHand.GetComponentInChildren<SkinnedMeshRenderer>().material = normalHandMaterial;
            }
            else
            {
                rightHand.GetComponentInChildren<SkinnedMeshRenderer>().material = invisibleMaterial;
            }
        }
    }


    // called when Start Recording is pressed
    public void TurnOnHandMesh()
    {
        isHandMeshVisible = true;
    }

    // called when Stop or Cancel Recording is pressed
    public void TurnOffHandMesh()
    {
        isHandMeshVisible = false;
    }

}
