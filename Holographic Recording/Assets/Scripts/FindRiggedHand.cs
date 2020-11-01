using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;

public class FindRiggedHand : MonoBehaviour
{
    private TextMeshPro tmPro;


   
    void Start()
    {
        tmPro = GetComponent<TextMeshPro>();
        tmPro.text = "can print something";
    }

    void Update()
    {

        // Log something every 30 frames.
        if (Time.frameCount % 30 == 0)
        {

            GameObject leftHand = GameObject.Find("Left_OurRiggedHandLeft(Clone)/L_Hand/MainL_JNT/WristL_JNT");
            if(leftHand == null)
            {
                tmPro.text = "left hand not found";
            }
            else
            {
                tmPro.text = "Visualizer x coordinate: " + leftHand.transform.position.x;
            }

        }
        

    }
}


