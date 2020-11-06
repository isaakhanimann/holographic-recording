using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimerBehaviour : MonoBehaviour
{

    public TextMeshPro textMeshPro;

    private int currentRecordingTimeInSeconds = 0;


    public void StartTimer()
    {
        gameObject.SetActive(true);
        currentRecordingTimeInSeconds = 0;
        StartCoroutine(UpdateText());
    }

    public void StopTimer()
    {
        gameObject.SetActive(false);
        StopCoroutine(UpdateText());
    }

    IEnumerator UpdateText()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            currentRecordingTimeInSeconds += 1;
            textMeshPro.text = currentRecordingTimeInSeconds + "s";
        }

    }
}
