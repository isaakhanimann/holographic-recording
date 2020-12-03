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

  async void Start()
  {
	debugText.text += "AnchorManager.Start() called \n";
	base.Start();
	SpawnNewAnchoredObject(new Vector3(0.1f, 0, 0), new Quaternion(0, 0, 0, 1));
	await SaveCurrentObjectAnchorToCloudAsync();
  }

  /*    public async void OnButtonClicked()
	  {
		  debugText.text += "Button Clicked, calling save current object anchor \n";
		  await SaveCurrentObjectAnchorToCloudAsync();

		  while (!saved)
		  {
			  await Task.Yield();
		  }
	  }
  */

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
