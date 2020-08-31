/*
 * Copyright (c) 2014 LowLevelTech
 * Alex Dunn
 * llt@dunnalex.com
*/

using UnityEngine;
using System.Collections;

namespace PyroTechnix
{
    [System.Serializable]
    public class CameraShakeProperties
    {
        public AnimationCurve envelope;
        public float duration;
        public float amplitude;
        public float frequency;

        private Vector3 originatingPosition;
        public Vector3 OriginatingPosition
        {
            set { originatingPosition = value; }
            get { return originatingPosition; }
        }
    }

    public class CameraShake : MonoBehaviour
    {
        public Vector3 shakeRange = new Vector3(5, 2, 1);
        public float maxAffectedDistance = 50;

        private Vector3 tMovement = Vector3.zero;

        public void MakeMeShake(CameraShakeProperties props)
        {
            StartCoroutine("Shake", props);
        }

        public void DirectShake(float amplitude)
        {
            transform.eulerAngles -= tMovement;

            tMovement = Vector3.Scale(SmoothRandom.GetVector3(5 * Time.time), shakeRange) * amplitude;

            transform.eulerAngles += tMovement;
        }

        public void DirectShake(float frequency, float amplitude)
        {
            transform.eulerAngles -= tMovement;

            tMovement = Vector3.Scale(SmoothRandom.GetVector3(frequency * Time.time), shakeRange) * amplitude;

            transform.eulerAngles += tMovement;
        }

        public void StopShake()
        {
            StopCoroutine("Shake");
        }

        /// <summary>
        /// This is the internal shake function.  Called symbollically using the MakeMeShake function, this is required when wanting to stop a coroutine (using StopShake).
        /// </summary>
        /// <param name="props">Shake properties for the specific shake.</param>
        /// <returns>Coroutine.</returns>
        private IEnumerator Shake(CameraShakeProperties props)
        {
            float timer = 0;

            while (timer <= props.duration)
            {
                float normalizedDistanceToShakeOrigin = 1 - (Mathf.Clamp(Vector3.Distance(transform.position, props.OriginatingPosition), 0, maxAffectedDistance) / maxAffectedDistance);
                float amplitude = props.envelope.Evaluate(timer / props.duration) * props.amplitude * normalizedDistanceToShakeOrigin;

                DirectShake(props.frequency, amplitude);

                timer += Time.deltaTime;
                yield return null;
            }
        }
    }
}