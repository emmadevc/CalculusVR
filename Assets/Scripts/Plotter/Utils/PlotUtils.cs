using Assets.Scripts.EMath.Geometry;
using Assets.Scripts.EMath.Set;
using System;
using UnityEngine;

namespace Assets.Scripts.Plotter.Utils
{
    class PlotUtils
    {
        public static float scale { get; private set; }
        public const int qualityLevels = 6;
        private const float axisRadius = 0.016f;
        private const float functionRadius = 0.02f;

        static PlotUtils()
        {
            ResetScale();
        }

        public static float AxisRadius()
        {
            return axisRadius;
        }

        public static float FunctionRadius()
        {
            return functionRadius;
        }

        public static void ResetScale()
        {
            scale = 1;
        }

        public static void SetScale(float proportion, Interval xInterval, Interval yInterval)
        {
            scale = proportion / MinorDistance(xInterval, yInterval);
        }

        public static float MinorDistance(Interval one, Interval two)
        {
            double oneDistance = one.Distance();
            double twoDistance = two.Distance();

            return (float)(oneDistance < twoDistance ? oneDistance : twoDistance);
        }

        public static Vector3 ToVector3(Point p)
        {
            return new Vector3((float)p.x * scale, (float)p.y * scale, (float)p.z * scale);
        }

        public static Color CreateColor(int r, int g, int b, int a = 255)
        {
            return new Color(r / 255f, g / 255f, b / 255f, a / 255f);
        }

        public static Color CreateColor(string hex)
        {
            string r = Substring(hex, 1, 2);
            string g = Substring(hex, 3, 2);
            string b = Substring(hex, 5, 2);
            string a = Substring(hex, 7, 2);

            return CreateColor(ToInt(r), ToInt(g), ToInt(b), string.IsNullOrEmpty(a) ? 255 : ToInt(a));
        }

        public static Color CreateColor(string hex, int a)
        {
            string r = Substring(hex, 1, 2);
            string g = Substring(hex, 3, 2);
            string b = Substring(hex, 5, 2);

            return CreateColor(ToInt(r), ToInt(g), ToInt(b), a);
        }

        private static string Substring(string str, int start, int length)
        {
            try
            {
                return str.Substring(start, length);
            }
            catch (Exception e)
            {
            }

            return "";
        }

        private static int ToInt(string str)
        {
            try
            {
                if (!string.IsNullOrEmpty(str))
                {
                    return (int)Convert.ToInt64(str, 16);
                }

            }
            catch (Exception e)
            {
            }

            return 0;
        }
    }
}
