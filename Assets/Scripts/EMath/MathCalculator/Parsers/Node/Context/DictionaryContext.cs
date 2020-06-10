using Assets.Scripts.EMath.MathCalculator.Exceptions;
using System.Collections.Generic;

namespace Assets.Scripts.EMath.MathCalculator.Parsers.Node.Context
{
    class DictionaryContext : IContext
    {
        private readonly Dictionary<string, double> variables;

        public DictionaryContext(Dictionary<string, double> variables)
        {
            this.variables = variables;
        }

        public double CallFunction(string name, object[] arguments)
        {
            throw new InvalidFunctionException("No se puede llamar a una función de este contexto");
        }

        public double ResolveVariable(string name)
        {
            if (!variables.ContainsKey(name))
            {
                throw new InvalidVariableException($"Variable desconocida: '{name}'");
            }

            return variables[name];
        }
    }
}
