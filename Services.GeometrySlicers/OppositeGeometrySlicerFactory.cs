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
    private readonly double _comparatorEpsilon;
    private readonly double _epsilon;
    private readonly int _maximumNumberOfPoints;
    public OppositeGeometrySlicerFactory(double comparatorEpsilon, double epsilon, int maximumNumberOfPoints = -1)
    {
        _comparatorEpsilon = comparatorEpsilon;
        _epsilon = epsilon;
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
        
        
        // var coordinateComparator = new EpsilonCoordinateComparator(_comparatorEpsilon);
        // LineService lineService = new LineService(_epsilon, coordinateComparator);
        // WeilerAthertonAlgorithm weilerAthertonAlgorithm = new WeilerAthertonAlgorithm(
        //     new LinesIntersector(coordinateComparator, lineService, _epsilon), lineService,
        //     coordinateComparator, new ContainsChecker(lineService, _epsilon));
        // return new Slicer(_maximumNumberOfPoints, weilerAthertonAlgorithm, new ConvexityIndexesGiver(lineService));
    }
}