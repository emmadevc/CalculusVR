using Assets.Scripts.EMath.Geometry;
using Assets.Scripts.Plotter.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Plotter.Cone
{
    class ConePlotter
    {
        private static readonly List<ConeAction> actions = new List<ConeAction>();
        private static readonly Pool<Cone> pool = new Pool<Cone>("cone");

        private const int NB_SIDES = 18;
        private const int NB_VERTICES_CAP = NB_SIDES + 1;

        public static void Plot(GameObject parent, Point start, Point end, float baseScale, float pointScale,
            Material material)
        {
            AddAction(pool.Get(), parent, start, end, baseScale, pointScale, material);
        }

        private static void AddAction(Cone cone, GameObject parent, Point start, Point end, float baseScale,
            float pointScale, Material material)
        {
            actions.Add(new ConeAction(cone, parent, start, end, baseScale, pointScale, material));
        }

        public static void PlotActions()
        {
            foreach (ConeAction action in actions)
            {
                Plot(action.cone, action.parent, action.start, action.end, action.baseScale,
                    action.pointScale, action.material);
            }

            actions.Clear();
        }

        public static void ClearActions()
        {
            actions.Clear();
        }

        public static void Clear()
        {
            pool.Clear();
        }

        private static void Plot(Cone cone, GameObject parent, Point start, Point end, float baseScale,
            float pointScale, Material material)
        {
            cone.mesh.Clear();

            cone.gameObject.transform.parent = parent.transform;
            cone.gameObject.transform.position = parent.transform.position;

            Vector3 pos = PlotUtils.ToVector3(start);
            Vector3 finalPos = PlotUtils.ToVector3(end);
            Vector3[] vertices = Vertices(Vector3.Distance(pos, finalPos), baseScale, pointScale);

            cone.mesh.vertices = vertices;
            cone.mesh.normals = Normales(vertices);
            cone.mesh.uv = Uvs(vertices);
            cone.mesh.triangles = Triangles();

            cone.mesh.RecalculateBounds();
            cone.mesh.Optimize();

            cone.meshRenderer.material = material;

            Locate(cone.gameObject, pos, finalPos);

            cone.gameObject.SetActive(true);
        }

        public static void Locate(GameObject gameObject, Vector3 from, Vector3 to)
        {
            gameObject.transform.localPosition = from;

            gameObject.transform.LookAt(to);
            gameObject.transform.rotation *= Quaternion.Euler(90, 0, 0);
        }

        private static Vector3[] Vertices(float height, float baseScale, float pointScale)
        {
            float bottomRadius = height * baseScale;
            float topRadius = bottomRadius * pointScale;
            int nbHeightSeg = 1;

            // bottom + top + sides
            Vector3[] vertices = new Vector3[NB_VERTICES_CAP + NB_VERTICES_CAP + NB_SIDES * nbHeightSeg * 2 + 2];
            int vert = 0;
            float _2pi = Mathf.PI * 2f;

            // Bottom cap
            vertices[vert++] = new Vector3(0f, 0f, 0f);
            while (vert <= NB_SIDES)
            {
                float rad = (float)vert / NB_SIDES * _2pi;
                vertices[vert] = new Vector3(Mathf.Cos(rad) * bottomRadius, 0f, Mathf.Sin(rad) * bottomRadius);
                vert++;
            }

            // Top cap
            vertices[vert++] = new Vector3(0f, height, 0f);
            while (vert <= NB_SIDES * 2 + 1)
            {
                float rad = (float)(vert - NB_SIDES - 1) / NB_SIDES * _2pi;
                vertices[vert] = new Vector3(Mathf.Cos(rad) * topRadius, height, Mathf.Sin(rad) * topRadius);
                vert++;
            }

            // Sides
            int v = 0;
            while (vert <= vertices.Length - 4)
            {
                float rad = (float)v / NB_SIDES * _2pi;
                vertices[vert] = new Vector3(Mathf.Cos(rad) * topRadius, height, Mathf.Sin(rad) * topRadius);
                vertices[vert + 1] = new Vector3(Mathf.Cos(rad) * bottomRadius, 0, Mathf.Sin(rad) * bottomRadius);
                vert += 2;
                v++;
            }

            vertices[vert] = vertices[NB_SIDES * 2 + 2];
            vertices[vert + 1] = vertices[NB_SIDES * 2 + 3];

            return vertices;
        }

        private static Vector3[] Normales(Vector3[] vertices)
        {
            // bottom + top + sides
            Vector3[] normales = new Vector3[vertices.Length];

            int vert = 0;
            float _2pi = Mathf.PI * 2f;

            // Bottom cap
            while (vert <= NB_SIDES)
            {
                normales[vert++] = Vector3.down;
            }

            // Top cap
            while (vert <= NB_SIDES * 2 + 1)
            {
                normales[vert++] = Vector3.up;
            }

            // Sides
            int v = 0;
            while (vert <= vertices.Length - 4)
            {
                float rad = (float)v / NB_SIDES * _2pi;
                float cos = Mathf.Cos(rad);
                float sin = Mathf.Sin(rad);

                normales[vert] = new Vector3(cos, 0f, sin);
                normales[vert + 1] = normales[vert];

                vert += 2;
                v++;
            }

            normales[vert] = normales[NB_SIDES * 2 + 2];
            normales[vert + 1] = normales[NB_SIDES * 2 + 3];

            return normales;
        }

        private static Vector2[] Uvs(Vector3[] vertices)
        {
            Vector2[] uvs = new Vector2[vertices.Length];
            float _2pi = Mathf.PI * 2f;

            // Bottom cap
            int u = 0;
            uvs[u++] = new Vector2(0.5f, 0.5f);
            while (u <= NB_SIDES)
            {
                float rad = (float)u / NB_SIDES * _2pi;
                uvs[u] = new Vector2(Mathf.Cos(rad) * .5f + .5f, Mathf.Sin(rad) * .5f + .5f);
                u++;
            }

            // Top cap
            uvs[u++] = new Vector2(0.5f, 0.5f);
            while (u <= NB_SIDES * 2 + 1)
            {
                float rad = (float)u / NB_SIDES * _2pi;
                uvs[u] = new Vector2(Mathf.Cos(rad) * .5f + .5f, Mathf.Sin(rad) * .5f + .5f);
                u++;
            }

            // Sides
            int u_sides = 0;
            while (u <= uvs.Length - 4)
            {
                float t = (float)u_sides / NB_SIDES;
                uvs[u] = new Vector3(t, 1f);
                uvs[u + 1] = new Vector3(t, 0f);
                u += 2;
                u_sides++;
            }
            uvs[u] = new Vector2(1f, 1f);
            uvs[u + 1] = new Vector2(1f, 0f);

            return uvs;
        }
        private static int[] Triangles()
        {
            int nbTriangles = NB_SIDES + NB_SIDES + NB_SIDES * 2;
            int[] triangles = new int[nbTriangles * 3 + 3];

            // Bottom cap
            int tri = 0;
            int i = 0;
            while (tri < NB_SIDES - 1)
            {
                triangles[i] = 0;
                triangles[i + 1] = tri + 1;
                triangles[i + 2] = tri + 2;
                tri++;
                i += 3;
            }
            triangles[i] = 0;
            triangles[i + 1] = tri + 1;
            triangles[i + 2] = 1;
            tri++;
            i += 3;

            // Top cap
            //tri++;
            while (tri < NB_SIDES * 2)
            {
                triangles[i] = tri + 2;
                triangles[i + 1] = tri + 1;
                triangles[i + 2] = NB_VERTICES_CAP;
                tri++;
                i += 3;
            }

            triangles[i] = NB_VERTICES_CAP + 1;
            triangles[i + 1] = tri + 1;
            triangles[i + 2] = NB_VERTICES_CAP;
            tri++;
            i += 3;
            tri++;

            // Sides
            while (tri <= nbTriangles)
            {
                triangles[i] = tri + 2;
                triangles[i + 1] = tri + 1;
                triangles[i + 2] = tri + 0;
                tri++;
                i += 3;

                triangles[i] = tri + 1;
                triangles[i + 1] = tri + 2;
                triangles[i + 2] = tri + 0;
                tri++;
                i += 3;
            }


            return triangles;
        }
    }
}