using NetTopologySuite.Geometries;
using Services.ScaleCalculator.Interfaces;

namespace Services.ScaleCalculator;

public class LinearToleranceCalculator : IToleranceCalculator
{
    private readonly double _multiplier;

    public LinearToleranceCalculator(double multiplier)
    {
        _multiplier = multiplier;
    }

    public double CalculateTolerance(Point pointLeftBottom, Point pointRightTop)
    {
        return (pointRightTop.X - pointLeftBottom.X) * _multiplier;
    }

}