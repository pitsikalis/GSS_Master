/*
 * Copyright (c) 2014 LowLevelTech
 * Alex Dunn
 * llt@dunnalex.com
*/

using UnityEngine;

namespace PyroTechnix
{
    [RequireComponent(typeof(AudioSource))]
    public class ExplosionSound : MonoBehaviour, IExplosionModule
    {
        public int soundPriority = 1;
        public float volume = 1;

        public AudioClip[] nearSounds;
        public AudioClip[] mediumSounds;
        public AudioClip[] farSounds;

        public float nearSoundDistanceThreshold = 10;
        public float mediumSoundDistanceThreshold = 30;
        public float farSoundDistanceThreshold = 60;

        #region IExplosionModule
        void IExplosionModule.OnExplode(ExplosionSystem parent)
        {
            float sqrDistanceToListener = (transform.position - Camera.main.transform.position).sqrMagnitude;

            if (sqrDistanceToListener < nearSoundDistanceThreshold * nearSoundDistanceThreshold)
            {
                if (nearSounds != null && nearSounds.Length > 0)
                {
                    GetComponent<AudioSource>().PlayOneShot(Helpers.RandomFromArray(nearSounds), volume);
                }
            }
            else if (sqrDistanceToListener < mediumSoundDistanceThreshold * mediumSoundDistanceThreshold)
            {
                if (mediumSounds != null && mediumSounds.Length > 0)
                {
                    GetComponent<AudioSource>().PlayOneShot(Helpers.RandomFromArray(mediumSounds), volume);
                }
            }
            else if (sqrDistanceToListener < farSoundDistanceThreshold * farSoundDistanceThreshold)
            {
                if (farSounds != null && farSounds.Length > 0)
                {
                    GetComponent<AudioSource>().PlayOneShot(Helpers.RandomFromArray(farSounds), volume);
                }
            }
        }
        #endregion
    }
}