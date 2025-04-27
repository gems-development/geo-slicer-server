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
        var res =
            geometryCollection.Geometries.Select(ToGeometryWithFragments).ToArray();
        var newGeometryCollection = new GeometryCollection(
            res.Select(g => g.Data).ToArray());
        SetSrid(geometryCollection, newGeometryCollection);
        var fragments = res
                .Select(g => g.GeometryFragments)
                .SelectMany(x => x)
                .ToArray();
        return new GeometryWithFragments<Geometry, FragmentWithNonRenderingBorder<Geometry, Geometry>>(
            newGeometryCollection, fragments);
    }
    
    private GeometryWithFragments<Geometry, FragmentWithNonRenderingBorder<Geometry, Geometry>>
        HandleMultiPolygon(MultiPolygon multiPolygon)
    {
        var res =
            multiPolygon.Geometries.Select(a => SlicePolygon((Polygon) a)).ToArray();
        foreach (var fragment in res)
        {
            foreach (var fr in fragment.GeometryFragments)
            {
                //Console.WriteLine(fr.Fragment.SRID);
            }
        }
        var newMultiPolygon = new MultiPolygon(
            res.Select(g => g.Data).ToArray());
        SetSrid(multiPolygon, newMultiPolygon);
        var fragments = res
            .Select(g => g.GeometryFragments)
            .SelectMany(x => x)
            .Select(g => 
                new FragmentWithNonRenderingBorder<Geometry, Geometry>(g.Fragment, g.NonRenderingBorder))
            .ToArray();
        return new GeometryWithFragments<Geometry, FragmentWithNonRenderingBorder<Geometry, Geometry>>(
            newMultiPolygon, fragments);
    }
    
    private GeometryWithFragments<Geometry, FragmentWithNonRenderingBorder<Geometry, Geometry>>
        CreateGeometryWithFragments<T>(T geometry, Geometry emptyGeometry) where T : Geometry 
    {
        SetSrid(geometry, emptyGeometry);
        return new GeometryWithFragments<Geometry, FragmentWithNonRenderingBorder<Geometry, Geometry>>(geometry,
            [new FragmentWithNonRenderingBorder<Geometry, Geometry>(geometry, emptyGeometry)]);
    }

    private GeometryWithFragments<Polygon, FragmentWithNonRenderingBorder<Polygon, MultiLineString>> SlicePolygon(Polygon polygon)
    {
        var slicedPolygon = _polygonSlicer.Slice(polygon).ToArray();
        foreach (var fragment in slicedPolygon)
        {
            //Console.WriteLine(fragment.Fragment.SRID);
            SetSrid(polygon, fragment.Fragment);
            SetSrid(polygon, fragment.NonRenderingBorder);
        }
        
        foreach (var fragment in slicedPolygon)
        {
            Console.WriteLine(fragment.Fragment.SRID);
            Console.WriteLine(fragment.NonRenderingBorder.SRID);
        }

        return new GeometryWithFragments<Polygon, FragmentWithNonRenderingBorder<Polygon, MultiLineString>>(
            polygon,
            slicedPolygon);
    }

    private GeometryWithFragments<Geometry, FragmentWithNonRenderingBorder<Geometry, Geometry>> HandlePolygon(Polygon polygon)
    {
        var slicedPolygon = SlicePolygon(polygon);
        return new GeometryWithFragments<Geometry, FragmentWithNonRenderingBorder<Geometry, Geometry>>(
            slicedPolygon.Data,
            slicedPolygon.GeometryFragments
                .Select(g =>
                    new FragmentWithNonRenderingBorder<Geometry, Geometry>(
                        g.Fragment,
                        g.NonRenderingBorder
                    ))
                .ToArray()
        );
    }
}