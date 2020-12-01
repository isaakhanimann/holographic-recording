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

    void Start()
    {
#if ENABLE_WINMD_SUPPORT
        researchMode = new HL2ResearchMode();
        researchMode.InitializeDepthSensor();
        
#endif
        StartPreviewEvent();
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

    static bool startRealtimePreview = false;
    public static void StartPreviewEvent()
    {
        startRealtimePreview = !startRealtimePreview;
    }

    private void LateUpdate()
    {
#if ENABLE_WINMD_SUPPORT
        // update depth map texture
        if (startRealtimePreview && researchMode.PointCloudBufferUpdated())
        {
            float[] pointCloud = researchMode.GetPointCloudBuffer();
            if (pointCloud.Length > 0)
            {
                //var byteArray = new byte[pointCloud.Length * 4];
                //Buffer.BlockCopy(pointCloud, 0, byteArray, 0, byteArray.Length);
                //File.WriteAllBytes(String.Format(Application.persistentDataPath+"/"+"pointCloud_{0}.dat", frame), byteArray2);
                Debug.Log("research mode: point cloud data written");
            }
        }
#endif
    }
}