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
    string filenamePrefix = "AUDIOFILE_";
    bool isSaved = true;
    // Start is called before the first frame update
    void Start()
    {
        persistentPath = Application.persistentDataPath;
        lengthInSamplesContainer = new NativeArray<int>(1, Allocator.Persistent);
        channelsContainer = new NativeArray<int>(1, Allocator.Persistent);
        frequencyContainer = new NativeArray<int>(1, Allocator.Persistent);
    }

    // Update is called once per frame
    void Update()
    {
        // Keep track of time to cut audio short;
        if (isRecording)
        {
            duration += Time.deltaTime;
        }
    }

    public void StartRecording()
    {
        isRecording = true;
        audioClip = Microphone.Start(null, false, MAX_RECORD_TIME, 44100);
    }

    public void StopAndSaveRecording(string filename)
    {
        isRecording = false;
        isSaving = true;

        Microphone.End(null);

        int lengthInSamples = (int)(Mathf.Ceil(duration) * audioClip.channels * audioClip.frequency);

        var originalSamples = new float[audioClip.samples];
        audioClip.GetData(originalSamples, 0);

        originalSamplesContainer = new NativeArray<float>(originalSamples, Allocator.Persistent);
        string filepath = Path.Combine(persistentPath, filenamePrefix + filename);
        pathContainer = new NativeArray<char>(filepath.ToCharArray(), Allocator.Persistent);

        lengthInSamplesContainer[0] = lengthInSamples;
        channelsContainer[0] = audioClip.channels;
        frequencyContainer[0] = audioClip.frequency;

        CutAudioJob cutJob = new CutAudioJob()
        {
            originalSamplesContainer = originalSamplesContainer,
            lengthInSamplesContainer = lengthInSamplesContainer,
            channelsContainer = channelsContainer,
            frequencyContainer = frequencyContainer,
            pathContainer = pathContainer,
        };

        isSaved = false;
        cutJobHandle = cutJob.Schedule();

        duration = 0;
    }

    private void LateUpdate()
    {
        if (!isSaved) {
            cutJobHandle.Complete();
            isSaved = true;
        }
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
