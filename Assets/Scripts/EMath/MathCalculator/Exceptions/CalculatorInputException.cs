using System;

namespace Assets.Scripts.EMath.MathCalculator.Exceptions
{
    class CalculatorInputException : Exception
    {
        public CalculatorInputException(string message) : base(message)
        {
        }
    }
}
