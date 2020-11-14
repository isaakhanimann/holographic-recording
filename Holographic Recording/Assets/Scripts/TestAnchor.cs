using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.XR.WSA.Persistence;
using UnityEngine.XR.WSA;

// EXAMPLE CODE FOR HOW TO USE WORLDANCHORSTORE
public class TestAnchor : MonoBehaviour
{
    public WorldAnchorStore store;
    GameObject gameObject;

    // Start is called before the first frame update
    void Start()
    {   
        // Get world anchor store
        WorldAnchorStore.GetAsync(AnchorStoreLoaded);

    }
    // Callback for when anchor store is loaded
    private void AnchorStoreLoaded(WorldAnchorStore st)
    {
        store = st;
        LoadAnchors();
    }

    private void LoadAnchors()
    {    
        bool isAnchorLoaded = store.Load(gameObject.name, gameObject);
        if (!isAnchorLoaded)
        {
            // Until the gameObjectIWantAnchored has an anchor saved at least once it will not be in the AnchorStore
        }
    }

    private void SaveAnchor()
    {
        bool isAnchorSaved;
        WorldAnchor anchor = gameObject.AddComponent<WorldAnchor>();
        // Remove any previous worldanchor saved with the same name so we can save new one
        this.store.Delete(gameObject.name.ToString()); 
        isAnchorSaved = this.store.Save(gameObject.name.ToString(), anchor);
        if (!isAnchorSaved)
        {
            Debug.Log("Anchor save failed.");
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
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
