/*
 * Copyright (c) 2014 LowLevelTech
 * Alex Dunn
 * llt@dunnalex.com
*/

using UnityEngine;

namespace PyroTechnix
{
    public interface IExplosionReactive
    {
        void OnExplosionHit(Vector3 explosionPosition);
        void OnExplosionForceHit(Vector4 explosionPosition_force);
        void OnCriticalExplosionHit(Vector3 explosionPosition);
    }
}
