using GeoSlicer.NonConvexSlicer;
using GeoSlicer.NonConvexSlicer.Helpers;
using GeoSlicer.Utils;
using GeoSlicer.Utils.Intersectors;
using GeoSlicer.Utils.Intersectors.CoordinateComparators;
using NetTopologySuite.Geometries;
using Services.GeometrySlicers.Interfaces.GeoSlicerDetails;

namespace Services.GeometrySlicers.GeoSlicerDetails;

public class NonConvexSlicerFactory : INonConvexSlicerFactory
{
    public Slicer GetSlicer()
    {
        double epsilon = 1E-15;

        GeometryFactory gf =
            NetTopologySuite.NtsGeometryServices.Instance.CreateGeometryFactory(0);

        LineService lineService = new LineService(epsilon, new EpsilonCoordinateComparator(epsilon));
        SegmentService segmentService = new SegmentService(lineService);

        return new(gf,
                segmentService,
                new NonConvexSlicerHelper(
                    new LinesIntersector(new EpsilonCoordinateComparator(epsilon), lineService, epsilon), lineService));
    }
}