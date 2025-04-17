using NetTopologySuite.Geometries;

namespace Services.ScaleCalculator.Interfaces;

public class ArrayToleranceCalculator : IToleranceCalculator
{
    private readonly double[] _tolerances = [0, 4E-5, 7.27E-5, 2E-4, 5E-4, 5.33E-4];
    
    public double CalculateTolerance(Point pointLeftBottom, Point pointRightTop)
    {
        return _tolerances[GetIndex(pointRightTop.X - pointLeftBottom.X)];
    }

    private int GetIndex(double dx)
    {
        return Math.Clamp((int)Math.Floor(Math.Log2(dx / 0.05)) + 1, 0, _tolerances.Length - 1);
    }
}