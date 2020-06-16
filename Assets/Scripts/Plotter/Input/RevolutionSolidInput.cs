using Assets.Scripts.EMath.Function;
using Assets.Scripts.EMath.Geometry;

namespace Assets.Scripts.Plotter.Input
{
    class RevolutionSolidInput : IPlotInput
    {
        private readonly Axis independent;
        private readonly Axis dependent;
        private readonly IFunction[] functions;
        private readonly double a;
        private readonly double b;
        private readonly double quality;

        public Axis rotate { get; }

        Axis IPlotInput.independent => independent;
        Axis IPlotInput.dependent => dependent;
        IFunction[] IPlotInput.functions => functions;
        double IPlotInput.a => a;
        double IPlotInput.b => b;
        double IPlotInput.quality => quality;

        public RevolutionSolidInput(Axis rotate, IFunction[] functions, 
            double a, double b, double quality)
        {
            string i = functions[0].independent;

            this.independent = i == "x" ? Axis.x : Axis.y;
            this.dependent = i == "x" ? Axis.y : Axis.x;
            this.rotate = rotate;
            this.functions = functions;
            this.a = a;
            this.b = b;
            this.quality = quality;
        }
    }
}
