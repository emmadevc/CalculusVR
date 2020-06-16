using Assets.Scripts.EMath.Geometry;
using Assets.Scripts.Plotter.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Plotter.Line
{
    class LinePlotter
    {
        private static readonly List<LineAction> actions = new List<LineAction>();
        private static readonly Pool<Line> pool = new Pool<Line>("line");

        public static void Plot(GameObject parent, Point start, Point end, Material material, Mesh mesh,
            float radius)
        {
            AddAction(pool.Get(), parent, start, end, material, mesh, radius);
        }

        private static void AddAction(Line line, GameObject parent, Point start, Point end, Material material,
            Mesh mesh, float radius)
        {
            actions.Add(new LineAction(line, parent, start, end, material, mesh, radius));
        }

        public static void PlotActions()
        {
            foreach (LineAction action in actions)
            {
                Plot(action.line, action.parent, action.start, action.end, action.material, action.mesh, 
                    action.radius);
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

        private static void Plot(Line line, GameObject parent, Point start, Point end, Material material,
            Mesh mesh, float radius)
        {
            line.gameObject.transform.parent = parent.transform;

            line.cylinder.transform.localPosition = new Vector3(0f, 1f, 0f);
            line.cylinder.transform.localScale = new Vector3(radius, 1f, radius);
            line.meshFilter.mesh = mesh;
            line.meshRenderer.material = material;

            Locate(line.gameObject, start, end);

            line.gameObject.SetActive(true);
        }

        private static void Locate(GameObject gameObject, Point start, Point end)
        {
            Vector3 from = PlotUtils.ToVector3(start);
            Vector3 to = PlotUtils.ToVector3(end);

            float height = 0.5f * Vector3.Distance(from, to);

            gameObject.transform.localPosition = from;

            gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x, height,
                gameObject.transform.localScale.z);

            gameObject.transform.LookAt(to);
            gameObject.transform.rotation *= Quaternion.Euler(90, 0, 0);
        }
    }
}
