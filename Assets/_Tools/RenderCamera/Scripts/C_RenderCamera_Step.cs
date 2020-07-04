using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_RenderCamera_Step : MonoBehaviour
{
    public C_RenderCam_1P2 RenderCameraTarget;

	private void StepCamCapture()
	{
		if (RenderCameraTarget.StepRender)
		{
			Time.timeScale = 0;
			//RenderCameraTarget.CamCapture();
			RenderCameraTarget.StartCoroutine("DelayedCamCapture");
		}
		else
			Debug.LogWarning("C_RenderCamera_Step: Render camera 'StepRender' variable has not been set to true.");
	}
}
