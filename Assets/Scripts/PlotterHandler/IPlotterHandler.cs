using Assets.Scripts.EMath.Function;
using Assets.Scripts.EMath.Geometry;

namespace Assets.Scripts.PlotterHandler
{
    public interface IPlotterHandler
    {
        
        int functionLimit { get; }

        bool axisInput { get; }

        bool negativeLimits { get; }

        void Plot(Axis axis, IFunction[] functions, double a, double b, double quality, 
            float rotation, float zoom);

        void Clear();

        string GetResultMessage();
    }
}
