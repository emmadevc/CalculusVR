using Assets.Scripts.EMath.Geometry;
using UnityEngine;

namespace Assets.Scripts.Plotter.Line
{
    class LineAction
    {
        public Line line { get; }
        public GameObject parent { get; }
        public Point start { get; }

        public Point end { get; }
        public Material material { get; }
        public Mesh mesh { get; }
        public float radius { get; }

        public LineAction(Line line, GameObject parent, Point start, Point end, Material material,
            Mesh mesh, float radius)
        {
            this.line = line;
            this.parent = parent;
            this.start = start;
            this.end = end;
            this.material = material;
            this.mesh = mesh;
            this.radius = radius;
        }
    }
}
