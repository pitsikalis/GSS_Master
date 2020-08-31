/*
 * Copyright (c) 2013 LowLevelTech
 * Alex Dunn
 * llt@dunnalex.com
*/

using System;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace PyroTechnix
{
    /// <summary>
    /// Simple helper object that is used to wrap up profile marker code using the 'using' keyword.
    /// 
    /// It's just to create neater, easier to read, code.
    /// </summary>
    public class ProfilerMarker : IDisposable
    {
        public ProfilerMarker(string name)
        {
            UnityEngine.Profiling.Profiler.BeginSample(name);
        }

        public void Dispose()
        {
            UnityEngine.Profiling.Profiler.EndSample();
        }
    }

    /// <summary>
    /// Mixed static helper class, includes all the custom helper functions used in Fluidity.
    /// </summary>
    public static class Helpers
    {
        /// <summary>
        /// Generic wrapper for Unitys find object of type function (<see cref="Object.FindObjectsOfType(Type)"/>).
        /// </summary>
        /// <typeparam name="T">Type of object to find, must inherit from UnityEngine.Object (<see cref="UnityEngine.Object"/>).</typeparam>
        /// <returns>An array containing all the objects found matching the type specified.</returns>
        public static T[] FindObjectsOfType<T>()  where T : Object
        {
            T[] objects = Object.FindObjectsOfType(typeof (T)) as T[];
            return objects;
        }

        public static T[] GetInterfaces<T>(this GameObject gObj)
        {
            if (!typeof(T).IsInterface) throw new SystemException("Specified type is not an interface!");

            var mObjs = gObj.GetComponents<MonoBehaviour>();

            return (from a in mObjs where a.GetType().GetInterfaces().Any(k => k == typeof(T)) select (T)(object)a).ToArray();
        }

        public static T RandomFromArray<T>(T[] array)
        {
            return array[Random.Range(0, array.Length)];
        }

        /// <summary>
        /// Extension: Sets the alpha of a colour and return a copy it.
        /// 
        /// Using this we can tidy up some inline code.  The original colour does not change, it is a struct.
        /// </summary>
        /// <param name="c">Input colour.</param>
        /// <param name="a">Desired alpha.</param>
        /// <returns>Copy of input colour with modified alpha value.</returns>
        public static Color SetAlpha(this Color c, float a)
        {
            return new Color(c.r, c.g, c.b, a);
        }

        /// <summary>
        /// Gizmo helper.
        /// 
        /// Draws a 3D arrow at a given position in a given direction.
        /// </summary>
        /// <param name="pos">Position to draw arrow in world space.</param>
        /// <param name="direction">Direction to orient arrow in world space.</param>
        public static void DrawArrow(Vector3 pos, Vector3 direction)
        {
            if (direction.sqrMagnitude < 0.001f) return;

            const float headLength = 0.5f;
            const float tipLength = headLength * 0.2f;
            const float bodyLength = 2;
            const int step = 30;

            Vector3 directionNormalized = direction.normalized;
            Quaternion directionQuaternion = Quaternion.LookRotation(directionNormalized);
            Vector3 coneScale = new Vector3(0.2f, 0.2F, 1);

            Gizmos.DrawRay(pos, directionNormalized * bodyLength);

            for (int i = 0; i < 360 / step; i++)
            {
                Vector3 cone = directionQuaternion * Quaternion.Euler(0, 180, i * step) * coneScale;
                Gizmos.color = Color.white;
                Gizmos.DrawRay(pos + directionNormalized * bodyLength, cone * headLength);
                Gizmos.color = Color.blue;
                Gizmos.DrawRay(pos + directionNormalized * bodyLength, cone * tipLength);
            }

            Gizmos.color = Color.white;
        }

        public static Mesh CreateRing(int verticesPerEdge = 10)
        {
            int numVertices = verticesPerEdge * 2;

            Vector3[] vertices = new Vector3[numVertices];
            Vector2[] texcoord = new Vector2[numVertices];
            float innerRadius = 0.5f;
            float outterRadius = 1.0f;

            float step = 2 * Mathf.PI / verticesPerEdge;

            int index = 0;
            for (float pi = 0.0f; pi < 2*Mathf.PI; pi += step)
            {
                vertices[index] = new Vector3(Mathf.Sin(pi), Mathf.Cos(pi), 0) * innerRadius;
                texcoord[index] = new Vector2(pi / (2 * Mathf.PI), 0);
                ++index;

                vertices[index] = new Vector3(Mathf.Sin(pi), Mathf.Cos(pi), 0) * outterRadius;
                texcoord[index] = new Vector2(pi / (2 * Mathf.PI), 1);
                ++index;
            }

            index = 0;

            int numTriangles = verticesPerEdge * 2;
            int numIndices = numTriangles * 3;

            int[] triangles = new int[numIndices];

            for (int i = 0; i < verticesPerEdge; i++)
            {
                triangles[index++] = ((i * 2) + 0) % numVertices;
                triangles[index++] = ((i * 2) + 1) % numVertices;
                triangles[index++] = ((i * 2) + 3) % numVertices;
                triangles[index++] = ((i * 2) + 0) % numVertices;
                triangles[index++] = ((i * 2) + 3) % numVertices;
                triangles[index++] = ((i * 2) + 2) % numVertices;
            }

            Mesh mesh = new Mesh();
            mesh.Clear();

            mesh.name = "PyroTechnix Ring";
            mesh.vertices = vertices;
            mesh.uv = texcoord;
            mesh.SetIndices(triangles, MeshTopology.Triangles, 0);
            mesh.RecalculateBounds();
            mesh.bounds = new UnityEngine.Bounds(Vector3.zero, Vector3.one);
            return mesh;
        }

        public static Mesh CreatePlane(int verticesPerEdge = 10)
        {
            int edgeCount = verticesPerEdge + 1;

            int numVertices = edgeCount * edgeCount;

            int index = 0;
            float scaleX = 1.0f / verticesPerEdge;
            float scaleY = 1.0f / verticesPerEdge;

            Vector3[] vertices = new Vector3[numVertices];

            for (float y = 0.0f; y < edgeCount; y++)
            {
                for (float x = 0.0f; x < edgeCount; x++)
                {
                    vertices[index++] = new Vector3(x * scaleX - 0.5f, y * scaleY - 0.5f, 0.0f) * 2;
                }
            }

            index = 0;

            int numTriangles = verticesPerEdge * verticesPerEdge * 6;
            int[] triangles = new int[numTriangles * 6];

            for (int y = 0; y < verticesPerEdge; y++)
            {
                for (int x = 0; x < verticesPerEdge; x++)
                {
                    triangles[index++] = (y * edgeCount) + x;
                    triangles[index++] = ((y + 1) * edgeCount) + x;
                    triangles[index++] = (y * edgeCount) + x + 1;
                    triangles[index++] = ((y + 1) * edgeCount) + x;
                    triangles[index++] = ((y + 1) * edgeCount) + x + 1;
                    triangles[index++] = (y * edgeCount) + x + 1;
                }
            }

            Mesh mesh = new Mesh();
            mesh.Clear();

            mesh.name = "PyroTechnix Board";
            mesh.vertices = vertices;
            mesh.SetIndices(triangles, MeshTopology.Triangles, 0);
            mesh.RecalculateBounds();
            mesh.bounds = new UnityEngine.Bounds(Vector3.zero, Vector3.one);

            return mesh;
        }
    }
}