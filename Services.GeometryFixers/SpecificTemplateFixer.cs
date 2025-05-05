using NetTopologySuite.Geometries;
using Services.GeometryFixers.Interfaces;

namespace Services.GeometryFixers;

// Шаблонный метод для исправления произвольной геометрии. 
// Разбирает геометрию рекурсивно, вызывая методы исправления - хуки (нужно переопределить в соответствующих реализациях)
// Для полигона вызываются методы исправления самого полигона, для его оболочки, и для его дыр
public class SpecificTemplateFixer : ISpecificFixer<Geometry>
{
    private static void SetSrid(Geometry geometryOriginal, Geometry fixedGeometry)
    {
        fixedGeometry.SRID = geometryOriginal.SRID;
    }
    
    public Geometry Fix(Geometry geometry)
    {
        return geometry switch
        {
            MultiPolygon multiPolygon => HandleMultiPolygon(multiPolygon),
            MultiLineString multiLineString => HandleMultiLineString(multiLineString),
            MultiPoint multiPoint => HandleMultiPoint(multiPoint),
            GeometryCollection geometryCollection => HandleGeometryCollection(geometryCollection),
            Polygon polygon => HandlePolygon(polygon),
            LinearRing linearRing => HandleLinearRing(linearRing),
            LineString lineString => HandleLineString(lineString),
            Point point => HandlePoint(point),
            _ => throw new NotSupportedException($"Geometry type {geometry.GetType().FullName} is not supported")
        };
    }

    private GeometryCollection HandleGeometryCollection(GeometryCollection geometryCollection)
    {
        var fixedGeometryCollection = FixGeometryCollection(geometryCollection);
        SetSrid(geometryCollection, fixedGeometryCollection);
        var res = new GeometryCollection(fixedGeometryCollection.Geometries.Select(g => Fix(g)).ToArray());
        SetSrid(geometryCollection, res);
        return res;
    }

    private MultiPolygon HandleMultiPolygon(MultiPolygon multiPolygon)
    {
        var fixedMultiPolygon = FixMultiPolygon(multiPolygon);
        SetSrid(multiPolygon, fixedMultiPolygon);
        var res = new MultiPolygon(fixedMultiPolygon.Geometries.Select(g => (Polygon) Fix(g)).ToArray());
        SetSrid(multiPolygon, res);
        return res;
    }

    private MultiLineString HandleMultiLineString(MultiLineString multiLineString)
    {
        var fixedMultiLineString = FixMultiLineString(multiLineString);
        SetSrid(multiLineString, fixedMultiLineString);
        var res = new MultiLineString(fixedMultiLineString.Geometries.Select(g => (LineString) Fix(g)).ToArray());
        SetSrid(multiLineString, res);
        return res;
    }

    private MultiPoint HandleMultiPoint(MultiPoint multiPoint)
    {
        var fixedMultiPoint = FixMultiPoint(multiPoint);
        SetSrid(multiPoint, fixedMultiPoint);
        var res = new MultiPoint(fixedMultiPoint.Geometries.Select(g => (Point) Fix(g)).ToArray());
        SetSrid(multiPoint, res);
        return res;
    }

    private Polygon HandlePolygon(Polygon polygon)
    {
        var fixedPolygon = FixPolygon(polygon);
        SetSrid(polygon, fixedPolygon);
        var fixedShell = (LinearRing) Fix(fixedPolygon.Shell);
        var fixedHoles = fixedPolygon.Holes.Select(h => (LinearRing) Fix(h)).ToArray();
        var res = new Polygon(fixedShell, fixedHoles);
        SetSrid(polygon, res);
        return res;
    }

    private LineString HandleLineString(LineString lineString)
    {
        var fixedLineString = FixLineString(lineString);
        SetSrid(lineString, fixedLineString);
        return fixedLineString;
    }

    private LinearRing HandleLinearRing(LinearRing linearRing)
    {
        var fixedLinearRing = FixLinearRing(linearRing);
        SetSrid(linearRing, fixedLinearRing);
        return fixedLinearRing;
    }

    private Point HandlePoint(Point point)
    {
        var fixedPoint = FixPoint(point);
        SetSrid(point, fixedPoint);
        return fixedPoint;
    }
    
    protected virtual GeometryCollection FixGeometryCollection(GeometryCollection geometryCollection)
    {
        return geometryCollection;
    }
    
    protected virtual MultiPolygon FixMultiPolygon(MultiPolygon multiPolygon)
    {
        return multiPolygon;
    }
    
    protected virtual MultiLineString FixMultiLineString(MultiLineString multiLineString)
    {
        return multiLineString;
    }
    
    protected virtual MultiPoint FixMultiPoint(MultiPoint multiPoint)
    {
        return multiPoint;
    }
    
    protected virtual Polygon FixPolygon(Polygon polygon)
    {
        return polygon;
    }
    
    protected virtual LineString FixLineString(LineString lineString)
    {
        return lineString;
    }
    
    protected virtual LinearRing FixLinearRing(LinearRing linearRing)
    {
        return linearRing;
    }
    
    protected virtual Point FixPoint(Point point)
    {
        return point;
    }
}