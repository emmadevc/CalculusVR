using Assets.Scripts.EMath.Function;
using Assets.Scripts.EMath.Geometry;
using Assets.Scripts.EMath.Integral;
using Assets.Scripts.Plotter;
using Assets.Scripts.Plotter.Input;
using Assets.Scripts.Plotter.Utils;
using System.Collections.Generic;
using UnityEngine;

public class TestPlotter : MonoBehaviour
{
    public GameObject parent;
    public Material planeMaterial;
    public Material axisMaterial;
    public Material functionMaterial;
    public Mesh lineMesh;

    private readonly double quality = 6; // Values between 1 (minQuality) and 6 (maxQuality)

    private readonly Dictionary<PlotterType, FunctionPlotter> plotters =
        new Dictionary<PlotterType, FunctionPlotter>();

    private readonly PlotterType selected = PlotterType.area;

    private readonly string[] expressions = new string[] { "x" };
    private readonly Axis rotationAxis = Axis.x;
    private double a = 0;
    private double b = 0.7;

    /*private readonly string[] expressions = new string[] { "x^2", "1/x", "2-x" };
    private readonly Axis rotationAxis = Axis.x;
    private const double a = -3;
    private const double b = 3;*/

    /*private readonly string[] expressions = new string[] { "2x", "x^2" };
    private const double a = -1;
    private const double b = 3;*/

    /*private readonly string[] expressions = new string[] { "2y", "y^2" };
    private const double a = -1;
    private const double b = 3;*/

    /*private readonly string[] expressions = new string[] { "2x^2" };
    private const double a = 0;
    private const double b = 5;*/

    /*private readonly string[] expressions = new string[] { "x^2" };
    private readonly Axis rotationAxis = Axis.x;
    private const double a = 0;
    private const double b = 5;*/

    /*private readonly string[] expressions = new string[] { "1/x" };
    private readonly Axis rotationAxis = Axis.x;
    private const double a = 0;
    private const double b = 5;*/

    /*private readonly string[] expressions = new string[] { "y" };
    private readonly Axis rotationAxis = Axis.x;
    private const double a = 0;
    private const double b = 3;*/

    /*private readonly string[] expressions = new string[] { "1/x" };
    private readonly Axis rotationAxis = Axis.y;
    private const double a = 0;
    private const double b = 3;*/

    private IDefiniteIntegrator integrator = new GaussDefiniteIntegrator();

    void Start()
    {
        plotters[PlotterType.area] = new AreaPlotter(integrator, parent, lineMesh, axisMaterial,
            functionMaterial, planeMaterial);

        plotters[PlotterType.revolutionSolid] = new RevolutionSolidPlotter(integrator, parent, lineMesh,
            axisMaterial, functionMaterial, planeMaterial);

        Plot();
    }

    private void Plot()
    {
        plotters[selected].Plot(PlotInput());

        Debug.Log("Result: " + plotters[selected].result);
    }

    private IPlotInput PlotInput()
    {
        if (selected == PlotterType.area)
        {
            return new AreaPlotInput(GetFunctions(), a, b, quality);
        }

        return new RevolutionSolidInput(rotationAxis, GetFunctions(), a, b, quality);
    }

    private IFunction[] GetFunctions()
    {
        IFunction[] functions = new IFunction[expressions.Length];

        for (int i = 0; i < functions.Length; i++)
        {
            functions[i] = new ExpressionFunction(expressions[i]);
        }

        return functions;
    }
}
