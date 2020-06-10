using UnityEngine.UIElements.Experimental;

namespace Assets.Scripts.EMath.Polynomial
{
    interface IPolynomialSeries
    {
        double[] weights { get; }
        double[] roots { get; }

        int n { get; }
    }
}
