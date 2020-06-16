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
            throw new InvalidFunctionException($"La función introducida es inválida: '{name}'");
        }

        public double ResolveVariable(string name)
        {
            if (!variables.ContainsKey(name))
            {
                throw new InvalidVariableException($"La variable introducida es inválida: '{name}'");
            }

            return variables[name];
        }
    }
}
