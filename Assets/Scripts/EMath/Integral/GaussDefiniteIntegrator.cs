using Assets.Scripts.EMath.Function;
using Assets.Scripts.EMath.Polynomial;
using Assets.Scripts.EMath.Set;

namespace Assets.Scripts.EMath.Integral
{
    class GaussDefiniteIntegrator : IDefiniteIntegrator
    {
        private readonly IPolynomialSeries polynomials;

        public GaussDefiniteIntegrator()
        {
            polynomials = new LegendrePolynomials();
        }

        public GaussDefiniteIntegrator(IPolynomialSeries polynomials)
        {
            this.polynomials = polynomials;
        }

        public DefiniteIntegral Integrate(IFunction function, double a, double b)
        {
            double c1 = (b + a) / 2.0;
            double c2 = (b - a) / 2.0;
            
            double sum = 0;

            for (int i = 0; i < polynomials.n; i++)
            {
                sum += polynomials.weights[i] * function.Evaluate(c1 + c2 * polynomials.roots[i]);
            }

            return new DefiniteIntegral(c2 * sum);
        }

        public DefiniteIntegral Integrate(IFunction function, Interval interval)
        {
            return Integrate(function, interval.a, interval.b);
        }

        public DefiniteIntegral Integrate(IntervalFunction intervalFunction)
        {
            return Integrate(intervalFunction.function, intervalFunction.interval.a, intervalFunction.interval.b);
        }
    }
}
