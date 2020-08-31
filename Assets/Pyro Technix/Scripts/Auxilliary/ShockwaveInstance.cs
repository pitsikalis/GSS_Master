/*
 * Copyright (c) 2014 LowLevelTech
 * Alex Dunn
 * llt@dunnalex.com
*/

using UnityEngine;
using System.Collections;

namespace PyroTechnix
{
    public class ShockwaveInstance : MonoBehaviour
    {
        private void OnWillRenderObject()
        {
            GetComponent<Renderer>().sharedMaterial.SetMatrix("_Camera2World", Camera.current.transform.localToWorldMatrix);
        }
    }
}