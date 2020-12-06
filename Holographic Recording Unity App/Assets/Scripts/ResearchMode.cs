using UnityEngine;
using System;
using System.Runtime.InteropServices;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using System.Collections.Generic;

#if ENABLE_WINMD_SUPPORT
using HL2UnityPlugin;
#endif

public class ResearchMode : MonoBehaviour
{
#if ENABLE_WINMD_SUPPORT
    HL2ResearchMode researchMode;
#endif
    bool startRealtimePreview = false;
    float startTime = 0.0F;
    int frameCount = 0;
    Dictionary<int, float> timesteps;
    Dictionary<int, float[]> points;

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
    public void RecordDepthData(bool b)
    {
        startRealtimePreview = b;
        if (startRealtimePreview)
        {
            startTime = Time.time;
            frameCount = 0;
            timesteps = new Dictionary<int, float>();
            points = new Dictionary<int, float[]>();
        }
    }

    private void LateUpdate()
    {
#if ENABLE_WINMD_SUPPORT
        // update depth map texture
        if (startRealtimePreview && researchMode.PointCloudUpdated())
        {
            frameCount++;
            float[] pointCloud = researchMode.GetPointCloudBuffer();
            if (pointCloud.Length > 0)
            {
                float timeStamp = Time.time - startTime;
                timesteps[frameCount] = timeStamp;
                points[frameCount] = pointCloud;
            }
        }
#endif
    }

    public void SavePointClouds(int num_recording)
    {
        foreach(int timestamp_key in timesteps.Keys)
        {
            if (points.ContainsKey(timestamp_key))
            {
                float[] pointCloud = points[timestamp_key];
                var byteArray = new byte[pointCloud.Length * 4];
                Buffer.BlockCopy(pointCloud, 0, byteArray, 0, byteArray.Length);
                File.WriteAllBytes(String.Format(Application.persistentDataPath + "/points_{0}_{1}.dat", num_recording, timesteps[timestamp_key]), byteArray);
                Debug.Log("research mode: point cloud data written"+ num_recording.ToString());
            }
        }
        
    }
}