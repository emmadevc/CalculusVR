using Assets.Scripts.EMath.Geometry;
using Assets.Scripts.EMath.Set;
using Assets.Scripts.Plotter.Cone;
using Assets.Scripts.Plotter.Label;
using Assets.Scripts.Plotter.Line;
using Assets.Scripts.Plotter.Utils;
using System;
using UnityEngine;

namespace Assets.Scripts.Plotter.Axes
{
    class AxisPlotter
    {
        private const double markProportion = 0.18;
        private const double labelHeight = 0.47;
        private const double labelMargin = 0.08;
        private const double arrowHeight = 0.4;
        private const float arrowBaseScale = 0.6f;
        private const float arrowPointScale = 0.01f;

        public static void Plot(GameObject parent, Material material, Mesh mesh, float radius,
            Interval xInterval, Interval yInterval, Interval zInterval)
        {
            if (xInterval != null)
            {
                PlotAxis(parent, material, mesh, radius, xInterval, Axis.x);
            }

            if (yInterval != null)
            {
                PlotAxis(parent, material, mesh, radius, yInterval, Axis.y);
            }

            if (zInterval != null)
            {
                PlotAxis(parent, material, mesh, radius, zInterval, Axis.z);
            }
        }

        private static void PlotAxis(GameObject parent, Material material, Mesh mesh, float radius,
            Interval i, Axis axis)
        {
            double proportion = 0.15 / PlotUtils.scale;
            double step = GetStep(i, proportion);

            Interval interval = AxisInterval(i, step);

            Point start = GetPoint(interval.a, axis);
            Point end = GetPoint(interval.b, axis);

            LinePlotter.Plot(parent, start, end, material, mesh, radius);

            ConePlotter.Plot(parent, start, GetPoint(interval.a - (arrowHeight * proportion), axis),
                arrowBaseScale, arrowPointScale, material);

            ConePlotter.Plot(parent, end, GetPoint(interval.b + (arrowHeight * proportion), axis),
                arrowBaseScale, arrowPointScale, material);

            PlotMarks(parent, material, mesh, radius, interval.a, interval.b, step,
                proportion, axis);
        }

        private static Interval AxisInterval(Interval interval, double step)
        {
            double start = interval.a > -step ? -step : interval.a;
            double end = step > interval.b ? step : interval.b;

            start += start >= 0 ? step : -step;
            end += end >= 0 ? step : -step;

            return new Interval(start, end);
        }

        private static Point GetPoint(double v, Axis axis)
        {
            switch (axis)
            {
                case Axis.x:
                    return new Point(v, 0);
                case Axis.y:
                    return new Point(0, v);
            }

            return new Point(0, 0, v);
        }

        private static void PlotMarks(GameObject parent, Material material, Mesh mesh, float radius,
            double markStart, double markEnd, double step, double proportion, Axis axis)
        {
            double min = step / 2;

            Debug.Log(markStart);
            Debug.Log(step);
            Debug.Log(markEnd);

            for (double i = markStart + step; i < markEnd; i += step)
            {
                if (i != 0 && (markEnd - i) > min)
                {
                    LinePlotter.Plot(parent, MarkPoint(i, axis, proportion, true),
                        MarkPoint(i, axis, proportion, false), material, mesh, radius);

                    PlotLabel(parent, material, axis, proportion, i);
                }
            }
        }

        // TODO: This method is a mess :)
        private static double GetStep(Interval interval, double proportion)
        {
            double step = CalculateStep(interval);

            if (proportion > 1.0)
            {
                step = Math.Ceiling(step * 1.8);

                if (step >= 1.0)
                {
                    step = (int)step;
                }
            }

            return step;
        }

        // TODO: This method is a mess :)
        private static double CalculateStep(Interval interval)
        {
            double distance = interval.Distance();

            if (distance < 1.0)
            {
                return distance / 5;
            }

            int diff = (int)distance;

            if (diff <= 10)
            {
                return 1;
            }

            int length = diff.ToString().Length;
            int numBase = int.Parse("1".PadRight(length, '0'));

            if (length == 2)
            {
                return (int)Math.Ceiling(diff / (double)numBase);
            }

            int cont = 9 - length;

            return (int)Math.Ceiling(diff / (double)numBase) * numBase / (cont > 0 ? cont : 1);
        }

        private static Point MarkPoint(double point, Axis axis, double proportion, bool start)
        {
            double direction = start ? -1 : 1;

            switch (axis)
            {
                case Axis.x:
                    return new Point(point, markProportion * proportion * direction);
                case Axis.y:
                    return new Point(markProportion * proportion * direction, point);
            }

            return new Point(0, markProportion * direction, point);
        }

        private static void PlotLabel(GameObject parent, Material material, Axis axis,
            double proportion, double point)
        {
            Alignment alignment = axis == Axis.y ? Alignment.Right : Alignment.Center;
            double dec = Math.Abs(point - (int)point);
            string message = dec > 0.0 && Math.Abs(point) < 1.0? point.ToString("0.##") : ((int)point).ToString();

            double[] depths = new double[] { -0.001, 0.001 };

            foreach (double depth in depths)
            {
                LabelPlotter.Plot(parent, MarkLabelPoint(point, depth, proportion, axis, true),
                    MarkLabelPoint(point, depth, proportion, axis, false), message, material.color, alignment);
            }
        }

        private static Point MarkLabelPoint(double point, double depth, double proportion, Axis axis, bool start)
        {
            double pos = -(markProportion + labelMargin + (axis == Axis.y ? 0 : labelHeight * 0.5f)) * proportion;
            pos += start ? 0 : labelHeight * proportion;

            switch (axis)
            {
                case Axis.x:
                    return new Point(point, pos, depth);
                case Axis.y:
                    return new Point(pos, point, depth);
            }

            return new Point(0, pos, point);
        }

        private static double Floor(double number)
        {
            double abs = Math.Abs(number);

            if (number >= 0.0)
            {
                return ((abs - (int)abs) > 0.2) ? Math.Ceiling(number) : Math.Floor(number);
            }

            return ((abs - (int)abs) > 0.2) ? Math.Floor(number) : Math.Ceiling(number);
        }

        private static double Ceiling(double number)
        {
            double abs = Math.Abs(number);

            if (number >= 0.0)
            {
                return ((abs - (int)abs) < 0.2) ? Math.Floor(number) : Math.Ceiling(number);
            }

            return ((abs - (int)abs) < 0.2) ? Math.Ceiling(number) : Math.Floor(number);
        }
    }
}
