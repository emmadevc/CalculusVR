using Assets.Scripts.EMath.MathCalculator.Exceptions;

namespace Assets.Scripts.EMath.MathCalculator.Parsers.Node.Context
{
    class ReflectionContext
    {
        private readonly object targetObject;

        public ReflectionContext(object targetObject)
        {
            this.targetObject = targetObject;
        }

        public double CallFunction(string name, object[] arguments)
        {
            var mi = targetObject.GetType().GetMethod(name);

            if (mi == null)
            {
                throw new InvalidFunctionException($"La función introducida es inválida: '{name}'");
            }

            return (double)mi.Invoke(targetObject, arguments);
        }

        public double ResolveVariable(string name)
        {
            var pi = targetObject.GetType().GetProperty(name);

            if (pi == null)
            {
                throw new InvalidVariableException($"La variable introducida es inválida: '{name}'");
            }

            return (double)pi.GetValue(targetObject);
        }
    }
}
