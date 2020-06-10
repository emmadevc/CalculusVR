using Assets.Scripts.EMath.MathCalculator.Parsers.Node.Context;
using System;

namespace Assets.Scripts.EMath.MathCalculator.Parsers.Node
{
    class NodeUnary : INode
    {
        private readonly INode rhs;
        private readonly Func<double, double> op;

        public NodeUnary(INode rhs, Func<double, double> op)
        {
            this.rhs = rhs;
            this.op = op;
        }

        public double Eval(IContext ctx)
        {
            return op(rhs.Eval(ctx));
        }
    }
}
