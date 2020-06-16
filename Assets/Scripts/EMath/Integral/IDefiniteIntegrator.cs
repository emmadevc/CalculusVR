using Assets.Scripts.EMath.Function;
using Assets.Scripts.EMath.Set;

namespace Assets.Scripts.EMath.Integral
{
    public interface IDefiniteIntegrator
    {
        DefiniteIntegral Integrate(IFunction function, double a, double b);
        DefiniteIntegral Integrate(IFunction function, Interval interval);
        DefiniteIntegral Integrate(IntervalFunction intervalFunction);
    }
}
