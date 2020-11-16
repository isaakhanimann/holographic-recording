using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera: MonoBehaviour
{

    private Camera mainCamera;
    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        gameObject.transform.LookAt(mainCamera.transform);
    }
}
