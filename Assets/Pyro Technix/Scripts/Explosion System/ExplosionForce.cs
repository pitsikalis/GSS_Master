/*
 * Copyright (c) 2014 LowLevelTech
 * Alex Dunn
 * llt@dunnalex.com
*/

using System;
using UnityEngine;

namespace PyroTechnix
{
    public sealed class ExplosionForce : MonoBehaviour, IExplosionModule
    {
        public enum OcclusionType
        {
            None,
            Basic
        }

        public  OcclusionType occlusionType;
        public LayerMask affectedLayers;
        public float criticalRadius = 0.5f;
        public float outerRadius = 3;
        public float power = 10000;

        // Must be at least 1 
        private const uint NumRaysToCast = 5;

        private Collider[] offendingColliders;
        private delegate float ExplosionOcclusion(Vector3 source, Collider checkPosition, float power);
        private Func<Vector3, float, Collider, float> OcclusionAlgorithm;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, outerRadius);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, criticalRadius);
        }

        #region IExplosionModule
        void IExplosionModule.OnExplode(ExplosionSystem parent)
        {
            UpdateOcclusionType();

            var explosionPosition = transform.position;
            offendingColliders = Physics.OverlapSphere(explosionPosition, outerRadius, affectedLayers);
            Collider[] criticalColliders = Physics.OverlapSphere(explosionPosition, criticalRadius);

            foreach (Collider hit in criticalColliders)
            {
                if (!hit) continue;

                hit.SendMessage("OnCriticalExplosionHit", explosionPosition, SendMessageOptions.DontRequireReceiver);
            }

            foreach (Collider hit in offendingColliders)
            {
                if (!hit) continue;

                hit.SendMessage("OnExplosionHit", explosionPosition, SendMessageOptions.DontRequireReceiver);

                if (hit.GetComponent<Rigidbody>())
                {
                    float hitPower = OcclusionAlgorithm(explosionPosition, power, hit);
                    //int numRays = 1;

                    //// Get some points on a circle in the plane tangential to the line between explosion and target (!)
                    //Vector3 forwardDirection = hit.transform.position - explosionPosition;
                    //for (int i = 0; i < NumRaysToCast - 1; i++)
                    //{
                    //    Vector3 relativePosition = Vector3.one;
                    //    Vector3.OrthoNormalize(ref forwardDirection, ref relativePosition);
                    //    relativePosition = Quaternion.AngleAxis(360f * i / NumRaysToCast, forwardDirection) * relativePosition;
                    //    relativePosition *= criticalRadius;

                    //    // If the line from us to the point is clear (otherwise we can get around obstacles by going through the ground etc.)
                    //    if (!Physics.Linecast(explosionPosition, explosionPosition + relativePosition, 1 << LayerMask.NameToLayer("Environment/World")))
                    //    {
                    //        hitPower += OcclusionAlgorithm(explosionPosition + relativePosition, power, hit);
                    //        numRays++;
                    //        Debug.DrawLine(explosionPosition + relativePosition, hit.transform.position, new Color(.3f, .05f, .45f), 0.4f);
                    //    }
                    //}

                    //hitPower = hitPower / numRays;

                    //if (hitPower <= 0.1f) continue;

                    Vector4 explosionInfo = new Vector4(explosionPosition.x, explosionPosition.y, explosionPosition.z, hitPower);
                    SendMessage("OnExplosionForceHit", explosionInfo, SendMessageOptions.DontRequireReceiver);
                    hit.GetComponent<Rigidbody>().AddExplosionForce(hitPower, explosionPosition, outerRadius, 0);
                }
            }
        }
        #endregion

        #region Occlusion
        private void UpdateOcclusionType()
        {
            switch (occlusionType)
            {
                case OcclusionType.None:
                    OcclusionAlgorithm = NoOcclusion;
                    break;

                case OcclusionType.Basic:
                    OcclusionAlgorithm = BasicOcclusion;
                    break;
            }
        }

        private static float NoOcclusion(Vector3 source, float hitPower, Collider target)
        {
            return hitPower;
        }

        private static float BasicOcclusion(Vector3 source, float hitPower, Collider target)
        {
            RaycastHit raycastHit;
            bool hit = Physics.Linecast(source, target.transform.position, out raycastHit, 1 << LayerMask.NameToLayer("Environment/World"));

            if (hit)
            {
                if (raycastHit.collider != target)
                {
                    return 0f;
                }
            }
            return hitPower;
        }

        #endregion
    }
}