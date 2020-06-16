using Assets.Scripts.EMath.Function;
using Assets.Scripts.EMath.Geometry;
using Assets.Scripts.EMath.Integral;
using Assets.Scripts.Plotter.Input;
using Assets.Scripts.Plotter.Plane;
using Assets.Scripts.Plotter.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Plotter
{
    class RevolutionSolidPlotter : FunctionPlotter
    {
        private readonly List<Point> areaPoints;
        private double volume;
        private double step;
        private Axis rotate;

        private readonly IDefiniteIntegrator integrator;

        private const double maxQuality = 8;
        private const double minQuality = 24;
        private const double qualityInterval = (minQuality - maxQuality) / (PlotUtils.qualityLevels - 1);

        public override double result => volume;

        public RevolutionSolidPlotter(IDefiniteIntegrator integrator, GameObject parent, Mesh lineMesh,
            Material axisMaterial, Material functionMaterial,
            Material planeMaterial) : base(parent, lineMesh, axisMaterial, functionMaterial, planeMaterial)
        {
            this.integrator = integrator;
            areaPoints = new List<Point>();
            axisEnabled[Axis.z] = false;
            scale = 0.7f;
        }

        protected override void BeforePlot(IPlotInput input)
        {
            ClearAreaPoints();

            this.step = maxQuality + qualityInterval * (PlotUtils.qualityLevels - input.quality);
            rotate = ((RevolutionSolidInput)input).rotate;
        }

        protected override void BeforeLinePlot(double a, double b, double i, double h)
        {
            if (!continuous)
            {
                return;
            }

            double i1 = i;
            double d1 = functions[0].Evaluate(i);
            double i2 = i + h;
            double d2 = functions[0].Evaluate(i + h);

            AddAreaPoint(i1, d1);

            if (UnsatisfiedRoot(i1, d1, i2, d2))
            {
                return;
            }

            if (IsInvalid(d2))
            {
                PlotArea();
                return;
            }

            if ((decimal)i2 >= (decimal)b)
            {
                AddAreaPoint(i2, d2);
                PlotArea();
            }
        }

        protected override void AfterPlot(double a, double b, FunctionPoint minI, FunctionPoint maxI,
                FunctionPoint minD, FunctionPoint maxD, FunctionPoint minF, FunctionPoint maxF)
        {
            CalculateVolume(a, b);
        }

        private bool UnsatisfiedRoot(double i1, double d1, double i2, double d2)
        {
            if (IsInvalid(d1) || IsInvalid(d2))
            {
                return false;
            }

            if (Math.Sign(d1) != Math.Sign(d2))
            {
                double root = EMath.Geometry.Line.Root(i1, d1, i2, d2);
                double yRoot = functions[0].Evaluate(root);

                if ((decimal)root >= (decimal)i1 && (decimal)root <= (decimal)i2 &&
                    Math.Abs(yRoot) < 0.02)
                {
                    AddAreaPoint(root, yRoot);
                    PlotArea();
                    AddAreaPoint(root, yRoot);
                }
                else
                {
                    return true;
                }
            }

            return false;
        }

        private void AddAreaPoint(double i, double d)
        {
            if (IsInvalid(d))
            {
                return;
            }

            areaPoints.Add(GetPoint(i, d));
        }

        protected void PlotArea()
        {
            if (areaPoints.Count == 0)
            {
                return;
            }

            List<Point> previous = new List<Point>(areaPoints);
            List<Point> rotated = new List<Point>();

            for (double angle = step; angle < 360; angle += step)
            {
                for (int i = 1; i < areaPoints.Count; i++)
                {
                    if (i - 1 > rotated.Count - 1)
                    {
                        rotated.Add(RotatePoint(areaPoints[i - 1], angle));
                    }

                    if (i > rotated.Count - 1)
                    {
                        rotated.Add(RotatePoint(areaPoints[i], angle));
                    }
                }

                PlotPlane(previous, rotated);

                previous = rotated.Select(p => p).ToList();
                rotated.Clear();
            }

            PlotPlane(previous, areaPoints);

            ClearAreaPoints();
        }

        private Point RotatePoint(Point point, double angle)
        {
            Point p = rotate == Axis.x ? point.Rotate(angle) : point.Rotate(0, angle);
            UpdateLimits(GetRelativeElements(p));

            return p;
        }

        private void PlotPlane(List<Point> top, List<Point> bottom)
        {
            List<Point> reversed = new List<Point>(bottom);
            reversed.Reverse();

            if (IsSamePoint(top.First(), reversed.Last()))
            {
                reversed.RemoveAt(reversed.Count - 1);
            }

            if (IsSamePoint(top.Last(), reversed.First()))
            {
                top.RemoveAt(top.Count - 1);
            }

            List<Point> points = top.Concat(reversed).ToList();

            PlanePlotter.Plot(parent, points, planeMaterials[0], PlaneView.Both);
        }

        private bool IsSamePoint(Point one, Point two)
        {
            return one.Distance(two) < 1e-13;
        }

        private void CalculateVolume(double a, double b)
        {
            if (!continuous)
            {
                PlanePlotter.ClearActions();
                volume = double.NaN;
            }
            else
            {
                IFunction vFunction;

                if (rotate == independent) // disc method
                {
                    vFunction = functions[0].Generate("(function)^2");
                    volume = Math.PI * integrator.Integrate(vFunction, a, b).value;
                }
                else // washer method
                {
                    vFunction = functions[0].Generate(independent.ToString() + "*(function)");
                    volume = 2 * Math.PI * integrator.Integrate(vFunction, a, b).value;
                }
            }
        }

        private void ClearAreaPoints()
        {
            areaPoints.Clear();
        }
    }
}
