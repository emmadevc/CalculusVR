using Assets.Scripts.EMath.Geometry;
using Assets.Scripts.Plotter.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Plotter.Plane
{
    class PlanePlotter
    {
        private static readonly List<PlaneAction> actions = new List<PlaneAction>();
        private static readonly Pool<Plane> pool = new Pool<Plane>("plane");

        public static void Plot(GameObject parent, List<Point> points, Material material,
            PlaneView view)
        {
            switch (view)
            {
                case PlaneView.Top:
                    AddAction(parent, points, material, false);
                    break;
                case PlaneView.Bottom:
                    AddAction(parent, points, material, true);
                    break;
                case PlaneView.Both:
                    AddAction(parent, points, material, false);
                    AddAction(parent, points, material, true);
                    break;
            }
        }

        private static void AddAction(GameObject parent, List<Point> points, Material material, bool reverse)
        {
            actions.Add(new PlaneAction(parent, points.Select(p => p).ToList(), material, reverse));
        }

        public static void PlotActions()
        {
            foreach (PlaneAction action in actions)
            {
                Plot(action.parent, action.points, action.material, action.reverse);
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

        private static void Plot(GameObject parent, List<Point> points, Material material, bool reverse)
        {
            Vector3[] vertices = points.Select(p => PlotUtils.ToVector3(p)).ToArray();
            int[] triangles = Triangles(vertices, reverse);

            Plot(pool.Get(), parent, vertices, triangles, material);
        }

        private static void Plot(Plane plane, GameObject parent, Vector3[] vertices, int[] triangles, Material material)
        {
            plane.mesh.Clear();

            plane.gameObject.transform.parent = parent.transform;

            plane.mesh.vertices = vertices;
            plane.mesh.triangles = triangles;

            plane.mesh.RecalculateNormals();
            plane.mesh.RecalculateBounds();
            plane.mesh.Optimize();

            plane.meshRenderer.sharedMaterial = material;

            plane.gameObject.SetActive(true);
        }

        private static int[] Triangles(Vector3[] vertices, bool reverse)
        {
            int length = vertices.Length;
            List<int> triangles = new List<int>();
            int quads = (length - 2) / 2 + length % 2;

            for (int i = 0; i < quads; i++)
            {
                triangles.Add(i);
                triangles.Add(i + 1);
                triangles.Add(length - (i + 1));

                if (i + 1 != length - (i + 2))
                {
                    triangles.Add(length - (i + 1));
                    triangles.Add(i + 1);
                    triangles.Add(length - (i + 2));
                }
            }

            if (reverse)
            {
                triangles.Reverse();
            }

            return triangles.ToArray();
        }
    }
}