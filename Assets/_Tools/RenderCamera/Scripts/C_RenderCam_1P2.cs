using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class C_RenderCam_1P2 : MonoBehaviour 
{
	public string FileName = "Capture";
	public string TargetFolder = "_Tools/RenderCamera/Captures";
	public RenderTexture TargetRenderTexture;
	public int RenderWidth = 1920;
	public int RenderHeight = 1080;

	public bool StepRender = false;
	[Range(0.01f, 0.5f)]
	public float StepTime = 0.01f;

	[SerializeField]
	private int _FileCounter = 0;

	// Alpha Shadow Parameters
	public bool EnableAlphaShadow = false;
	public GameObject TargetObjectParent;
	public GameObject[] TargetObjects;
	public Material ReplacementMat;
	public GameObject FloorObject;
	public float FloorHeight = 0.0f;
	public float ShadowIntensity = 0.6f;
	private GameObject _NewObjectHolder;
	private GameObject _CloneFloorObject;

	private void LateUpdate ()
	{
		if (Input.GetKeyDown(KeyCode.F9))
		{
			if (TargetRenderTexture != null)
				CamCapture ();
			else
				Debug.LogWarning ("C_RenderCam_1P2: Render Texture is set to null");
		}
	}

	private Material[] ReplaceAllMats (Renderer TargetRender)
	{
		Material[] NewMats = new Material[TargetRender.materials.Length];
		for (int i = 0; i < NewMats.Length; i++)
			NewMats [i] = ReplacementMat;
		return NewMats;
	}

	private void AlphaShadowReplace ()
	{
		_CloneFloorObject = Instantiate (FloorObject, new Vector3 (0.0f, FloorHeight, 0.0f), Quaternion.identity);
		_CloneFloorObject.GetComponent<Renderer> ().material.SetFloat ("_ShadowIntensity", ShadowIntensity);
		_NewObjectHolder = new GameObject ("C_NewObjectHolder");
		if (TargetObjectParent != null)
		{
			GameObject TargetObjectParentClone = Instantiate (TargetObjectParent, TargetObjectParent.transform.position, TargetObjectParent.transform.rotation);
			TargetObjectParentClone.transform.parent = _NewObjectHolder.transform;
			Renderer[] TempTarget = _NewObjectHolder.GetComponentsInChildren<Renderer> ();
			if (TempTarget.Length > 0)
			{
				for (int i = 0; i < TempTarget.Length; i++)
				{
					TempTarget [i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
					TempTarget [i].materials = ReplaceAllMats (TempTarget [i]);
				}
			}
		}
		else
		{
			if (TargetObjects.Length > 0)
			{
				for (int i = 0; i < TargetObjects.Length; i++)
				{
					GameObject TargetObjectClone = Instantiate (TargetObjects[i], TargetObjects[i].transform.position, TargetObjects[i].transform.rotation);
					TargetObjectClone.GetComponent<Renderer> ().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
					TargetObjectClone.GetComponent<Renderer> ().materials = ReplaceAllMats (TargetObjectClone.GetComponent<Renderer> ());
					TargetObjectClone.transform.parent = _NewObjectHolder.transform;
				}
			}
		}
	}

	public IEnumerator DelayedCamCapture ()
	{
		yield return new WaitForEndOfFrame();
		CamCapture();
	}

	public void CamCapture ()
	{
		Camera Cam = GetComponent<Camera>();
		TargetRenderTexture.Release();// Needed to clear render Texture.
		TargetRenderTexture.width = RenderWidth;
		TargetRenderTexture.height = RenderHeight;
		Cam.targetTexture = TargetRenderTexture;
		CameraClearFlags OriginFlag = Cam.clearFlags;

		if (EnableAlphaShadow)
		{
			Cam.clearFlags = CameraClearFlags.Depth;
			AlphaShadowReplace ();
		}

		RenderTexture currentRT = RenderTexture.active;
		RenderTexture.active = Cam.targetTexture;

		Cam.Render ();

		Texture2D Image = new Texture2D (Cam.targetTexture.width, Cam.targetTexture.height);
		Image.ReadPixels (new Rect (0, 0, Cam.targetTexture.width, Cam.targetTexture.height), 0, 0);
		Image.Apply ();
		RenderTexture.active = currentRT;

		var Bytes = Image.EncodeToPNG ();
		Destroy (Image);

		_FileCounter++;
		if (TargetFolder != null)
			File.WriteAllBytes (Application.dataPath + "/" + TargetFolder + "/" + FileName + "_" + _FileCounter.ToString("D2") + ".png", Bytes);
		else
			File.WriteAllBytes (Application.dataPath + "/" + FileName + "_" + _FileCounter.ToString("D2") + ".png", Bytes);
		Debug.Log ("C_RenderCam_1P2: " + FileName + "_" + _FileCounter.ToString("D2") + " Capture Successful.");

		Cam.targetTexture = null;
		if (EnableAlphaShadow)
		{
			Cam.clearFlags = OriginFlag;
			DestroyImmediate (_NewObjectHolder);
			DestroyImmediate (_CloneFloorObject);
		}
		if (StepRender)
			Time.timeScale = StepTime;
	}
}