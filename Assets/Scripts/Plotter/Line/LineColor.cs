
using Assets.Scripts.Plotter.Utils;
using UnityEngine;

namespace Assets.Scripts.Plotter.Line
{
    class LineColor
    {
        private static Color[] colors = new Color[] {
            PlotUtils.CreateColor(242, 95, 92),
            PlotUtils.CreateColor(255, 224, 102),
            PlotUtils.CreateColor(36, 123, 160)
        };

        public static Color Get(int i)
        {
            return colors[i];
        }

        public static int Count()
        {
            return colors.Length;
        }
    }
}
