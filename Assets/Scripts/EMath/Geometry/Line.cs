using System;

namespace Assets.Scripts.EMath.Geometry
{
    class Line
    {
        public static Point Intersection(double x1, double ya1, double yb1,
            double x2, double ya2, double yb2)
        {
            double ma = Slope(x1, x2, ya1, ya2);
            double mb = Slope(x1, x2, yb1, yb2);

            double root = (((ma - mb) * x1) - ya1 + yb1) / (ma - mb);

            return new Point(root, ma * (root - x1) + ya1);
        }

        public static double Root(double x1, double y1, double x2, double y2)
        {
            return -y1 / ((y2 - y1) / (x2 - x1)) + x1;
        }

        public static double Slope(double x1, double x2, double y1, double y2)
        {
            return (y2 - y1) / (x2 - x1);
        }
    }
}
