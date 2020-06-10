using Assets.Scripts.EMath.Geometry;

namespace Assets.Scripts.EMath.Function
{
    class FunctionPoint
    {
        public double i { get; set; }
        public double d { get; set; }
        public double f { get; set; }

        public FunctionPoint()
        {
        }

        public FunctionPoint(double i, double d = 0, double f = 0)
        {
            Set(i, d, f);
        }

        public void Set(double i, double d = 0, double f = 0)
        {
            this.i = i;
            this.d = d;
            this.f = f;
        }

        public Point ToPoint(Axis independent, Axis dependent)
        {
            double x = independent == Axis.x ? i : dependent == Axis.x ? d : f;
            double y = independent == Axis.y ? i : dependent == Axis.y ? d : f;
            double z = independent == Axis.z ? i : dependent == Axis.z ? d : f;

            return new Point(x, y, z);
        }
    }
}
