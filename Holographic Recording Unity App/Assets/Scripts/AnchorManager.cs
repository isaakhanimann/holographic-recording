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

  public AnchorStoreManager anchorStore;
  private string currentAnchorId = "";
  private Queue<Action> dispatchQueue;

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
	anchorStore = new AnchorStoreManager();
	dispatchQueue = new Queue<Action>();

	//UnityDispatcher.InvokeOnAppThread(() => { FindAnchors(); });

	debugText.text += "AnchorManager.Start() called \n";
	base.Start();
	base.SpawnOrMoveCurrentAnchoredObject(new Vector3(1, 0, 0.5f), new Quaternion(0, 0, 0, 1));
  }

  private void Update()
  {
	lock (dispatchQueue)
	{
	  if (dispatchQueue.Count > 0)
	  {
		dispatchQueue.Dequeue();
	  }
	}
  }

  // Step 1
  public void OnInitSessionClicked()
  {
	InitAnchorSession();
  }

  // Step 2
  public void OnSaveAnchorClicked()
  {
	debugText.text += "Button Clicked, calling SaveAnchorForObjectAsync() \n";
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

  // Trying to retrieve anchors from stored file, but somehow after CreateWatcher() is called, app crashes or doesnt trigger anchor_located
  public async void FindAnchorsTask()
  {
	ResetAnchorIdsToLocate();

	debugText.text += "FindAnchorsTask() called\n";

	// Fetch the IDs of anchors stored in the text file
	List<string> anchorKeysToFind = await anchorStore.RetrieveAnchorKeys();

	if (anchorKeysToFind.Count > 0)
	{
	  debugText.text += "Found anchor keys\n";

	  // Set the IDs to locate in the AnchorLocateCriteria object
	  SetAnchorIdsToLocate(anchorKeysToFind);

	  debugText.text += "anchorLocateCriteria.Identifiers: ";
	  foreach (string id in anchorLocateCriteria.Identifiers)
	  {
		debugText.text += id + ", ";
	  }
	  debugText.text += "\n";

	  debugText.text += "Creating watcher \n";

	  // Create the watcher for those anchor GUIDs
	  // SetNearDevice(3.0f, 10);
	  CloudSpatialAnchorWatcher watcher = CreateWatcher();
	  debugText.text += "Watcher ID: " + watcher.Identifier + "\n";
	}
	else
	{
	  debugText.text += "Can't find anchor keys\n";
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
	debugText.text += "InitAnchorSession() called \n";

	base.Start();

	currentCloudAnchor = null;

	if (CloudManager.Session == null)
	{
	  debugText.text += "Creating session \n";
	  await CloudManager.CreateSessionAsync();
	}


	debugText.text += "Start session \n";

	if (!CloudManager.IsSessionStarted)
	{
	  await CloudManager.StartSessionAsync();
	}
  }


  protected override void OnCloudLocateAnchorsCompleted(LocateAnchorsCompletedEventArgs args)
  {
	Task.Run(async () => {
	  await Task.Delay(2500);
	});
	args.Watcher.Stop();
  }

  protected override void OnCloudAnchorLocated(AnchorLocatedEventArgs args)
  {
	//debugText.text += "AnchorManager.OnCloudAnchorLocated() called\n";

	//debugText.text += "args.Status: " + args.Status + "\n";
	base.OnCloudAnchorLocated(args);

	if (args.Status == LocateAnchorStatus.Located)
	{
	  currentCloudAnchor = args.Anchor;

	  //debugText.text += "Spawning prefab again \n";

	  UnityDispatcher.InvokeOnAppThread(() =>
	  {
		QueueOnUpdate(() =>
		{
		  Pose anchorPose = currentCloudAnchor.GetPose();
		  Instantiate(original: anchoredObjectPrefab, position: anchorPose.position, rotation: anchorPose.rotation);

		  Task.Run(async () => {
			await Task.Delay(1000);
		  });
		});
	  });

	  /*UnityDispatcher.InvokeOnAppThread(() =>
	  {      
		Pose anchorPose = Pose.identity;

		SpawnOrMoveCurrentAnchoredObject(anchorPose.position, anchorPose.rotation);

		UnityEngine.XR.WSA.WorldAnchor wa = spawnedObject.AddComponent<UnityEngine.XR.WSA.WorldAnchor>();
		wa.SetNativeSpatialAnchorPtr(currentCloudAnchor.LocalAnchor);
		debugText.text += "Spawned and moved object \n";
	  });*/

	}
  }

  private void QueueOnUpdate(Action p)
  {
	lock (dispatchQueue)
	{
	  dispatchQueue.Enqueue(p);
	}
  }

  public void AddCloudNativeAnchorToObject(ref GameObject go)
  {
	go.AddComponent<CloudNativeAnchor>();
  }

  public async void SaveObjectAnchorToCloudAndStopSession(GameObject go)
  {
	await SaveObjectAnchorToCloudAsyncTaskAndStopSession(go);
  }

  // Copy pasted this method from DemoScriptBase to allow specifying gameobject to get cloudnativeanchor from that will be saved.
  public async Task SaveObjectAnchorToCloudAsyncTaskAndStopSession(GameObject go)
  {
	debugText.text += "SaveObjectAnchorToCloudAsyncTaskAndStopSession() called \n";

	// Get the cloud-native anchor behavior
	CloudNativeAnchor cna = go.GetComponent<CloudNativeAnchor>();

	debugText.text += "get component from spawned object...\n";


	// If the cloud portion of the anchor hasn't been created yet, create it
	if (cna.CloudAnchor == null)
	{
	  debugText.text += "Cloud anchor is null\n";

	  cna.NativeToCloud();
	}
	else
	{
	  debugText.text += "Cloud anchor is not null\n";
	}

	debugText.text += "checked anchor is noull or not \n";


	// Get the cloud portion of the anchor
	CloudSpatialAnchor cloudAnchor = cna.CloudAnchor;
	debugText.text += "get cloud anchor\n";

	// In this sample app we delete the cloud anchor explicitly, but here we show how to set an anchor to expire automatically
	//cloudAnchor.Expiration = DateTimeOffset.Now.AddDays(7);

	while (!CloudManager.IsReadyForCreate)
	{
	  debugText.text += "CloudManager not is ready for create\n";

	  await Task.Delay(330);
	  float createProgress = CloudManager.SessionStatus.RecommendedForCreateProgress;
	  debugText.text += createProgress + "\n";
	  debugText.text += $"Move your device to capture more environment data: {createProgress:0%}\n";

	}

	bool success = false;

	debugText.text += "Saving...\n";

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

  protected override async Task OnSaveCloudAnchorSuccessfulAsync()
  {
	await base.OnSaveCloudAnchorSuccessfulAsync();

	// For now store Id locally (needs to be retrieved from ASA for cross device persitence)
	currentAnchorId = currentCloudAnchor.Identifier;
	await anchorStore.StoreAnchorKey(currentAnchorId);

	debugText.text += "currentAnchorId: " + currentAnchorId + "\n";

	debugText.text += "Save cloud anchor successful \n";
	CloudManager.StopSession();
  }

  protected override void OnSaveCloudAnchorFailed(Exception exception)
  {
	base.OnSaveCloudAnchorFailed(exception);

	debugText.text += "Save cloud anchor failed \n";
  }

}
