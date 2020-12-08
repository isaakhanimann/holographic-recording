﻿using UnityEngine;
using System;
using System.Runtime.InteropServices;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit;

#if ENABLE_WINMD_SUPPORT
using HL2UnityPlugin;
#endif

public class Test2 : MonoBehaviour
{
#if ENABLE_WINMD_SUPPORT
    HL2ResearchMode researchMode;
#endif
    bool startRealtimePreview = true;
    float startTime = 0.0F;
    int frameCount = 0;
    Dictionary<int, float> timesteps;
    Dictionary<int, Vector3> leftPalmCoords;
    Dictionary<int, Vector3> rightPalmCoords;
    Dictionary<int, float[]> points;

    private const float THRESH = 0.3F;
    private const bool filterPointClouds = true;

    Mesh mesh;
    //public MeshRenderer meshRenderer;
    MeshFilter mf;

    private bool showRecording = true;
    private int runningFrame = -1;
    private string[] files;

    void Start()
    {
#if ENABLE_WINMD_SUPPORT
        researchMode = new HL2ResearchMode();
        researchMode.InitializeDepthSensor();
#endif
        StartDepthSensingLoopEvent();
        mf = gameObject.AddComponent<MeshFilter>();
        mesh = new Mesh
        {
            // Use 32 bit integer values for the mesh, allows for stupid amount of vertices (2,147,483,647 I think?)
            indexFormat = UnityEngine.Rendering.IndexFormat.UInt32
        };
        mf.mesh = mesh;

        //Vector3 v = new Vector3(0, 1, 0);
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
    }

    void UpdateMesh(float[] points)
    {
        //runningFrame++;
        Debug.Log("Update Mesh");

        //Vector3[] positions = readFile(files[runningFrame]);
        //Debug.Log(files[runningFrame]);

        //Debug.Log("Renderer: points received");
        //if (positions == null || positions.Length == 0)
        //{
        //    Debug.Log("Empty array");
        //    return;
        //}
        int size = points.Length/3;
        Vector3[] positions = new Vector3[size];

        Color[] colours = new Color[size];
        Debug.Log(size);

        for (int n = 0; n < size; n++)
        {
            positions[n] = new Vector3(points[n*3], points[n * 3 + 1], points[n * 3 + 2]);
            colours[n] = new Color(1, 1, 1, 1);
        }

        mf.mesh.Clear();

        Mesh mesh2 = new Mesh
        {
            indexFormat = UnityEngine.Rendering.IndexFormat.UInt32
        };
        mesh2.vertices = positions;
        mesh2.colors = colours;
        int[] indices = new int[positions.Length];

        for (int i = 0; i < positions.Length; i++)
        {
            indices[i] = i;
        }

        mesh2.SetIndices(indices, MeshTopology.Points, 0);
        mesh2.RecalculateNormals();
        mf.mesh = mesh2;
        Debug.Log("Renderer: points updated");
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
            leftPalmCoords = new Dictionary<int, Vector3>();
            rightPalmCoords = new Dictionary<int, Vector3>();
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
            UpdateMesh(pointCloud);
            //if (pointCloud.Length > 0)
            //{
            //    float timeStamp = Time.time - startTime;
            //    timesteps[frameCount] = timeStamp;
            //    points[frameCount] = pointCloud;
            //    StorePalmVectors(frameCount);
            //}
        }
#endif
    }

    private void StorePalmVectors(int count)
    {
        // get mrtk joints
        Vector3 leftPalm = new Vector3();
        Vector3 rightPalm = new Vector3();

        if (HandJointUtils.TryGetJointPose(TrackedHandJoint.Palm, Handedness.Left, out MixedRealityPose leftPose))
        {
            leftPalm = leftPose.Position;
        }

        if (HandJointUtils.TryGetJointPose(TrackedHandJoint.Palm, Handedness.Right, out MixedRealityPose rightPose))
        {
            rightPalm = rightPose.Position;
        }

        if (leftPalm == null)
        {
            leftPalm = new Vector3(0, 0, 0);
        }

        if (rightPalm == null)
        {
            rightPalm = new Vector3(0, 0, 0);
        }

        leftPalmCoords[count] = leftPalm;
        rightPalmCoords[count] = rightPalm;
    }

    public void SavePointClouds(int num_recording)
    {
        foreach (int timestamp_key in timesteps.Keys)
        {
            if (points.ContainsKey(timestamp_key))
            {
                float[] pointCloud = points[timestamp_key];

                if (filterPointClouds)
                {
                    int size = pointCloud.Length / 3;
                    Vector3[] pcl_temp = new Vector3[size];
                    bool[] filter = new bool[size];
                    int n_points = 0;

                    for (int n = 0; n < size; n++)
                    {
                        float min_base = float.PositiveInfinity;

                        Vector3 pt = new Vector3(pointCloud[n * 3], pointCloud[n * 3 + 1], pointCloud[n * 3 + 2]);
                        pcl_temp[n] = pt;

                        float dist1 = Vector3.Distance(pt, leftPalmCoords[timestamp_key]);
                        float dist2 = Vector3.Distance(pt, rightPalmCoords[timestamp_key]);
                        float m = Math.Min(min_base, Math.Min(dist1, dist2));

                        if (m < THRESH)
                        {
                            n_points++;
                            filter[n] = true;
                        }
                        else
                        {
                            filter[n] = false;
                        }
                    }

                    pointCloud = new float[n_points * 3];
                    int k = 0;
                    for (int n = 0; n < size; n++)
                    {
                        if (filter[n])
                        {
                            Vector3 temp = pcl_temp[n];
                            pointCloud[k] = temp[0];
                            pointCloud[k + 1] = temp[1];
                            pointCloud[k + 2] = temp[2];
                            k += 3;
                        }
                    }
                }

                var byteArray = new byte[pointCloud.Length * 4];
                Buffer.BlockCopy(pointCloud, 0, byteArray, 0, byteArray.Length);
                File.WriteAllBytes(String.Format(Application.persistentDataPath + "/points_{0}_{1}.dat", num_recording, timesteps[timestamp_key]), byteArray);
                Debug.Log("research mode: point cloud data written" + num_recording.ToString());
            }
        }

    }
}
