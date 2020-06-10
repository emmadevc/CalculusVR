using Assets.Scripts.EMath.MathCalculator.Parsers.Node.Context;
using Assets.Scripts.EMath.Utils;

namespace Assets.Scripts.EMath.MathCalculator.Parsers.Node
{
    class NodeVariable : INode
    {
        private readonly string name;
        private readonly Optional<double> reserved;

        public NodeVariable(string name)
        {
            this.name = name;
            this.reserved = ReservedVariable.Get(name);
        }

        public double Eval(IContext ctx)
        {
            return reserved.OrElseGet(() => ctx.ResolveVariable(name));
        }

        public bool isReserved()
        {
            return reserved.isPresent;
        }
    }
}
