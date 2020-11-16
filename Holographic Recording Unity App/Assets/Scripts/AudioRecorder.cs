using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Jobs;
using Unity.Collections;


public class AudioRecorder : MonoBehaviour
{
    int MAX_RECORD_TIME = 2000;
    private AudioClip audioClip;
    private float duration = 0;
    private bool isRecording = false;

    NativeArray<float> originalSamplesContainer;
    NativeArray<float> cutSamplesContainer;

    NativeArray<int> lengthInSamplesContainer;
    NativeArray<int> channelsContainer;
    NativeArray<int> frequencyContainer;

    JobHandle cutJobHandle;
    CutAudioJob cutJob;

    // Start is called before the first frame update
    void Start()
    {
        lengthInSamplesContainer = new NativeArray<int>(1, Allocator.Persistent);
        channelsContainer = new NativeArray<int>(1, Allocator.Persistent);
        frequencyContainer = new NativeArray<int>(1, Allocator.Persistent);
    }

    // Update is called once per frame
    void Update()
    {   
        // Keep track of time to cut audio short;
        if (isRecording) {
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

    public void StartRecording() {
        isRecording = true;
        Debug.Log("a was pressed: Recording audio");
        audioClip = Microphone.Start ( null, false, MAX_RECORD_TIME, 44100 );
    }

    public void StopAndSaveRecording() {
        isRecording = false;
        Microphone.End(null);
        Debug.Log("StopAndSaveRecording executed: Saving audio of length " + duration);

        int lengthInSamples = (int)(Mathf.Ceil(duration) * audioClip.channels * audioClip.frequency);

        var originalSamples = new float[audioClip.samples];
        audioClip.GetData(originalSamples, 0);

        originalSamplesContainer = new NativeArray<float>(originalSamples, Allocator.Persistent);
        cutSamplesContainer = new NativeArray<float>(new float[lengthInSamples], Allocator.Persistent);

        lengthInSamplesContainer[0] = lengthInSamples;
        channelsContainer[0] = audioClip.channels;
        frequencyContainer[0] = audioClip.frequency;
        
        cutJob = new CutAudioJob()
        {
            originalSamplesContainer = originalSamplesContainer,
            cutSamplesContainer = cutSamplesContainer,
            lengthInSamplesContainer = lengthInSamplesContainer,
            channelsContainer = channelsContainer,
            frequencyContainer = frequencyContainer
        };

        cutJobHandle = cutJob.Schedule();

        duration = 0;       
    }

/*    private void LateUpdate()
    {
        cutJobHandle.Complete();

        SavWav.Save(
            "AUDIOFILE_TEST_UNITY",
            cutJob.cutSamplesContainer.ToArray(),
            cutJob.frequencyContainer[0],
            cutJob.channelsContainer[0],
            cutJob.lengthInSamplesContainer[0]
        );
    }*/

    private void OnDestroy()
    {
        // make sure to Dispose() any NativeArrays when we're done
        lengthInSamplesContainer.Dispose();
        originalSamplesContainer.Dispose();
        cutSamplesContainer.Dispose();
        channelsContainer.Dispose();
        frequencyContainer.Dispose();
    }


    public struct CutAudioJob : IJob
    {
        public NativeArray<float> originalSamplesContainer;
        public NativeArray<float> cutSamplesContainer;
        public NativeArray<int> lengthInSamplesContainer;
        public NativeArray<int> channelsContainer;
        public NativeArray<int> frequencyContainer;

        public void Execute()
        {
            for (int i = 0; i < lengthInSamplesContainer[0]; i++)
            {
                cutSamplesContainer[i] = originalSamplesContainer[i];
            }
        }
    }
}
