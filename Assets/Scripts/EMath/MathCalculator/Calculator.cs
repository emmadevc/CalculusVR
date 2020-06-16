using Assets.Scripts.EMath.MathCalculator.Parsers;
using System.Collections.Generic;

namespace Assets.Scripts.EMath.MathCalculator
{
    class Calculator
    {
        private static readonly Dictionary<string, Parser> parsers = new Dictionary<string, Parser>();

        public static double Evaluate(string expression)
        {
            return Parser(expression).Eval();
        }

        public static double Evaluate(string expression, Dictionary<string, double> variables)
        {
            return Parser(expression).Eval(variables);
        }

        public static ISet<string> Variables(string expression)
        {
            return Parser(expression).variables;
        }

        public static void Clear()
        {
            parsers.Clear();
        }

        private static Parser Parser(string expression)
        {
            if (!parsers.ContainsKey(expression))
            {
                parsers[expression] = new Parser(expression);
            }

            return parsers[expression];
        }
    }
}
