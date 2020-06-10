using Assets.Scripts.EMath.MathCalculator.Parsers.Node.Context;

namespace Assets.Scripts.EMath.MathCalculator.Parsers.Node
{
    interface INode
    {
        double Eval(IContext ctx);
    }
}
