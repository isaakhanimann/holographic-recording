using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableDeveloperConsole : MonoBehaviour
{

    void Update()
    {
        if(Debug.developerConsoleVisible == true)
        {
            Debug.developerConsoleVisible = false;
        }
    }
}
