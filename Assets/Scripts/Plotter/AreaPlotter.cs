using Assets.Scripts.EMath.Function;
using Assets.Scripts.EMath.Geometry;
using Assets.Scripts.EMath.Integral;
using Assets.Scripts.Plotter.Plane;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Assets.Scripts.Plotter.Input;

namespace Assets.Scripts.Plotter
{
    class AreaPlotter : FunctionPlotter
    {
        private readonly List<Point> topAreaPoints;
        private readonly List<Point> bottomAreaPoints;
        private readonly OrderedFunctions ordered;
        private double area;

        private readonly IDefiniteIntegrator integrator;

        public override double result => area;

        public AreaPlotter(IDefiniteIntegrator integrator, GameObject parent, Mesh lineMesh,
            Material axisMaterial, Material functionMaterial,
            Material planeMaterial) : base(parent, lineMesh, axisMaterial, functionMaterial, planeMaterial)
        {
            this.topAreaPoints = new List<Point>();
            this.bottomAreaPoints = new List<Point>();
            this.ordered = new OrderedFunctions();
            this.integrator = integrator;

            axisEnabled[Axis.z] = false;
            scale = 1.5f;
        }

        protected override void BeforePlot(IPlotInput input)
        {
            ClearAreaPoints();

            ordered.Set(functions);
            area = 0.0;
        }

        protected override void BeforeLinePlot(double a, double b, double i, double h)
        {
            if (functions.Length > 1)
            {
                Evaluate(b, i, i + h);
            }
            else
            {
                Evaluate(b, i, functions[0].Evaluate(i), i + h, functions[0].Evaluate(i + h));
            }
        }

        protected override void AfterPlot(double a, double b, FunctionPoint minI, FunctionPoint maxI,
                FunctionPoint minD, FunctionPoint maxD, FunctionPoint minF, FunctionPoint maxF)
        {
            if (!closed)
            {
                area = double.NaN;
            }
        }

        private void Evaluate(double b, double i1, double i2)
        {
            AddAreaPoints(i1, b);
            EvaluateIntersection(i1, i2);

            if ((decimal)i2 >= (decimal)b)
            {
                AddAreaPoints(i2, b);
                PlotArea();
            }
        }

        private void AddAreaPoints(double i, double b)
        {
            double dt = ordered.Evaluate(i, 0);
            double db = ordered.Evaluate(i, 1);

            if (IsInvalid(dt) || IsInvalid(db))
            {
                PlotArea();
                return;
            }

            AddTopAreaPoint(i, dt);

            if ((decimal)dt != (decimal)db)
            {
                AddBottomAreaPoint(i, db);
            }
        }

        private void EvaluateIntersection(double i1, double i2)
        {
            if (ordered.Index(i1, 0) != ordered.Index(i2, 0))
            {
                int ia = ordered.Index(i1, 1);
                int ib = ordered.Index(i1, 0);

                double da1 = functions[ia].Evaluate(i1);
                double db1 = functions[ib].Evaluate(i1);

                double da2 = functions[ia].Evaluate(i2);
                double db2 = functions[ib].Evaluate(i2);

                if (IsInvalid(da1) || IsInvalid(db1) || IsInvalid(da2) || IsInvalid(db2))
                {
                    PlotArea();
                    return;
                }

                FunctionPoint root = FunctionLine.Intersection(i1, da1, db1, i2, da2, db2);

                if ((decimal)root.i >= (decimal)i1 && (decimal)root.i <= (decimal)i2)
                {
                    AddTopAreaPoint(root);
                }

                PlotArea();

                if ((decimal)root.i < (decimal)i2)
                {
                    AddTopAreaPoint(root.i, root.d);
                }
            }
        }

        private void Evaluate(double b, double i1, double d1, double i2, double d2)
        {
            AddAreaPoint(i1, d1);
            EvaluateRoot(i1, d1, i2, d2);

            if (IsInvalid(d2))
            {
                PlotArea(i1);
                return;
            }

            if ((decimal)i2 >= (decimal)b)
            {
                AddAreaPoint(i2, d2);
                PlotArea(i2);
            }
        }

        private void AddAreaPoint(double i, double d)
        {
            if (IsInvalid(d))
            {
                return;
            }

            if (topAreaPoints.Count == 0)
            {
                AddTopAreaPoint(i, 0);

                if (Math.Sign(d) != 0)
                {
                    AddTopAreaPoint(i, d);
                }
            }
            else
            {
                AddTopAreaPoint(i, d);
            }
        }

        private void EvaluateRoot(double i1, double d1, double i2, double d2)
        {
            if (IsInvalid(d1) || IsInvalid(d2))
            {
                return;
            }

            if (Math.Sign(d1) != Math.Sign(d2))
            {
                double root = EMath.Geometry.Line.Root(i1, d1, i2, d2);

                if ((decimal)root >= (decimal)i1 && (decimal)root <= (decimal)i2)
                {
                    PlotArea(root);
                    AddTopAreaPoint(root, 0);
                }
                else
                {
                    PlotArea();
                }
            }
        }

        private void AddTopAreaPoint(double i, double d)
        {
            topAreaPoints.Add(GetPoint(i, d));
        }

        private void AddBottomAreaPoint(double i, double d)
        {
            bottomAreaPoints.Add(GetPoint(i, d));
        }

        private void AddTopAreaPoint(FunctionPoint fp)
        {
            topAreaPoints.Add(fp.ToPoint(independent, dependent));
        }

        private void AddBottomAreaPoint(FunctionPoint fp)
        {
            bottomAreaPoints.Add(fp.ToPoint(independent, dependent));
        }

        private void PlotArea(double i)
        {
            if (topAreaPoints.Count > 0)
            {
                AddTopAreaPoint(i, 0);
                PlotArea();
            }
        }

        private void PlotArea()
        {
            bottomAreaPoints.Reverse();

            List<Point> areaPoints = topAreaPoints.Concat(bottomAreaPoints).ToList();

            if (areaPoints.Count == 0)
            {
                return;
            }

            PlanePlotter.Plot(parent, areaPoints, planeMaterials[0], PlaneView.Both);

            CalculateArea();

            ClearAreaPoints();
        }

        private void ClearAreaPoints()
        {
            topAreaPoints.Clear();
            bottomAreaPoints.Clear();
        }

        private void CalculateArea()
        {
            if (!closed)
            {
                return;
            }

            double a = getFunctionPoint(topAreaPoints.First()).i;
            double b = getFunctionPoint(topAreaPoints.Last()).i;

            double intervalArea = Integrate(ordered.Index(a, 0), a, b);

            for (int c = 1; c < functions.Length; c++)
            {
                intervalArea -= Integrate(ordered.Index(a, c), a, b);
            }

            area += intervalArea;
        }

        private double Integrate(int index, double a, double b)
        {
            IFunction function = functions[index];

            if (!IsInvalid(function.Evaluate(a)) && !IsInvalid(function.Evaluate(b)))
            {
                return integrator.Integrate(function, a, b).value;
            }

            return 0;
        }
    }
}
