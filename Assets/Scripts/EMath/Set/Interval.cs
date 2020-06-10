using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.EMath.Set
{
    class Interval
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
