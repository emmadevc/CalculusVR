namespace Assets.Scripts.EMath.MathCalculator.Parsers.Node.Context
{
    interface IContext
    {
        double ResolveVariable(string name);
        double CallFunction(string name, object[] arguments);
    }
}
