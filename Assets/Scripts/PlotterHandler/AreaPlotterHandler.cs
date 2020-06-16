using Assets.Scripts.EMath.Function;
using Assets.Scripts.EMath.Geometry;
using Assets.Scripts.Plotter;
using Assets.Scripts.Plotter.Input;
using Assets.Scripts.PlotterHandler;
using Assets.Scripts.PlotterHandler.Utils;
using UnityEngine;

public class AreaPlotterHandler : MonoBehaviour, IPlotterHandler
{
    public Material planeMaterial;
    public Material axisMaterial;
    public Material functionMaterial;
    public UIHandler handler;

    private FunctionPlotter plotter;

    int IPlotterHandler.functionLimit => 3;

    bool IPlotterHandler.axisInput => false;

    bool IPlotterHandler.negativeLimits => true;

    private Transform parent;

    void Start()
    {
        plotter = new AreaPlotter(handler.integrator, gameObject, handler.lineMesh, axisMaterial,
            functionMaterial, planeMaterial);

        parent = gameObject.transform.parent;
    }

    public void Plot(Axis axis, IFunction[] functions, double a, double b, double quality,
        float rotation, float zoom)
    {
        TransformationHandler.Reset(gameObject);

        plotter.Plot(new AreaPlotInput(functions, a, b, quality));

        TransformationHandler.Transform(gameObject, rotation, zoom);

        gameObject.transform.SetParent(parent, false);
    }

    public void Clear()
    {
        plotter.Clear();
    }

    public string GetResultMessage()
    {
        return $"Area: {(double.IsNaN(plotter.result) ? "Indeterminable" : plotter.result.ToString())}";
    }
}
