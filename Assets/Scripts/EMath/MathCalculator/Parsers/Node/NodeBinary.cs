using Assets.Scripts.EMath.MathCalculator.Parsers.Node.Context;
using System;

namespace Assets.Scripts.EMath.MathCalculator.Parsers.Node
{
    class NodeBinary : INode
    {
        private readonly INode lhs;
        private readonly INode rhs;
        private readonly Func<double, double, double> op;

        public NodeBinary(INode lhs, INode rhs, Func<double, double, double> op)
        {
            this.lhs = lhs;
            this.rhs = rhs;
            this.op = op;
        }

        public double Eval(IContext ctx)
        {
            return op(lhs.Eval(ctx), rhs.Eval(ctx));
        }
    }
}
