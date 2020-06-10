using Assets.Scripts.EMath.MathCalculator.Exceptions;
using Assets.Scripts.EMath.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Assets.Scripts.EMath.MathCalculator.Parsers.Node
{
    class ReservedFunction
    {
        private static readonly Dictionary<string, Optional<Func<object[], double>>> functions;

        static ReservedFunction()
        {
            functions = new Dictionary<string, Optional<Func<object[], double>>>();
            Type mathType = typeof(Math);

            functions["abs"] = createFunction(mathType.GetMethod("Abs", new[] { typeof(double) }));

            functions["acos"] = createFunction(mathType.GetMethod("Acos", new[] { typeof(double) }));
            functions["acosh"] = createFunction(mathType.GetMethod("Acosh", new[] { typeof(double) }));
            functions["asin"] = createFunction(mathType.GetMethod("Asin", new[] { typeof(double) }));
            functions["asinh"] = createFunction(mathType.GetMethod("Asinh", new[] { typeof(double) }));
            functions["atan"] = createFunction(mathType.GetMethod("Atan", new[] { typeof(double) }));
            functions["atan2"] = createFunction(mathType.GetMethod("Atan2", new[] { typeof(double) }));
            functions["atanh"] = createFunction(mathType.GetMethod("Atanh", new[] { typeof(double) }));
            functions["cos"] = createFunction(mathType.GetMethod("Cos", new[] { typeof(double) }));
            functions["cosh"] = createFunction(mathType.GetMethod("Cosh", new[] { typeof(double) }));
            functions["sin"] = createFunction(mathType.GetMethod("Sin", new[] { typeof(double) }));
            functions["sinh"] = createFunction(mathType.GetMethod("Sinh", new[] { typeof(double) }));
            functions["tan"] = createFunction(mathType.GetMethod("Tan", new[] { typeof(double) }));
            functions["tanh"] = createFunction(mathType.GetMethod("Tanh", new[] { typeof(double) }));

            functions["pow"] = createFunction(mathType.GetMethod("Pow", new[] { typeof(double) }));
            functions["sqrt"] = createFunction(mathType.GetMethod("Sqrt", new[] { typeof(double) }));

            functions["exp"] = createFunction(mathType.GetMethod("Exp", new[] { typeof(double) }));
            functions["ln"] = createFunction(mathType.GetMethod("Log", new[] { typeof(double) }));

            functions["ceiling"] = createFunction(mathType.GetMethod("Ceiling", new[] { typeof(double) }));
            functions["floor"] = createFunction(mathType.GetMethod("Floor", new[] { typeof(double) }));
            functions["round"] = createFunction(mathType.GetMethod("Round", new[] { typeof(double) }));
            functions["truncate"] = createFunction(mathType.GetMethod("Truncate", new[] { typeof(double) }));
        }

        public static Optional<Func<object[], double>> Get(string function)
        {
            if (function.StartsWith("log"))
            {
                return EvaluateLog(function.Substring(3));
            }

            if (function.StartsWith("root"))
            {
                return EvaluateRoot(function.Substring(4));
            }

            if (!functions.ContainsKey(function))
            {
                return Optional<Func<object[], double>>.Empty();
            }

            return functions[function];
        }

        private static Optional<Func<object[], double>> EvaluateLog(string stringBase)
        {
            try
            {
                int logBase = stringBase.Length == 0 ? 10 : int.Parse(stringBase);

                return Optional<Func<object[], double>>.Of(args => Math.Log((double)args[0], logBase));
            }
            catch (Exception)
            {
                throw new InvalidFunctionException($"Base de logaritmo invalido: '{stringBase}'");
            }
        }

        private static Optional<Func<object[], double>> EvaluateRoot(string stringBase)
        {
            try
            {
                int rootBase = stringBase.Length == 0 ? 2 : int.Parse(stringBase);

                if (rootBase == 1)
                {
                    throw new Exception();
                }

                return Optional<Func<object[], double>>.Of(args => Math.Pow((double)args[0], 1.0f / rootBase));
            }
            catch (Exception)
            {
                throw new InvalidFunctionException($"Base de raiz invalida: '{stringBase}'");
            }
        }

        private static Optional<Func<object[], double>> createFunction(MethodInfo method)
        {
            return Optional<Func<object[], double>>.Of(args => (double)method.Invoke(null, args));
        }
    }
}
