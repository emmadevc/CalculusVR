using Assets.Scripts.EMath.Geometry;
using Assets.Scripts.Plotter.Line;
using Assets.Scripts.Plotter.Plane;
using Assets.Scripts.Plotter.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlaneTestPlotter : MonoBehaviour
{
    public GameObject parent;
    public Material planeMaterial;
    public Material lineMaterial;
    public Mesh lineMesh;

    private void Start()
    {
        List<Point> points = new List<Point>();

        points.Add(new Point(0, 1, 1));
        points.Add(new Point(1, 0, 1));
        points.Add(new Point(2, -1, 1));
        points.Add(new Point(3, -1, 1));
        points.Add(new Point(4, 0, 1));
        points.Add(new Point(5, 1, 1));

        points.Add(new Point(5, 1, 0));
        points.Add(new Point(4, 0, 0));
        points.Add(new Point(3, -1, 0));
        points.Add(new Point(2, -1, 0));
        points.Add(new Point(1, 0, 0));
        points.Add(new Point(0, 1, 0));

        Plot(points);

        points.Clear();

        points.Add(new Point(0, 1, -1.5));

        points.Add(new Point(1, 0, -1));
        points.Add(new Point(2, -1, -1));
        points.Add(new Point(3, -1, -1));
        points.Add(new Point(4, 0, -1));

        points.Add(new Point(5, 1, -1.5));
        
        points.Add(new Point(4, 0, -2));
        points.Add(new Point(3, -1, -2));
        points.Add(new Point(2, -1, -2));
        points.Add(new Point(1, 0, -2));

        Plot(points);

        PlanePlotter.PlotActions();
        LinePlotter.PlotActions();
    }

    private void Plot(List<Point> points)
    {
        PlanePlotter.Plot(parent, points, planeMaterial, PlaneView.Top);
        PlotBorder(points);
    }

    private void PlotBorder(List<Point> points)
    {
        for (int i = 1; i < points.Count; i++)
        {
            LinePlotter.Plot(parent, points[i - 1], points[i], lineMaterial, lineMesh, PlotUtils.FunctionRadius());
        }

        LinePlotter.Plot(parent, points.Last(), points.First(), lineMaterial, lineMesh, PlotUtils.FunctionRadius());
    }
}
