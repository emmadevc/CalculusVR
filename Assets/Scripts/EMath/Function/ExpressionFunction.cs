using Assets.Scripts.EMath.MathCalculator;
using Assets.Scripts.EMath.Geometry;
using System.Collections.Generic;
using Assets.Scripts.EMath.MathCalculator.Exceptions;
using System.Linq;

namespace Assets.Scripts.EMath.Function
{
    class ExpressionFunction : IFunction
    {
        private readonly string expression;
        private readonly string independent;

        private readonly Dictionary<string, double> image;
        private readonly Dictionary<string, double> variables;

        string IFunction.independent => independent;

        public ExpressionFunction(string expression)
        {
            ISet<string> vars = Calculator.Variables(expression);

            if (vars.Count > 1)
            {
                throw new InvalidVariableException("Las funciones sólo pueden tener una variable");
            }

            this.expression = expression;
            this.independent = vars.Select(v => v).First();

            image = new Dictionary<string, double>();
            variables = new Dictionary<string, double>();
        }

        public double Evaluate(double i)
        {
            string key = i.ToString();

            if (!image.ContainsKey(key))
            {
                variables[independent.ToString()] = i;
                image[key] = Calculator.Evaluate(expression, variables);
            }

            return image[key];
        }

        public IFunction Generate(string newExpression)
        {
            return new ExpressionFunction(newExpression.Replace("function", expression));
        }
    }
}
