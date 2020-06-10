using Assets.Scripts.EMath.Utils;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.EMath.MathCalculator.Parsers.Node
{
    class ReservedVariable
    {
        private static readonly Dictionary<string, Optional<double>> variables;

        static ReservedVariable()
        {
            variables = new Dictionary<string, Optional<double>>();

            variables["e"] = Optional<double>.Of(Math.E);
            variables["pi"] = Optional<double>.Of(Math.PI);
        }

        public static Optional<double> Get(string variable)
        {
            if (!variables.ContainsKey(variable))
            {
                return Optional<double>.Empty();
            }

            return variables[variable];
        }
    }
}
