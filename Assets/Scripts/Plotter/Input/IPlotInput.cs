using Assets.Scripts.EMath.Function;
using Assets.Scripts.EMath.Geometry;

namespace Assets.Scripts.Plotter.Input
{
    interface IPlotInput
    {
        Axis independent { get; }
        Axis dependent { get; }
        IFunction[] functions { get; }
        double a { get; }
        double b { get; }
        double quality { get; }
    }
}
