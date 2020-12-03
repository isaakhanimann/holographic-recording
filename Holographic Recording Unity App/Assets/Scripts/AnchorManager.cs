using Microsoft.Azure.SpatialAnchors;
using Microsoft.Azure.SpatialAnchors.Unity;
using Microsoft.Azure.SpatialAnchors.Unity.Examples;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using UnityEngine.XR.WSA;

public class AnchorManager : DemoScriptBase
{

    private string currentAnchorId = "";
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

    void Start()
    {
        debugText.text += "AnchorManager.Start() called \n";
        base.Start();
        base.SpawnOrMoveCurrentAnchoredObject(new Vector3(0.1f, 0, 0), new Quaternion(0, 0, 0, 1));
    }

    // Step 1
    public void OnCreateSessionClicked()
    {
        CreateSession();
    }

    // Step 2
    public void OnStartSessionClicked()
    {
        StartSession();
    }

    // Step 3
    public void OnSaveAnchorClicked()
    {
        debugText.text += "Button Clicked, calling save current object anchor \n";
        SaveAnchorForObjectAsync();
    }

    // For this poc we need to reset session manually (since Id is stored locally)
    public void OnResetSessionClicked()
    {
        ResetSession();
    }

    // Step 5 Find Anchors
    public void FindAnchors()
    {
        List<string> anchorsToFind = new List<string>();
        debugText.text += currentAnchorId + "\n";

        anchorsToFind.Add(currentAnchorId);

        base.SetAnchorIdsToLocate(anchorsToFind);

        currentWatcher = base.CreateWatcher();
    }

    async void SaveAnchorForObjectAsync()
    {
        await SaveCurrentObjectAnchorToCloudAsync();
    }


    // Update is called once per frame
    void Update()
    {

    }

    async void ResetSession()
    {
        CloudManager.StopSession();
        base.CleanupSpawnedObjects();
        await CloudManager.ResetSessionAsync();
    }

   
    async void CreateSession()
    {
        if (CloudManager.Session == null)

        {
            debugText.text += "Creating session \n";

            await CloudManager.CreateSessionAsync();

        }

        currentCloudAnchor = null;
    }

    async void StartSession()
    {
        debugText.text += "Start session \n";

        await CloudManager.StartSessionAsync();
    }

    protected override void OnCloudAnchorLocated(AnchorLocatedEventArgs args)
    {
        debugText.text += "ON Cloud anchor located \n";

        //base.OnCloudAnchorLocated(args);
        //if (args.Status == LocateAnchorStatus.Located)
        //{

            currentCloudAnchor = args.Anchor;
        debugText.text += "get located anchor \n";

        UnityDispatcher.InvokeOnAppThread(() =>

            {
                debugText.text += "spawning object again \n";

                Pose anchorPose = Pose.identity;

                SpawnOrMoveCurrentAnchoredObject(anchorPose.position, anchorPose.rotation);

                UnityEngine.XR.WSA.WorldAnchor wa = spawnedObject.AddComponent<UnityEngine.XR.WSA.WorldAnchor>();
                wa.SetNativeSpatialAnchorPtr(currentCloudAnchor.LocalAnchor);
                debugText.text += "Spawned and moved object \n";
            });

       // }
    }

    protected override async Task SaveCurrentObjectAnchorToCloudAsync()
    {
        await base.SaveCurrentObjectAnchorToCloudAsync();
    }


    protected override async Task OnSaveCloudAnchorSuccessfulAsync()
    {
        await base.OnSaveCloudAnchorSuccessfulAsync();

        // For now store Id locally (needs to be retrieved from ASA for cross device persitence)
        currentAnchorId = currentCloudAnchor.Identifier;
        debugText.text += currentAnchorId + "\n";

        debugText.text += "Save cloud anchor successful \n";
    }

    protected override void OnSaveCloudAnchorFailed(Exception exception)
    {
        base.OnSaveCloudAnchorFailed(exception);

        debugText.text += "Save cloud anchor failed \n";
    }

}
