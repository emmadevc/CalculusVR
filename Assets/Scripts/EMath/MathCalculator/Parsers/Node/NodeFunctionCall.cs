using Assets.Scripts.EMath.MathCalculator.Parsers.Node.Context;
using Assets.Scripts.EMath.Utils;
using System;

namespace Assets.Scripts.EMath.MathCalculator.Parsers.Node
{
    class NodeFunctionCall : INode
    {
        private readonly string name;
        private readonly INode[] arguments;
        private readonly Optional<Func<object[], double>> reserved;

        public NodeFunctionCall(string functionName, INode[] arguments)
        {
            this.name = functionName;
            this.arguments = arguments;
            this.reserved = ReservedFunction.Get(name);
        }

        public double Eval(IContext ctx)
        {
            var args = Arguments(ctx);

            return reserved
                .Map(f => f.Invoke(args))
                .OrElseGet(() => ctx.CallFunction(name, args));
        }

        private object[] Arguments(IContext ctx)
        {
            var args = new object[arguments.Length];

            for (int i = 0; i < arguments.Length; i++)
            {
                args[i] = arguments[i].Eval(ctx);
            }

            return args;
        }
    }
}
