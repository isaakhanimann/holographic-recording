using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.XR.WSA.Persistence;
using UnityEngine.XR.WSA;
using Microsoft.MixedReality.Toolkit.Experimental.Utilities;
// EXAMPLE CODE FOR HOW TO USE WORLDANCHORSTORE
public class TestAnchor : MonoBehaviour
{
    public TextMeshPro debugLog;
    public WorldAnchorStore store;
    bool isAnchorSaved;
    public WorldAnchorManager worldAnchorManager;

    // Start is called before the first frame update
    void Start()
    {
        // Get world anchor store
        WorldAnchorStore.GetAsync(AnchorStoreLoaded);

    }

    void OnPause()
    {
        SaveGame();
    }

    // Callback for when anchor store is loaded
    private void AnchorStoreLoaded(WorldAnchorStore st)
    {
        store = st;
        LoadAnchors();
    }

    private void LoadAnchors()
    {
        debugLog.text += "\n Called load anchor and name of world anchor game object is : " + gameObject.name.ToString() + "\n";
        bool isAnchorLoaded = store.Load("randomIdtoMakesureNoDuplicate", gameObject);
        if (!isAnchorLoaded)
        {
            Debug.Log("No anchor has been stored yet");
            debugLog.text += "\n Loading anchor but could not find anchor \n"; 
        }
        printIds();
    }

    public void SaveGame()
    {
        WorldAnchor anchor = gameObject.GetComponent<WorldAnchor>();
        // Remove any previous worldanchor saved with the same name so we can save new one
        //store.Delete(gameObject.name.ToString());
        if (!isAnchorSaved)
        {
            if (anchor != null)
            {
                isAnchorSaved = store.Save("randomIdtoMakesureNoDuplicate", anchor);
                if (!isAnchorSaved)
                {
                    Debug.Log("Anchor save failed.");
                    debugLog.text += "\n Anchor saving failed\n";
                }
            }
            else
            {
                debugLog.text += "\n Anchor is null \n";
            }
        }
    }

    public void SaveAnchor()
    {
        WorldAnchor anchor = gameObject.GetComponent<WorldAnchor>();

        if (anchor == null)
        {
            string name = worldAnchorManager.AttachAnchor(gameObject);
            Debug.Log("Added anchor: " + name);
            debugLog.text += "Added anchor " + name + "\n";
        } else
        {
            debugLog.text += "anchor already added\n";
        }
         
    }

    private void ClearAnchor()
    {
        WorldAnchor anchor = gameObject.GetComponent<WorldAnchor>();
        if (anchor)
        {
            // remove any world anchor component from the game object so that it can be moved
            DestroyImmediate(anchor);
        }
    }

    // How to iterate all ids stored
    private void printIds()
    {
        string[] ids = store.GetAllIds();
        for (int index = 0; index < ids.Length; index++)
        {
            Debug.Log(ids[index]);
            debugLog.text += ids[index] + "\n";
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}