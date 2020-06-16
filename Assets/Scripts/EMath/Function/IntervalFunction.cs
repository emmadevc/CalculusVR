using Assets.Scripts.EMath.Set;

namespace Assets.Scripts.EMath.Function
{
    public class IntervalFunction
    {
        public IFunction function { get; }
        public Interval interval { get; }

        public IntervalFunction(IFunction function, double a, double b)
        {
            this.function = function;
            this.interval = new Interval(a, b);
        }

        public IntervalFunction(IFunction function, Interval interval)
        {
            this.function = function;
            this.interval = interval;
        }

        public double Evaluate(double x)
        {
            return function.Evaluate(x);
        }

        public double Distance()
        {
            return interval.Distance();
        }
    }
}
