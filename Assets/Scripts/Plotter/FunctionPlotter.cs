using Assets.Scripts.EMath.Function;
using Assets.Scripts.EMath.Geometry;
using Assets.Scripts.EMath.Set;
using Assets.Scripts.Plotter.Axes;
using Assets.Scripts.Plotter.Cone;
using Assets.Scripts.Plotter.Input;
using Assets.Scripts.Plotter.Label;
using Assets.Scripts.Plotter.Line;
using Assets.Scripts.Plotter.Plane;
using Assets.Scripts.Plotter.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Plotter
{
    abstract class FunctionPlotter
    {
        protected readonly GameObject parent;
        private readonly Mesh lineMesh;
        private readonly Material axisMaterial;

        protected readonly Material[] functionMaterials;
        protected readonly Material[] planeMaterials;

        protected readonly List<List<Point>> functionPoints;
        protected readonly Dictionary<Axis, bool> axisEnabled;

        protected IFunction[] functions;
        protected bool[] continuousFunction;
        protected bool[] closedFunction;

        protected bool continuous;
        protected bool closed;

        // Independent values, usually x
        protected Axis independent;
        private readonly FunctionPoint minI;
        private readonly FunctionPoint maxI;

        // Dependent values, usually y
        protected Axis dependent;
        private readonly FunctionPoint minD;
        private readonly FunctionPoint maxD;

        // Axis that is not independent nor dependent (basically not used)
        private readonly FunctionPoint minF;
        private readonly FunctionPoint maxF;

        private Interval xInterval;
        private Interval yInterval;
        private Interval zInterval;

        private const double maxQuality = 0.005;
        private const double minQuality = 0.1;
        private const double qualityInterval = (minQuality - maxQuality) / (PlotUtils.qualityLevels - 1);

        private const double rootTolerance = 0.08;

        protected float scale;

        public abstract double result { get; }

        protected FunctionPlotter(GameObject parent, Mesh lineMesh, Material axisMaterial,
            Material functionMaterial, Material planeMaterial)
        {
            this.parent = parent;
            this.lineMesh = lineMesh;
            this.axisMaterial = axisMaterial;
            this.functionPoints = new List<List<Point>>();
            this.functionMaterials = new Material[LineColor.Count()];
            this.planeMaterials = new Material[PlaneColor.Count()];
            this.axisEnabled = new Dictionary<Axis, bool>();

            axisEnabled[Axis.x] = axisEnabled[Axis.y] = axisEnabled[Axis.z] = true;

            minI = new FunctionPoint();
            maxI = new FunctionPoint();
            minD = new FunctionPoint();
            maxD = new FunctionPoint();
            minF = new FunctionPoint();
            maxF = new FunctionPoint();

            CreateMaterials(functionMaterial, planeMaterial);
        }

        public void Plot(IPlotInput input)
        {
            if (input.functions.Length > 0)
            {
                PlotUtils.ResetScale();

                Clear();

                functions = input.functions;
                independent = input.independent;
                dependent = input.dependent;
                continuous = true;
                closed = true;

                continuousFunction = new bool[functions.Length];
                closedFunction = new bool[functions.Length];

                functionPoints.Clear();

                for (int i = 0; i < functions.Length; i++)
                {
                    continuousFunction[i] = true;
                    closedFunction[i] = true;
                    functionPoints.Add(new List<Point>());
                }

                BeforePlot(input);

                minI.Set(double.MaxValue);
                maxI.Set(double.MinValue);

                minD.Set(0, double.MaxValue);
                maxD.Set(0, double.MinValue);

                minF.Set(0, 0, double.MaxValue);
                maxF.Set(0, 0, double.MinValue);

                PlotFunctions(input.a, input.b, input.quality);

                AfterPlot(input.a, input.b, minI, maxI, minD, maxD, minF, maxF);

                PlotAxis();

                PlanePlotter.PlotActions();
                LinePlotter.PlotActions();
                ConePlotter.PlotActions();
                LabelPlotter.PlotActions();
            }
        }

        public void Clear()
        {
            PlanePlotter.Clear();
            LinePlotter.Clear();
            LabelPlotter.Clear();
            ConePlotter.Clear();
        }

        private void PlotFunctions(double a, double b, double quality)
        {
            double h = (b - a) * (maxQuality + qualityInterval * (PlotUtils.qualityLevels - quality));
            double i = a;

            do
            {
                if ((decimal)(i + h) > (decimal)b)
                {
                    h = b - i;
                }

                BeforeLinePlot(a, b, i, h);
                PlotFunctionLines(i, i + h, b);

                i += h;

                AfterLinePlot(a, b, i, h);
            } while (i < b);
        }

        protected virtual void BeforePlot(IPlotInput input)
        {
        }

        protected virtual void BeforeLinePlot(double a, double b, double i, double h)
        {
        }

        protected virtual void AfterLinePlot(double a, double b, double i, double h)
        {
        }

        protected virtual void AfterPlot(double a, double b, FunctionPoint minI, FunctionPoint maxI,
            FunctionPoint minD, FunctionPoint maxD, FunctionPoint minF, FunctionPoint maxF)
        {
        }

        private void PlotFunctionLines(double i, double ih, double b)
        {
            for (int c = 0; c < functions.Length; c++)
            {
                AddFunctionLine(functions[c], i, ih, b, functionMaterials[c], functionPoints[c], c);
            }
        }

        private void AddFunctionLine(IFunction function, double i, double ih, double b, Material material,
            List<Point> points, int index)
        {
            double d = function.Evaluate(i);

            if (IsInvalid(d))
            {
                SetInvalidContinuity(points, index);

                return;
            }

            Point start = GetPoint(i, d);
            points.Add(start);

            double dh = function.Evaluate(ih);

            if (IsInvalid(dh))
            {
                SetInvalidContinuity(ih, b, points, index);

                return;
            }

            Point end = GetPoint(ih, dh);

            if ((decimal)ih >= (decimal)b)
            {
                points.Add(end);
            }

            if (UnsatisfiedRoot(function, i, d, ih, dh))
            {
                SetInvalidContinuity(ih, b, points, index);

                return;
            }

            PlotLine(start, end, material);
        }

        private void SetInvalidContinuity(List<Point> points, int index)
        {
            SetInvalidContinuity(double.NaN, double.NaN, false, points, index);
        }

        private void SetInvalidContinuity(double i, double b, List<Point> points, int index)
        {
            SetInvalidContinuity(i, b, true, points, index);
        }

        private void SetInvalidContinuity(double i, double b, bool end, List<Point> points, int index)
        {
            continuousFunction[index] = false;
            continuous = false;

            SetOpenFunction(i, b, end, points, index);
        }

        private void SetOpenFunction(double i, double b, bool end, List<Point> points, int index)
        {
            if (!end && points.Count > 0)
            {
                closedFunction[index] = false;
                closed = false;
            }

            if (end && points.Count > 0 && i < b)
            {
                closedFunction[index] = false;
                closed = false;
            }
        }

        private bool UnsatisfiedRoot(IFunction function, double i1, double d1, double i2, double d2)
        {
            if (Math.Sign(d1) != Math.Sign(d2))
            {
                double root = EMath.Geometry.Line.Root(i1, d1, i2, d2);

                if (!((decimal)root >= (decimal)i1 && (decimal)root <= (decimal)i2 &&
                    Math.Abs(function.Evaluate(root)) < rootTolerance))
                {
                    Debug.Log(Math.Abs(function.Evaluate(root)));
                    Debug.Log("Unsatisfied Root!!!");
                    return true;
                }
            }

            return false;
        }

        protected void PlotLine(Point start, Point end, Material material)
        {
            UpdateLimits(GetRelativeElements(start));
            UpdateLimits(GetRelativeElements(end));

            LinePlotter.Plot(parent, start, end, material, lineMesh, PlotUtils.FunctionRadius());
        }

        protected void UpdateLimits(double[] elements)
        {
            UpdateLimits(elements[0], elements[1], elements[2]);
        }

        protected void UpdateLimits(double i, double d, double f)
        {
            UpdateLimit(i, d, f, maxI, i > maxI.i);
            UpdateLimit(i, d, f, minI, i < minI.i);
            UpdateLimit(i, d, f, maxD, d > maxD.d);
            UpdateLimit(i, d, f, minD, d < minD.d);
            UpdateLimit(i, d, f, maxF, f > maxF.f);
            UpdateLimit(i, d, f, minF, f < minF.f);
        }

        private void UpdateLimit(double i, double d, double f, FunctionPoint p, bool condition)
        {
            if (condition)
            {
                p.Set(i, d, f);
            }
        }

        protected Point GetPoint(double i, double d)
        {
            double x = GetAbsoluteElement(i, d, 0, Axis.x);
            double y = GetAbsoluteElement(i, d, 0, Axis.y);
            double z = GetAbsoluteElement(i, d, 0, Axis.z);

            return new Point(Limit(x), Limit(y), Limit(z));
        }

        private double Limit(double n)
        {
            if (n > 50000)
            {
                return 50000;
            }
            else if (n < -50000)
            {
                return -50000;
            }

            return n;
        }

        private void PlotAxis()
        {
            Interval iInterval = new Interval(
                minI.i != double.MaxValue ? minI.i : 0,
                maxI.i != double.MinValue ? maxI.i : 0
            );

            Interval dInterval = new Interval(
                minD.d != double.MaxValue ? minD.d : 0,
                maxD.d != double.MinValue ? maxD.d : 0
            );

            Interval fInterval = new Interval(
                minF.f != double.MaxValue ? minF.f : 0,
                maxF.f != double.MinValue ? maxF.f : 0
            );

            PlotAxis(iInterval, dInterval, fInterval);
        }

        private void PlotAxis(Interval iInterval, Interval dInterval, Interval fInterval)
        {
            xInterval = null;
            yInterval = null;
            zInterval = null;

            if (axisEnabled[Axis.x])
            {
                xInterval = GetAbsoluteElement(iInterval, dInterval, fInterval, Axis.x);
            }

            if (axisEnabled[Axis.y])
            {
                yInterval = GetAbsoluteElement(iInterval, dInterval, fInterval, Axis.y);
            }

            if (axisEnabled[Axis.z])
            {
                zInterval = GetAbsoluteElement(iInterval, dInterval, fInterval, Axis.z);
            }

            PlotUtils.SetScale(scale, xInterval, yInterval);

            AxisPlotter.Plot(parent, axisMaterial, lineMesh, PlotUtils.AxisRadius(), xInterval, yInterval,
                zInterval);
        }

        protected T GetAbsoluteElement<T>(T i, T d, T f, Axis axis)
        {
            return independent == axis ? i : dependent == axis ? d : f;
        }

        protected FunctionPoint getFunctionPoint(Point p)
        {
            double[] e = GetRelativeElements(p);

            return new FunctionPoint(e[0], e[1], e[2]);
        }

        protected double[] GetRelativeElements(Point p)
        {
            return GetRelativeElements(p.x, p.y, p.z);
        }

        protected T[] GetRelativeElements<T>(T x, T y, T z)
        {
            T i = independent == Axis.x ? x : independent == Axis.y ? y : z;
            T d = dependent == Axis.x ? x : dependent == Axis.y ? y : z;
            T f = (independent != Axis.x && dependent != Axis.x) ? x :
                (independent != Axis.y && dependent != Axis.y) ? y : z;

            return new T[] { i, d, f };
        }

        private void CreateMaterials(Material function, Material plane)
        {
            for (int c = 0; c < LineColor.Count(); c++)
            {
                functionMaterials[c] = new Material(function);
                functionMaterials[c].color = LineColor.Get(c);
            }

            for (int c = 0; c < PlaneColor.Count(); c++)
            {
                planeMaterials[c] = new Material(plane);
                planeMaterials[c].color = PlaneColor.Get(c);
            }
        }

        protected bool IsInvalid(double n)
        {
            return double.IsNaN(n) || double.IsInfinity(n);
        }
    }
}
