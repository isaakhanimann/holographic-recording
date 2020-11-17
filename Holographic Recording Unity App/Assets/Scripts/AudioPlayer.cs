using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AudioPlayer : MonoBehaviour
{
    AudioSource audio;

    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponent<AudioSource>();
    }

    public void PlayAudio()
    {
        string filename = "AUDIOFILE_TEST_UNITY.wav";
        StartCoroutine(PlayAudioClip(Application.persistentDataPath + "/" + filename));
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            print("Z was pressed: playing sound");
            PlayAudio();
        }
    }

    IEnumerator PlayAudioClip(string path)
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + path, AudioType.WAV))
        {
            yield return www.Send();

            if (www.isNetworkError)
            {
                Debug.Log(www.error);
            }
            else if (www != null && www.isDone)
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                audio.clip = clip;
                audio.Play();
            }
        }
    }
}