using GeoSlicer.DivideAndRuleSlicers.OppositesSlicer;
using GeoSlicer.Utils;
using GeoSlicer.Utils.Intersectors;
using GeoSlicer.Utils.Intersectors.CoordinateComparators;
using GeoSlicer.Utils.PolygonClippingAlghorithm;
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
        var coordinateComparator = new EpsilonCoordinateComparator(_comparatorEpsilon);
        LineService lineService = new LineService(_epsilon, coordinateComparator);
        WeilerAthertonAlghorithm weilerAthertonAlghorithm = new WeilerAthertonAlghorithm(
            new LinesIntersector(coordinateComparator, lineService, _epsilon), lineService,
            coordinateComparator, new ContainsChecker(lineService, _epsilon), _epsilon);
        return new Slicer(lineService, _maximumNumberOfPoints, weilerAthertonAlghorithm);
    }
}