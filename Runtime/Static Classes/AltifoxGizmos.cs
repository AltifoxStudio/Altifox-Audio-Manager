using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AltifoxStudio.AltifoxAudioManager
{
    namespace AltifoxGizmos
    {
        public class AltifoxCylinderGizmo
        {
            public static void Draw(Vector3 center, float radius, float height)
            {
                if (radius <= 0 || height <= 0) return;
                var halfHeight = height * 0.5f;
                DrawVolume(center - Vector3.up * halfHeight, center + Vector3.up * halfHeight, radius);
            }

            public static void DrawHalf(Vector3 bottomCenter, float radius, float height)
            {
                if (radius <= 0 || height <= 0) return;
                DrawVolume(bottomCenter, bottomCenter + Vector3.up * height, radius);
            }

            public static void DrawVolume(Vector3 bottom, Vector3 top, float radius)
            {
                Color originalColor = Handles.color;
                Handles.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.1f);
                Handles.DrawSolidDisc(top, Vector3.up, radius);
                Handles.DrawSolidDisc(bottom, Vector3.up, radius);
                Handles.color = originalColor;
                Handles.DrawWireDisc(top, Vector3.up, radius);
                Handles.DrawWireDisc(bottom, Vector3.up, radius);
                int lineCount = 12;
                for (int i = 0; i < lineCount; i++)
                {
                    float angle = i * (360f / lineCount);
                    var pointOffset = new Vector3(Mathf.Sin(Mathf.Deg2Rad * angle) * radius, 0, Mathf.Cos(Mathf.Deg2Rad * angle) * radius);
                    Handles.DrawLine(bottom + pointOffset, top + pointOffset, 1.5f);
                }

            }

        }

        public class AltifoxIcosphereGizmo
        {
            public static class IcosphereGenerator
            {
                private struct MeshData { public Vector3[] vertices; public List<int> triangles;
                }
                private static readonly Dictionary<int, MeshData> cache = new Dictionary<int, MeshData>();

                public static void DrawWireframe(float radius, bool hemisphere)
                {
                    if (radius <= 0) return;
                    int subdivisions = 2;
                    if (!cache.ContainsKey(subdivisions)) { cache[subdivisions] = Create(subdivisions); }
                    var mesh = cache[subdivisions];

                    for (int i = 0; i < mesh.triangles.Count; i += 3)
                    {
                        Vector3 v1 = mesh.vertices[mesh.triangles[i]] * radius;
                        Vector3 v2 = mesh.vertices[mesh.triangles[i + 1]] * radius;
                        Vector3 v3 = mesh.vertices[mesh.triangles[i + 2]] * radius;

                        if (hemisphere)
                        {
                            if (v1.y >= -0.001f && v2.y >= -0.001f) Handles.DrawLine(v1, v2);
                            if (v2.y >= -0.001f && v3.y >= -0.001f) Handles.DrawLine(v2, v3);
                            if (v3.y >= -0.001f && v1.y >= -0.001f) Handles.DrawLine(v3, v1);
                        }
                        else
                        {
                            Handles.DrawLine(v1, v2); Handles.DrawLine(v2, v3); Handles.DrawLine(v3, v1);
                        }
                    }
                    if (hemisphere) { Handles.DrawWireDisc(Vector3.zero, Vector3.up, radius); }
                }

                private static MeshData Create(int subdivisions)
                {
                    var vertices = new List<Vector3>();
                    var triangles = new List<int>();
                    float t = (1f + Mathf.Sqrt(5f)) / 2f;

                    vertices.Add(new Vector3(-1, t, 0).normalized); vertices.Add(new Vector3(1, t, 0).normalized);
                    vertices.Add(new Vector3(-1, -t, 0).normalized); vertices.Add(new Vector3(1, -t, 0).normalized);
                    vertices.Add(new Vector3(0, -1, t).normalized); vertices.Add(new Vector3(0, 1, t).normalized);
                    vertices.Add(new Vector3(0, -1, -t).normalized); vertices.Add(new Vector3(0, 1, -t).normalized);
                    vertices.Add(new Vector3(t, 0, -1).normalized); vertices.Add(new Vector3(t, 0, 1).normalized);
                    vertices.Add(new Vector3(-t, 0, -1).normalized); vertices.Add(new Vector3(-t, 0, 1).normalized);

                    triangles.AddRange(new int[] { 0, 11, 5, 0, 5, 1, 0, 1, 7, 0, 7, 10, 0, 10, 11, 1, 5, 9, 5, 11, 4, 11, 10, 2, 10, 7, 6, 7, 1, 8, 3, 9, 4, 3, 4, 2, 3, 2, 6, 3, 6, 8, 3, 8, 9, 4, 9, 5, 2, 4, 11, 6, 2, 10, 8, 6, 7, 9, 8, 1 });

                    var midCache = new Dictionary<long, int>();
                    for (int i = 0; i < subdivisions; i++)
                    {
                        var newTris = new List<int>();
                        for (int j = 0; j < triangles.Count; j += 3)
                        {
                            int v1 = triangles[j], v2 = triangles[j + 1], v3 = triangles[j + 2];
                            int a = GetMidPoint(v1, v2, ref vertices, midCache);
                            int b = GetMidPoint(v2, v3, ref vertices, midCache);
                            int c = GetMidPoint(v3, v1, ref vertices, midCache);
                            newTris.AddRange(new int[] { v1, a, c, v2, b, a, v3, c, b, a, b, c });
                        }
                        triangles = newTris;
                    }
                    return new MeshData { vertices = vertices.ToArray(), triangles = triangles };
                }

                private static int GetMidPoint(int p1, int p2, ref List<Vector3> vertices, Dictionary<long, int> cache)
                {
                    long smaller = Mathf.Min(p1, p2), greater = Mathf.Max(p1, p2);
                    long key = (smaller << 32) + greater;
                    if (cache.TryGetValue(key, out int ret)) return ret;
                    Vector3 middle = (vertices[p1] + vertices[p2]) * 0.5f;
                    int i = vertices.Count;
                    vertices.Add(middle.normalized);
                    cache.Add(key, i);
                    return i;
                }
            }
        }
    }
}