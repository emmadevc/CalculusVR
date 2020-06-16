namespace Assets.Scripts.EMath.Set
{
    public class Interval
    {
        public double a { get; }
        public double b { get; }

        public Interval(double a, double b)
        {
            this.a = a;
            this.b = b;
        }

        public double Distance()
        {
            return b-a;
        }
    }
}
