using GeoSlicer.DivideAndRuleSlicers;
using GeoSlicer.DivideAndRuleSlicers.OppositesIndexesGivers;
using GeoSlicer.Utils;
using GeoSlicer.Utils.Intersectors;
using GeoSlicer.Utils.Intersectors.CoordinateComparators;
using GeoSlicer.Utils.PolygonClippingAlgorithm;
using Services.GeometrySlicers.Interfaces;

namespace Services.GeometrySlicers;

public class OppositeGeometrySlicerFactory : IOppositeSlicerFactory
{
    private readonly int _maximumNumberOfPoints;
    public OppositeGeometrySlicerFactory(int maximumNumberOfPoints = -1)
    {
        _maximumNumberOfPoints = maximumNumberOfPoints;
    }

    public Slicer GetSlicer()
    {
        WeilerAthertonAlgorithm weilerAtherton = new WeilerAthertonAlgorithm(
            new LinesIntersector(
                new EpsilonCoordinateComparator(1E-9),
                new LineService(1E-10, new EpsilonCoordinateComparator(1E-10)), 1E-15),
            new LineService(1E-15, new EpsilonCoordinateComparator(1E-8)),
            new EpsilonCoordinateComparator(1E-8),
            new ContainsChecker(new LineService(1E-15, new EpsilonCoordinateComparator(1E-8)), 1E-15));
        Slicer slicer = new Slicer(_maximumNumberOfPoints,
            weilerAtherton, new ConvexityIndexesGiver(new LineService(1E-5, new EpsilonCoordinateComparator(1E-8))));
        return slicer;
    }
}