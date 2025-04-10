using GeoSlicer.DivideAndRuleSlicers.OppositesSlicer;
using GeoSlicer.Utils;
using GeoSlicer.Utils.Intersectors;
using GeoSlicer.Utils.Intersectors.CoordinateComparators;
using GeoSlicer.Utils.PolygonClippingAlghorithm;
using Microsoft.Extensions.DependencyInjection;

namespace GeoSlicer.Config;

public static class GeoSlicerExtension
{
    public static void AddAlgorithms(
        this IServiceCollection serviceCollection, double comparatorEpsilon, double epsilon, int maximumNumberOfPoints = -1)
    {
        var coordinateComparator = new EpsilonCoordinateComparator(comparatorEpsilon);
        LineService lineService = new LineService(epsilon, coordinateComparator);
        WeilerAthertonAlghorithm weilerAthertonAlghorithm = new WeilerAthertonAlghorithm(
            new LinesIntersector(coordinateComparator, lineService, epsilon), lineService,
            coordinateComparator, new ContainsChecker(lineService, epsilon), epsilon);
            
        if (maximumNumberOfPoints > 0)
            serviceCollection
                .AddTransient<Slicer>(provider => new Slicer(lineService, maximumNumberOfPoints, weilerAthertonAlghorithm));
    }
}