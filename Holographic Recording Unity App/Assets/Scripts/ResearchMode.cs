using UnityEngine;
using System;
using System.Runtime.InteropServices;
using UnityEngine.UI;
using System.IO;

#if ENABLE_WINMD_SUPPORT
using HL2UnityPlugin;
#endif

public class ResearchMode : MonoBehaviour
{
#if ENABLE_WINMD_SUPPORT
    HL2ResearchMode researchMode;
#endif
    static bool startRealtimePreview = false;
    static float startTime = 0.0F;
    static int recordingNumber = 0;

    void Start()
    {
#if ENABLE_WINMD_SUPPORT
        researchMode = new HL2ResearchMode();
        researchMode.InitializeDepthSensor();
        
#endif
        StartDepthSensingLoopEvent();
    }

    public void StartDepthSensingLoopEvent()
    {
#if ENABLE_WINMD_SUPPORT
        researchMode.StartDepthSensorLoop();
#endif
    }

    public void StopSensorLoopEvent()
    {
#if ENABLE_WINMD_SUPPORT
        researchMode.StopAllSensorDevice();
#endif
    }

    // use this call to control recording
    public static void TogglePreviewEvent(int numberOfRecording)
    {
        startRealtimePreview = !startRealtimePreview;
        if (startRealtimePreview)
        {
            startTime = Time.time;
            recordingNumber = numberOfRecording;
        }
    }

    private void LateUpdate()
    {
#if ENABLE_WINMD_SUPPORT
        // update depth map texture
        if (startRealtimePreview && researchMode.PointCloudUpdated())
        {
            float[] pointCloud = researchMode.GetPointCloudBuffer();
            if (pointCloud.Length > 0)
            {
                float timeStamp = Time.time - startTime;
                var byteArray = new byte[pointCloud.Length * 4];
                Buffer.BlockCopy(pointCloud, 0, byteArray, 0, byteArray.Length);
                File.WriteAllBytes(String.Format(Application.persistentDataPath+"/points_{0}_{1}.dat", numberOfRecording, timeStamp), byteArray);
                Debug.Log("research mode: point cloud data written");
            }
        }
#endif
    }
}