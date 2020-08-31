/*
 * Copyright (c) 2014 LowLevelTech
 * Alex Dunn
 * llt@dunnalex.com
*/

using UnityEngine;

namespace PyroTechnix
{
    public sealed class ExplosionCameraShake : MonoBehaviour, IExplosionModule
    {
        public CameraShakeProperties cameraShakeProperties;

        void IExplosionModule.OnExplode(ExplosionSystem parent)
        {
            cameraShakeProperties.OriginatingPosition = transform.position;
            Camera.main.SendMessage("MakeMeShake", cameraShakeProperties, SendMessageOptions.DontRequireReceiver);
        }
    }
}
