using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit;
using TMPro;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System;
using Unity.Jobs;
using Unity.Collections;

public class AudioRecorder : MonoBehaviour
{
    int MAX_RECORD_TIME = 2000;

    public TextMeshPro debugLog;

    private AudioClip audioClip;
    private float duration = 0;
    private bool isRecording = false;
    private bool isSaving = false;

    NativeArray<float> originalSamplesContainer;
    NativeArray<int> lengthInSamplesContainer;
    NativeArray<int> channelsContainer;
    NativeArray<int> frequencyContainer;
    NativeArray<char> pathContainer;

    JobHandle cutJobHandle;
    string persistentPath;

    // Start is called before the first frame update
    void Start()
    {
       
        persistentPath = Application.persistentDataPath;
        string filepath = Path.Combine(persistentPath, "AUDIOFILE_TEST_UNITY");
        debugLog.text += filepath + "\n";
        lengthInSamplesContainer = new NativeArray<int>(1, Allocator.Persistent);
        channelsContainer = new NativeArray<int>(1, Allocator.Persistent);
        frequencyContainer = new NativeArray<int>(1, Allocator.Persistent);
        pathContainer = new NativeArray<char>(filepath.ToCharArray(), Allocator.Persistent);
    }

    // Update is called once per frame
    void Update()
    {
        // Keep track of time to cut audio short;
        if (isRecording)
        {
            duration += Time.deltaTime;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            StartRecording();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            StopAndSaveRecording();
        }

    }

    public void StartRecording()
    {
        isRecording = true;
        debugLog.text += "a was pressed: Recording audio" + System.Environment.NewLine;
        audioClip = Microphone.Start(null, false, MAX_RECORD_TIME, 44100);
    }

    public void StopAndSaveRecording()
    {

        isRecording = false;
        isSaving = true;

        Microphone.End(null);
        debugLog.text += "StopAndSaveRecording executed: Saving audio of length " + duration + System.Environment.NewLine;

        Debug.Log(audioClip.channels);

        int lengthInSamples = (int)(Mathf.Ceil(duration) * audioClip.channels * audioClip.frequency);

        var originalSamples = new float[audioClip.samples];
        audioClip.GetData(originalSamples, 0);


        Debug.Log("1");

        originalSamplesContainer = new NativeArray<float>(originalSamples, Allocator.Persistent);

        lengthInSamplesContainer[0] = lengthInSamples;
        channelsContainer[0] = audioClip.channels;
        frequencyContainer[0] = audioClip.frequency;
        Debug.Log("2");

        CutAudioJob cutJob = new CutAudioJob()
        {
            originalSamplesContainer = originalSamplesContainer,
            lengthInSamplesContainer = lengthInSamplesContainer,
            channelsContainer = channelsContainer,
            frequencyContainer = frequencyContainer,
            pathContainer = pathContainer,
        };

        Debug.Log("3");

        cutJobHandle = cutJob.Schedule();
        debugLog.text += "scheduled job \n";

        duration = 0;
    }

    private void LateUpdate()
    {
        cutJobHandle.Complete();
    }

    private void OnDestroy()
    {
        // make sure to Dispose() any NativeArrays when we're done
        lengthInSamplesContainer.Dispose();
        originalSamplesContainer.Dispose();
        channelsContainer.Dispose();
        frequencyContainer.Dispose();
        pathContainer.Dispose();
    }


    public struct CutAudioJob : IJob
    {
        public NativeArray<float> originalSamplesContainer;
        public NativeArray<int> lengthInSamplesContainer;
        public NativeArray<int> channelsContainer;
        public NativeArray<int> frequencyContainer;
        public NativeArray<char> pathContainer;
        public void Execute()
        {
            Debug.Log("in job");

            float[] cutSamples = new float[lengthInSamplesContainer[0]];
            for (int i = 0; i < lengthInSamplesContainer[0]; i++)
            {
                cutSamples[i] = originalSamplesContainer[i];

            }

            SavWav.Save(
                new String(pathContainer.ToArray()),
                cutSamples,
                frequencyContainer[0],
                channelsContainer[0],
                lengthInSamplesContainer[0]
            );
        }
    }
}