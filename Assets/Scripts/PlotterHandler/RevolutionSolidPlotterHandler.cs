using Assets.Scripts.EMath.Function;
using Assets.Scripts.EMath.Geometry;
using Assets.Scripts.Plotter;
using Assets.Scripts.Plotter.Input;
using Assets.Scripts.PlotterHandler;
using Assets.Scripts.PlotterHandler.Utils;
using UnityEngine;

public class RevolutionSolidPlotterHandler : MonoBehaviour, IPlotterHandler
{
    public Material planeMaterial;
    public Material axisMaterial;
    public Material functionMaterial;
    public UIHandler handler;

    private FunctionPlotter plotter;

    int IPlotterHandler.functionLimit => 1;

    bool IPlotterHandler.axisInput => true;

    bool IPlotterHandler.negativeLimits => false;

    private Transform parent;

    void Start()
    {
        plotter = new RevolutionSolidPlotter(handler.integrator, gameObject, handler.lineMesh, axisMaterial,
            functionMaterial, planeMaterial);

        parent = gameObject.transform.parent;
    }

    public void Plot(Axis rotationAxis, IFunction[] functions, double a, double b, double quality,
        float rotation, float zoom)
    {
        TransformationHandler.Reset(gameObject);

        plotter.Plot(new RevolutionSolidInput(rotationAxis, functions, a, b, quality));

        TransformationHandler.Transform(gameObject, rotation, zoom);

        gameObject.transform.SetParent(parent, false);
    }

    public void Clear()
    {
        plotter.Clear();
    }

    public string GetResultMessage()
    {
        return $"Volumen: {(double.IsNaN(plotter.result) ? "Indeterminable" : plotter.result.ToString())}";
    }
}
