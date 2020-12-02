using Microsoft.Azure.SpatialAnchors;
using Microsoft.Azure.SpatialAnchors.Unity;
using Microsoft.Azure.SpatialAnchors.Unity.Examples;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;
public class AnchorManager : DemoScriptBase
{

    public TextMeshPro debugText;
    private Task saveAnchorTask;
    private bool saved = false;
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
        debugText.text += "Start called \n";
        base.Start();
        SpawnNewAnchoredObject(new Vector3(0.1f, 0, 0), new Quaternion(0, 0, 0, 1));
        //SaveAnchor();
    }

    public async void OnButtonClicked()
    {
        debugText.text += "Button Clicked, calling save current object anchor \n";
        await SaveCurrentObjectAnchorToCloudAsync();

        while (!saved)
        {
            await Task.Yield();
        }
    }


    protected override void OnCloudAnchorLocated(AnchorLocatedEventArgs args)
    {
        base.OnCloudAnchorLocated(args);

        debugText.text += "Anchor loaded \n";


        if (args.Status == LocateAnchorStatus.Located)
        {
            currentCloudAnchor = args.Anchor;

            UnityDispatcher.InvokeOnAppThread(() =>
            {
            // currentAppState = AppState.DemoStepDeleteFoundAnchor;
            Pose anchorPose = Pose.identity;

            // HoloLens: The position will be set based on the unityARUserAnchor that was located.
            SpawnOrMoveCurrentAnchoredObject(anchorPose.position, anchorPose.rotation);
            });
        }
    }

    protected override async Task SaveCurrentObjectAnchorToCloudAsync()

    {

        // Get the cloud-native anchor behavior

        CloudNativeAnchor cna = spawnedObject.GetComponent<CloudNativeAnchor>();

        debugText.text += "get component cloud native anchor \n";


        // If the cloud portion of the anchor hasn't been created yet, create it

        if (cna.CloudAnchor == null) {
            debugText.text += "Calling NativeToCloud \n";

            cna.NativeToCloud(); 
        }



        // Get the cloud portion of the anchor

        CloudSpatialAnchor cloudAnchor = cna.CloudAnchor;
        debugText.text += "get cloud anchor\n";



        // In this sample app we delete the cloud anchor explicitly, but here we show how to set an anchor to expire automatically

        cloudAnchor.Expiration = DateTimeOffset.Now.AddDays(7);

        debugText.text += cloudAnchor.Expiration + " \n";


        while (!CloudManager.IsReadyForCreate)

        {

            await Task.Delay(330);

            float createProgress = CloudManager.SessionStatus.RecommendedForCreateProgress;

            debugText.text += $"Move your device to capture more environment data: {createProgress:0%} \n";

        }



        bool success = false;



        debugText.text += "Saving ...\n";



        try

        {

            // Actually save

            await CloudManager.CreateAnchorAsync(cloudAnchor);



            // Store

            currentCloudAnchor = cloudAnchor;



            // Success?

            success = currentCloudAnchor != null;



            if (success && !isErrorActive)

            {

                // Await override, which may perform additional tasks

                // such as storing the key in the AnchorExchanger
                debugText.text += "Is successful \n";


                await OnSaveCloudAnchorSuccessfulAsync();

                saved = true;
            }

            else

            {
                debugText.text += "Failed \n";

                OnSaveCloudAnchorFailed(new Exception("Failed to save, but no exception was thrown."));
                saved = true;
            }

        }

        catch (Exception ex)

        {
            debugText.text += "Failed \n";

            OnSaveCloudAnchorFailed(ex);
            saved = true;
        }

    }



    // Update is called once per frame
    void Update()
    {

    }

    protected override async Task OnSaveCloudAnchorSuccessfulAsync()

    {

        await base.OnSaveCloudAnchorSuccessfulAsync();

        debugText.text += "Save cloud anchor successful \n";

    }

    protected override void OnSaveCloudAnchorFailed(Exception exception)

    {

        base.OnSaveCloudAnchorFailed(exception);
        
        debugText.text += "Save cloud anchor failed \n";
    }

}
