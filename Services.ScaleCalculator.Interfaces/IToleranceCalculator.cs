using NetTopologySuite.Geometries;

namespace Services.ScaleCalculator.Interfaces;

public interface IToleranceCalculator
{
    double CalculateTolerance(Point pointLeftBottom, Point pointRightTop);
}