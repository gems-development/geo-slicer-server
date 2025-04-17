using UseCases.Interfaces;
using DataAccess.Repositories.ConsoleApp.Interfaces;
using DomainModels;
using NetTopologySuite.Geometries;
using Services.GeometryCreators.Interfaces;
using Services.GeometryValidateErrors;

namespace UseCases;

public class GeometrySaver<TGeometryIn, TSliceType, TKey> : IGeometrySaver<TGeometryIn, TSliceType, TKey> where TGeometryIn : Geometry
{
    private IGeometryWithFragmentsCreator<TGeometryIn, TSliceType> GeometryWithFragmentsCreator{ get; set; }
    private IRepository<GeometryWithFragments<TGeometryIn, TSliceType>, TKey> Repository { get; set; }
    private IGeometryCorrector<TGeometryIn> GeometryCorrector { get; set; }

    public GeometrySaver(
        IGeometryWithFragmentsCreator<TGeometryIn, TSliceType> geometryWithFragmentsCreator,
        IRepository<GeometryWithFragments<TGeometryIn, TSliceType>, TKey> repository,
        IGeometryCorrector<TGeometryIn> geometryCorrector)
    {
        GeometryWithFragmentsCreator = geometryWithFragmentsCreator;
        Repository = repository;
        GeometryCorrector = geometryCorrector;
    }
        
    public TKey SaveGeometry(TGeometryIn geometry, string layerAlias, out string validateResult)
    {
        validateResult = "";
        try
        {
            GeometryValidateError[]? geometryValidateErrors = null;
                
            bool validatedGeometry = ValidateGeometry(ref geometryValidateErrors, geometry, ref validateResult);
            geometry = FixGeometry(validatedGeometry, geometry, geometryValidateErrors);

            GeometryWithFragments<TGeometryIn, TSliceType> geometryOut =
                GeometryWithFragmentsCreator.ToGeometryWithFragments(geometry);

            return Repository.Save(geometryOut, layerAlias);
        }
        catch (Exception e)
        { 
            Repository.RollbackTransaction();
            throw new Exception(validateResult + "\n" + e.Message, e);
        }
    }

    private bool ValidateGeometry(
        ref GeometryValidateError[]? geometryValidateErrors, TGeometryIn geometry, ref string result)
    {
        return GeometryCorrector.ValidateGeometry(ref geometryValidateErrors, geometry, ref result);
    }

    private TGeometryIn FixGeometry(
        bool validatedGeometry, TGeometryIn geometry, GeometryValidateError[]? geometryValidateErrors)
    {
        return GeometryCorrector.FixGeometry(validatedGeometry, geometry, geometryValidateErrors);
    }

    public void StartTransaction()
    {
        Repository.StartTransaction();
    }
        
    public void CommitTransaction()
    {
        Repository.CommitTransaction();
    }
        
    public void RollbackTransaction()
    {
        Repository.RollbackTransaction();
    }
}