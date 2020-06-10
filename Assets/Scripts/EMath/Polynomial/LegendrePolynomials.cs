using System;

namespace Assets.Scripts.EMath.Polynomial
{
    class LegendrePolynomials : IPolynomialSeries
    {
        private const int defaultN = 5;

        private readonly double[] weights;
        private readonly double[] roots;
        private readonly double[,] abscissas;
        private readonly int n;

        double[] IPolynomialSeries.weights => weights;
        double[] IPolynomialSeries.roots => roots;
        int IPolynomialSeries.n => n;

        public LegendrePolynomials(int n)
        {
            this.n = n;

            roots = new double[n];
            weights = new double[n];
            abscissas = new double[n + 1, n + 1];

            CreateAbscissas();
            CreateNodes();
        }

        public LegendrePolynomials() : this(defaultN)
        {
        }

        private void CreateAbscissas()
        {
            abscissas[0, 0] = 1;
            abscissas[1, 1] = 1;

            for (int i = 2; i <= n; i++)
            {
                abscissas[i, 0] = -(i - 1) * abscissas[i - 2, 0] / i;

                for (int j = 1; j <= i; j++)
                {
                    abscissas[i, j] = ((2 * i - 1) * abscissas[i - 1, j - 1]
                            - (i - 1) * abscissas[i - 2, j]) / i;
                }
            }
        }

        private void CreateNodes()
        {
            for (int i = 1; i <= n; i++)
            {
                double r = Math.Cos(Math.PI * (i - 0.25) / (n + 0.5));
                double r1;

                do
                {
                    r1 = r;
                    r -= Value(n, r) / Difference(n, r);
                } while (r != r1);

                roots[i - 1] = r;

                r1 = Difference(n, r);
                weights[i - 1] = 2 / ((1 - r * r) * r1 * r1);
            }
        }

        private double Difference(int ni, double x)
        {
            return ni * (x * Value(ni, x) - Value(ni - 1, x)) / (x * x - 1);
        }

        private double Value(int ni, double x)
        {
            double s = abscissas[ni, ni];

            for (int i = ni; i > 0; i--)
            {
                s = s * x + abscissas[ni, i - 1];
            }

            return s;
        }
    }
}
