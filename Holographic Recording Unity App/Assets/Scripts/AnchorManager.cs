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

  void Start()
  {
    anchorStoreInstance = new GameObject();
	anchorStoreInstance.AddComponent<AnchorStore>();
	anchorStore = anchorStoreInstance.GetComponent<AnchorStore>();
	anchorStore.Init();

	instantiatedAnchorIds = new HashSet<string>();

	base.Start();
	// base.SpawnOrMoveCurrentAnchoredObject(new Vector3(1, 0, 0.5f), new Quaternion(0, 0, 0, 1));
  }

  // Step 1
  public void OnInitSessionClicked()
  {
	InitAnchorSession();
  }

  // Step 2
  public void OnSaveAnchorClicked()
  {
	SaveAnchorForObjectAsync();
  }

  // For this poc we need to reset session manually (since Id is stored locally)
  public void OnResetSessionClicked()
  {
	ResetSession();
  }

  // Step 4 Find Anchors
  public void FindAnchors()
  {
	FindAnchorsTask();
  }

  public void DeleteAllAnchors() {
	  anchorStore.DeleteAll();
  }

  // Trying to retrieve anchors from stored file, but somehow after CreateWatcher() is called, app crashes or doesnt trigger anchor_located
  public async void FindAnchorsTask()
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

  async void SaveAnchorForObjectAsync()
  {
	await SaveCurrentObjectAnchorToCloudAsync();
  }

  async void ResetSession()
  {
	CloudManager.StopSession();
	base.CleanupSpawnedObjects();
	await GetComponent<SpatialAnchorManager>().ResetSessionAsync();
  }


  public async void InitAnchorSession()
  {
	base.Start();

	currentCloudAnchor = null;

	if (CloudManager.Session == null)
	{
	  await CloudManager.CreateSessionAsync();
	}

	if (!CloudManager.IsSessionStarted)
	{
	  await CloudManager.StartSessionAsync();
	}
  }

  public void DeleteAnchorByRecording(string recordingId) {
	anchorStore.DeleteByRecordingId(recordingId);
  }


  protected override void OnCloudLocateAnchorsCompleted(LocateAnchorsCompletedEventArgs args)
  {
	Task.Run(async () => {
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
	  debugText.text += $"Move your device to capture more environment data: {createProgress:0%}\n";
	}

	bool success = false;

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
		anchorStore.Save(currentCloudAnchor.Identifier, recordingId);
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

		// For now store Id locally (needs to be retrieved from ASA for cross device persitence)
		debugText.text += currentCloudAnchor.Identifier + "\n";
		// CloudManager.StopSession();
	}

  protected override void OnSaveCloudAnchorFailed(Exception exception)
  {
	base.OnSaveCloudAnchorFailed(exception);
  }

}
