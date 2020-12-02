using Microsoft.Azure.SpatialAnchors;
using Microsoft.Azure.SpatialAnchors.Unity;
using Microsoft.Azure.SpatialAnchors.Unity.Examples;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;

public class AnchorManager : DemoScriptBase
{

    public TextMeshPro debugText;
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
    SpawnNewAnchoredObject(new Vector3(0.1f, 0, 0), new Quaternion(0, 0, 0, 1));
    SaveAnchor();
    }

    private async Task SaveAnchor()
    {
    await SaveCurrentObjectAnchorToCloudAsync();
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
