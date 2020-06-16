using Assets.Scripts.EMath.Geometry;
using Assets.Scripts.Plotter.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Plotter.Label
{
    class LabelPlotter
    {
        private static readonly List<LabelAction> actions = new List<LabelAction>();
        private static readonly Pool<Label> pool = new Pool<Label>("label");

        public static void Plot(GameObject parent, Point start, Point end, string message, Color color,
            Alignment alignment)
        {
            AddAction(pool.Get(), parent, start, end, message, color, alignment);
        }

        private static void AddAction(Label label, GameObject parent, Point start, Point end,
            string message, Color color, Alignment alignment)
        {
            actions.Add(new LabelAction(label, parent, start, end, message, color, alignment));
        }

        public static void PlotActions()
        {
            foreach (LabelAction action in actions)
            {
                Plot(action.label, action.parent, action.start, action.end, action.message, action.color, 
                    action.alignment);
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

        private static void Plot(Label label, GameObject parent, Point start, Point end,
            string message, Color color, Alignment alignment)
        {
            label.gameObject.transform.SetParent(parent.transform);

            Vector3 pos = PlotUtils.ToVector3(start);
            float height = Vector3.Distance(pos, PlotUtils.ToVector3(end));
            float width = message.Length * height * 1.5f;

            label.mesh.text = message;
            label.mesh.alignment = LabelAlignment(alignment);
            label.mesh.rectTransform.sizeDelta = new Vector2(width, height);
            label.mesh.color = color;

            label.gameObject.transform.localPosition = new Vector3(pos.x + TextOffset(width, alignment), pos.y, pos.z);
            
            label.gameObject.SetActive(true);
        }

        private static TMPro.TextAlignmentOptions LabelAlignment(Alignment alignment)
        {
            switch (alignment)
            {
                case Alignment.Left:
                    return TMPro.TextAlignmentOptions.MidlineLeft;
                case Alignment.Right:
                    return TMPro.TextAlignmentOptions.MidlineRight;
            }

            return TMPro.TextAlignmentOptions.Midline;
        }

        private static float TextOffset(float width, Alignment alignment)
        {
            switch (alignment)
            {
                case Alignment.Left:
                    return width * 0.5f;
                case Alignment.Right:
                    return -width * 0.5f;
            }

            return 0;
        }
    }
}
