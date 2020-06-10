namespace Assets.Scripts.EMath.Function
{
    interface IFunction
    {
        string independent { get; }

        double Evaluate(double i);

        IFunction Generate(string expression);
    }
}
