using DomainModels;
using NetTopologySuite.Geometries;
using Services.GeometryCreators.Interfaces;
using Services.GeometrySlicers.Interfaces;

namespace Services.GeometryCreators;

public class GeometryWithFragmentsCreator : IGeometryWithFragmentsCreator<Geometry, FragmentWithNonRenderingBorder<Geometry, Geometry>>
{
    private IGeometrySlicer<Polygon, FragmentWithNonRenderingBorder<Polygon, MultiLineString>> _polygonSlicer;

    public GeometryWithFragmentsCreator(IGeometrySlicer<Polygon, FragmentWithNonRenderingBorder<Polygon, MultiLineString>> polygonSlicer)
    {
        _polygonSlicer = polygonSlicer;
    }

    private static void SetSrid(Geometry geometryOriginal, Geometry fixedGeometry)
    {
        fixedGeometry.SRID = geometryOriginal.SRID;
    }
    
    public GeometryWithFragments<Geometry, FragmentWithNonRenderingBorder<Geometry, Geometry>> ToGeometryWithFragments(Geometry geometry)
    {
        return geometry switch
        {
            MultiPolygon multiPolygon => HandleMultiPolygon(multiPolygon),
            MultiLineString multiLineString => CreateGeometryWithFragments(multiLineString, MultiLineString.Empty),
            MultiPoint multiPoint => CreateGeometryWithFragments(multiPoint, MultiPoint.Empty),
            GeometryCollection geometryCollection => HandleGeometryCollection(geometryCollection),
            Polygon polygon => HandlePolygon(polygon),
            LinearRing linearRing => CreateGeometryWithFragments(linearRing, new LinearRing([])),
            LineString lineString => CreateGeometryWithFragments(lineString, LineString.Empty),
            Point point => CreateGeometryWithFragments(point, Point.Empty),
            _ => throw new NotSupportedException($"Geometry type {geometry.GetType().FullName} is not supported")
        };
    }

    private GeometryWithFragments<Geometry, FragmentWithNonRenderingBorder<Geometry, Geometry>>
        HandleGeometryCollection(GeometryCollection geometryCollection)
    {
        var fragments = geometryCollection.Geometries
                .Select(ToGeometryWithFragments)
                .Select(g => g.GeometryFragments)
                .SelectMany(x => x)
                .ToArray();
        return new GeometryWithFragments<Geometry, FragmentWithNonRenderingBorder<Geometry, Geometry>>(
            geometryCollection, fragments);
    }
    
    private GeometryWithFragments<Geometry, FragmentWithNonRenderingBorder<Geometry, Geometry>>
        HandleMultiPolygon(MultiPolygon multiPolygon)
    {
        var fragments = multiPolygon.Geometries
            .Select(a => HandlePolygon((Polygon) a))
            .Select(g => g.GeometryFragments)
            .SelectMany(x => x)
            .ToArray();
        return new GeometryWithFragments<Geometry, FragmentWithNonRenderingBorder<Geometry, Geometry>>(
            multiPolygon, fragments);
    }
    
    private GeometryWithFragments<Geometry, FragmentWithNonRenderingBorder<Geometry, Geometry>>
        CreateGeometryWithFragments<T>(T geometry, Geometry emptyGeometry) where T : Geometry 
    {
        SetSrid(geometry, emptyGeometry);
        return new GeometryWithFragments<Geometry, FragmentWithNonRenderingBorder<Geometry, Geometry>>(geometry,
            [new FragmentWithNonRenderingBorder<Geometry, Geometry>(geometry, emptyGeometry)]);
    }

    private GeometryWithFragments<Geometry, FragmentWithNonRenderingBorder<Geometry, Geometry>> HandlePolygon(Polygon polygon)
    {
        var slicedPolygon = _polygonSlicer
            .Slice(polygon)
            .Select(a => { 
                SetSrid(polygon, a.Fragment); 
                SetSrid(polygon, a.NonRenderingBorder); 
                return a; 
            })
            .Select(a =>
                new FragmentWithNonRenderingBorder<Geometry, Geometry>(a.Fragment, a.NonRenderingBorder))
            .ToArray();

        return new GeometryWithFragments<Geometry, FragmentWithNonRenderingBorder<Geometry, Geometry>>(
            polygon,
            slicedPolygon);
    }
}