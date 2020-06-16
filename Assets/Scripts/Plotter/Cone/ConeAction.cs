using Assets.Scripts.EMath.Geometry;
using UnityEngine;

namespace Assets.Scripts.Plotter.Cone
{
    class ConeAction
    {
        public Cone cone { get; }
        public GameObject parent { get; }
        public Point start { get; }
        public Point end { get; }
        public float baseScale { get; }
        public float pointScale { get; }
        public Material material { get; }

        public ConeAction(Cone cone, GameObject parent, Point start, Point end, float baseScale,
            float pointScale, Material material)
        {
            this.cone = cone;
            this.parent = parent;
            this.start = start;
            this.end = end;
            this.baseScale = baseScale;
            this.pointScale = pointScale;
            this.material = material;
        }
    }
}
