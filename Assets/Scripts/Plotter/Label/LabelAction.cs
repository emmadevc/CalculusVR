using Assets.Scripts.EMath.Geometry;
using UnityEngine;

namespace Assets.Scripts.Plotter.Label
{
    class LabelAction
    {
        public Label label { get; }
        public GameObject parent { get; }
        public Point start { get; }
        public Point end { get; }
        public string message { get; }
        public Color color { get; }
        public Alignment alignment { get; }

        public LabelAction(Label label, GameObject parent, Point start, Point end, string message, 
            Color color, Alignment alignment)
        {
            this.label = label;
            this.parent = parent;
            this.start = start;
            this.end = end;
            this.message = message;
            this.color = color;
            this.alignment = alignment;
        }
    }
}
