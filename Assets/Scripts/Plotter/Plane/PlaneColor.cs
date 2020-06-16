using Assets.Scripts.Plotter.Utils;
using UnityEngine;

namespace Assets.Scripts.Plotter.Plane
{
    class PlaneColor
    {
        private static Color[] colors = new Color[] {
            PlotUtils.CreateColor("#6a994e", 204)
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
