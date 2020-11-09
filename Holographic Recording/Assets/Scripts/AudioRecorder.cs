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

    public TextMeshPro debugLog;

    private AudioClip audioClip;
    private float duration = 0;
    private bool isRecording = false;

    // Start is called before the first frame update
    void Start()
    {

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
        debugLog.text += "a was pressed: Recording audio" + System.Environment.NewLine;
        audioClip = Microphone.Start ( null, false, MAX_RECORD_TIME, 44100 );
    }

    public void StopAndSaveRecording() {
        isRecording = false;
        Microphone.End(null);
        debugLog.text += "StopAndSaveRecording executed: Saving audio of length " + duration + System.Environment.NewLine;

        int lengthInSamples = (int)(Mathf.Ceil(duration) * audioClip.channels * audioClip.frequency);

        var originalSamples = new float[audioClip.samples];
        audioClip.GetData(originalSamples, 0);


        CutAudioJob cutJob = new CutAudioJob();
        NativeArray<float> originalSamplesContainer = new NativeArray<float>(originalSamples, Allocator.Persistent);
        cutJob.originalSamplesContainer = originalSamplesContainer;

        NativeArray<int> lengthInSamplesContainer = new NativeArray<int>(1, Allocator.Persistent);
        lengthInSamplesContainer[0] = lengthInSamples;
        cutJob.lengthInSamplesContainer = lengthInSamplesContainer;

        NativeArray<int> channelsContainer = new NativeArray<int>(1, Allocator.Persistent);
        channelsContainer[0] = audioClip.channels;
        cutJob.channelsContainer = channelsContainer;

        NativeArray<int> frequencyContainer = new NativeArray<int>(1, Allocator.Persistent);
        frequencyContainer[0] = audioClip.frequency;
        cutJob.frequencyContainer = frequencyContainer;

        JobHandle cutJobHandle = cutJob.Schedule();

        duration = 0;
    }


    public struct CutAudioJob : IJob
    {
        public NativeArray<float> originalSamplesContainer;
        public NativeArray<int> lengthInSamplesContainer;
        public NativeArray<int> channelsContainer;
        public NativeArray<int> frequencyContainer;

        public void Execute()
        {
            List<float> samplesCutShort = new List<float>();
            for (int i = 0; i < lengthInSamplesContainer[0]; i++)
            {
                samplesCutShort.Add(originalSamplesContainer[i]);
            }

            AudioClip audioClip = AudioClip.Create("audioClip", lengthInSamplesContainer[0], channelsContainer[0], frequencyContainer[0], false);
            audioClip.SetData(samplesCutShort.ToArray(), 0);

            SavWav.Save("AUDIOFILE_TEST_UNITY", audioClip);
        }
    }
}
