using Assets.Scripts.EMath.MathCalculator.Parsers.Node.Context;

namespace Assets.Scripts.EMath.MathCalculator.Parsers.Node
{
    class NodeNumber : INode
    {
        private readonly double number;

        public NodeNumber(double number)
        {
            this.number = number;
        }

        public double Eval(IContext ctx)
        {
            return number;
        }
    }
}
