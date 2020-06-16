using Assets.Scripts.EMath.Geometry;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Plotter.Plane
{
    class PlaneAction
    {
        public GameObject parent { get; }
        public List<Point> points { get; }
        public Material material { get; }
        public bool reverse { get; }

        public PlaneAction(GameObject parent, List<Point> points, Material material, bool reverse)
        {
            this.parent = parent;
            this.points = points;
            this.material = material;
            this.reverse = reverse;
        }
    }
}
