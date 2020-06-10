using Assets.Scripts.EMath.Geometry;

namespace Assets.Scripts.EMath.Function
{
    class FunctionLine
    {
        public static FunctionPoint Intersection(double i1, double da1, double db1,
            double i2, double da2, double db2)
        {
            double ma = Line.Slope(i1, i2, da1, da2);
            double mb = Line.Slope(i1, i2, db1, db2);

            double root = (((ma - mb) * i1) - da1 + db1) / (ma - mb);

            return new FunctionPoint(root, ma * (root - i1) + da1);
        }
    }
}
