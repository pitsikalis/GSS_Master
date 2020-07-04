using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Editor_CameraPositioner : Editor
{
//#if UNITY_EDITOR
    // Start is called before the first frame update
    void Start()
    {
       //var brain = GameObject.FindWithTag("Main Camera").GetComponent<Camera>();
       // var brain = GetComponent<Cinemachine.CinemachineBrain>();

      // if (brain)
      //  {
      //      brain.enabled = true;
       // }
        
    }

    [MenuItem("Tools/AllignCameraWithView %]")]
    public static void AllignCameraWithView()
    {
        var cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Transform>();
       // var brain = cam.GetComponent<CCinemachine.CinemachineBrain>();
       // if (brain)
       // {
       //     brain.enabled - false;
       // }

        cam.position = SceneView.lastActiveSceneView.pivot - SceneView.lastActiveSceneView.camera.transform.forward * SceneView.lastActiveSceneView.cameraDistance;
        cam.rotation = SceneView.lastActiveSceneView.rotation;
    }

//#endif
}
