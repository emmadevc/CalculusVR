using System;

namespace Assets.Scripts.EMath.Geometry
{
    class Point
    {
        public static readonly Point origin = new Point(0, 0, 0);

        public double x { get; }
        public double y { get; }
        public double z { get; }

        public Point(double x, double y = 0, double z = 0)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public double Distance(Point other)
        {
            return (Math.Sqrt(Math.Pow(other.x - x, 2) + Math.Pow(other.y - y, 2) + Math.Pow(other.z - z, 2)));
        }

        public Point Rotate(double rollDegress, double pitchDegress = 0, double yawDegrees = 0)
        {
            double roll = rollDegress * 0.0174533;
            double pitch = pitchDegress * 0.0174533;
            double yaw = yawDegrees * 0.0174533;

            var cosa = Math.Cos(yaw);
            var sina = Math.Sin(yaw);

            var cosb = Math.Cos(pitch);
            var sinb = Math.Sin(pitch);

            var cosc = Math.Cos(roll);
            var sinc = Math.Sin(roll);

            var axx = cosa * cosb;
            var axy = cosa * sinb * sinc - sina * cosc;
            var axz = cosa * sinb * cosc + sina * sinc;

            var ayx = sina * cosb;
            var ayy = sina * sinb * sinc + cosa * cosc;
            var ayz = sina * sinb * cosc - cosa * sinc;

            var azx = -sinb;
            var azy = cosb * sinc;
            var azz = cosb * cosc;

            return new Point(axx * x + axy * y + axz * z, ayx * x + ayy * y + ayz * z,
                azx * x + azy * y + azz * z);
        }
    }
}
