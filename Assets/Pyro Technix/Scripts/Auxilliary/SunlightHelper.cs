using UnityEngine;
using System.Collections;

namespace PyroTechnix
{
    [RequireComponent(typeof(Light))]
    internal class SunlightHelper : MonoBehaviour 
    {
        private static Light instance;
        public static Light Instance
        {
            get
            {
                if (instance == null)
                {
                    SunlightHelper helper = FindObjectOfType(typeof(SunlightHelper)) as SunlightHelper;

                    if (helper != null)
                    {
                        instance = helper.GetComponent<Light>();
                    }
                }

                return instance;
            }
        }
    }
}