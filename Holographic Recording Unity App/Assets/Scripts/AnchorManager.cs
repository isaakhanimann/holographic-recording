using Microsoft.Azure.SpatialAnchors;
using Microsoft.Azure.SpatialAnchors.Unity;
using Microsoft.Azure.SpatialAnchors.Unity.Examples;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR.WSA;

public class AnchorManager : DemoScriptBase
{
    GameObject anchorStoreInstance;
    protected AnchorStore anchorStore;
    protected HashSet<string> instantiatedAnchorIds;

    public override Task AdvanceDemoAsync()
    {
        throw new NotImplementedException();
    }

    protected override Color GetStepColor()
    {
        throw new NotImplementedException();
    }

    protected override bool IsPlacingObject()
    {
        throw new NotImplementedException();
    }

    public virtual async void Start()
    {
        anchorStoreInstance = new GameObject();
        anchorStoreInstance.AddComponent<AnchorStore>();
        anchorStore = anchorStoreInstance.GetComponent<AnchorStore>();
        anchorStore.Init();

        instantiatedAnchorIds = new HashSet<string>();

        base.Start();
    }

    public virtual void Update()
    {
        base.Update();
    }

    public async Task InitAnchorSession() {
        // Create CloudSpatialAnchorSession
        if (CloudManager.Session == null)
        {
            await CloudManager.CreateSessionAsync();
        }

        // Start CloudSpatialAnchorSession
        if (!CloudManager.IsSessionStarted)
        {
            await CloudManager.StartSessionAsync();
        }
    }

    // Trying to retrieve anchors from stored file, but somehow after CreateWatcher() is called, app crashes or doesnt trigger anchor_located
    public void FindAnchors()
    {
        ResetAnchorIdsToLocate();

        // Fetch the IDs of anchors stored in the text file
        List<string> anchorKeysToFind = anchorStore.GetAllAnchorIds();

        if (anchorKeysToFind.Count > 0)
        {
            // Set the IDs to locate in the AnchorLocateCriteria object
            SetAnchorIdsToLocate(anchorKeysToFind);

            // Create the watcher for those anchor GUIDs
            CreateWatcher();
        }
    }

    public void DeleteAnchorByRecording(string recordingId)
    {
        anchorStore.DeleteByRecordingId(recordingId);
    }

    public override void OnCloudAnchorLocated(AnchorLocatedEventArgs args)
    {

    }

    protected override void OnCloudLocateAnchorsCompleted(LocateAnchorsCompletedEventArgs args)
    {
        Task.Run(async () =>
        {
            await Task.Delay(2500);
        });
        args.Watcher.Stop();
    }

    public void AddCloudNativeAnchorToObject(ref GameObject go)
    {
        go.AddComponent<CloudNativeAnchor>();
    }

    public async void SaveObjectAnchorToCloud(GameObject go, string recordingId)
    {
        await SaveObjectAnchorToCloudAsyncTask(go, recordingId);
    }

    // Copy pasted this method from DemoScriptBase to allow specifying gameobject to get cloudnativeanchor from that will be saved.
    public async Task SaveObjectAnchorToCloudAsyncTask(GameObject go, string recordingId)
    {
        // Get the cloud-native anchor behavior
        CloudNativeAnchor cna = go.GetComponent<CloudNativeAnchor>();

        // If the cloud portion of the anchor hasn't been created yet, create it
        if (cna.CloudAnchor == null)
        {
            cna.NativeToCloud();
        }

        // Get the cloud portion of the anchor
        CloudSpatialAnchor cloudAnchor = cna.CloudAnchor;

        while (!CloudManager.IsReadyForCreate)
        {
            await Task.Delay(330);
            float createProgress = CloudManager.SessionStatus.RecommendedForCreateProgress;
            Debug.Log($"Move your device to capture more environment data: {createProgress:0%}\n");
        }

        bool success = false;

        try
        {
            // Actually save
            await CloudManager.CreateAnchorAsync(cloudAnchor);

            // Success?
            success = cloudAnchor != null;

            if (success && !isErrorActive)
            {
                anchorStore.Save(cloudAnchor.Identifier, recordingId);

                await OnSaveCloudAnchorSuccessfulAsync();
            }
            else
            {
                OnSaveCloudAnchorFailed(new Exception("Failed to save, but no exception was thrown."));
            }
        }
        catch (Exception ex)
        {
            OnSaveCloudAnchorFailed(ex);
        }
    }

    private int GetRandomNumberBetween1and100000()
    {
        System.Random random = new System.Random();
        int randomInt = random.Next(1, 100001);
        return randomInt;
    }

    protected override async Task OnSaveCloudAnchorSuccessfulAsync()
    {
        await base.OnSaveCloudAnchorSuccessfulAsync();
    }

    protected override void OnSaveCloudAnchorFailed(Exception exception)
    {
        base.OnSaveCloudAnchorFailed(exception);
    }

}
