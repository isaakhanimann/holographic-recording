using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LogHandTransformsToPanel : MonoBehaviour
{
    public GameObject debugLogsObject;

    
    void Update()
    {
        debugLogsObject.GetComponent<TextMeshPro>().text = $"x = {gameObject.transform.localPosition.x}, y = {gameObject.transform.localPosition.y}, z = {gameObject.transform.localPosition.z}";
    }
}
