using NetTopologySuite.Geometries;
using Services.GeometryValidateErrors;
using Services.GeometryValidators.Interfaces;

namespace Services.GeometryValidators;

public class SpecificTemplateValidator : ISpecificValidator<Geometry>
{
    protected static readonly GeometryValidateError GeometryValid = 
        new GeometryValidateError(GeometryValidateErrorType.GeometryValid, "no errors were found");
    
    public GeometryValidateError ValidateGeometry(Geometry geometry)
    {
        return geometry switch
        {
            MultiPolygon multiPolygon => HandleGeometryCollection(multiPolygon, ValidateMultiPolygon),
            MultiLineString multiLineString => HandleGeometryCollection(multiLineString, ValidateMultiLineString),
            MultiPoint multiPoint => HandleGeometryCollection(multiPoint, ValidateMultiPoint),
            GeometryCollection geometryCollection => HandleGeometryCollection(geometryCollection, ValidateGeometryCollection),
            Polygon polygon => HandlePolygon(polygon),
            LinearRing linearRing => ValidateLinearRing(linearRing),
            LineString lineString => ValidateLineString(lineString),
            Point point => ValidatePoint(point),
            _ => throw new NotSupportedException($"Geometry type {geometry.GetType().FullName} is not supported")
        };
    }

    private GeometryValidateError HandleGeometryCollection<T>(T geometryCollection, Func<T, GeometryValidateError> validateFunc) 
        where T : GeometryCollection
    {
        var error = validateFunc(geometryCollection);
        if (error.Type != GeometryValidateErrorType.GeometryValid)
            return error;
        
        foreach (var geometry in geometryCollection.Geometries)
        {
            error = ValidateGeometry(geometry);
            if (error.Type != GeometryValidateErrorType.GeometryValid)
                return error;
        }

        return error;
    }

    private GeometryValidateError HandlePolygon(Polygon polygon)
    {
        var error = ValidatePolygon(polygon);
        if (error.Type != GeometryValidateErrorType.GeometryValid)
            return error;
        error = ValidateGeometry(polygon.Shell);
        if (error.Type != GeometryValidateErrorType.GeometryValid)
            return error;
       
        foreach (var hole in polygon.Holes)
        {
            error = ValidateGeometry(hole);
            if (error.Type != GeometryValidateErrorType.GeometryValid)
                return error;
        }

        return error;
    }
    
    protected virtual GeometryValidateError ValidateGeometryCollection(GeometryCollection geometryCollection)
    {
        return GeometryValid;
    }
    
    protected virtual GeometryValidateError ValidateMultiPolygon(MultiPolygon multiPolygon)
    {
        return GeometryValid;
    }
    
    protected virtual GeometryValidateError ValidateMultiLineString(MultiLineString multiLineString)
    {
        return GeometryValid;
    }
    
    protected virtual GeometryValidateError ValidateMultiPoint(MultiPoint multiPoint)
    {
        return GeometryValid;
    }
    
    protected virtual GeometryValidateError ValidatePolygon(Polygon polygon)
    {
        return GeometryValid;
    }
    
    protected virtual GeometryValidateError ValidateLineString(LineString lineString)
    {
        return GeometryValid;
    }
    
    protected virtual GeometryValidateError ValidateLinearRing(LinearRing linearRing)
    {
        return GeometryValid;
    }
    
    protected virtual GeometryValidateError ValidatePoint(Point point)
    {
        return GeometryValid;
    }
}