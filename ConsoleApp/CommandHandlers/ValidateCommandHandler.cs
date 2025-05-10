using ConsoleApp.Builders;
using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite.Geometries;
using Services.GeometryValidateErrors;
using UseCases.Interfaces;
using Utils;

namespace ConsoleApp.CommandHandlers;

internal static class ValidateCommandHandler
{
    internal static void Handle(IEnumerable<FileInfo> files, int srid)
    {
        IServiceCollection serviceCollection = new ServiceCollection();
        using var geometryCorrectorProvider = serviceCollection.BuildGeometryCorrectorProvider(Epsilons.EpsilonCoordinateComparator);
        var correctionService = geometryCorrectorProvider.GetService<IGeometryCorrector<Geometry>>();
        if (correctionService == null)
        {
            throw new NullReferenceException("Correction service is null");
        }
        
        foreach (var o in files)
        {
            string errors = "";
            GeometryValidateErrorType[]? geometryValidateErrors = null;
            try
            {
                var featureCollection = new GeometryReader().ReadGeometriesFromFile(o.FullName);
                foreach (var feature in featureCollection)
                {
                    var geometry = feature.Geometry;
                    geometry.SRID = srid;
                    correctionService.ValidateGeometry(ref geometryValidateErrors, geometry, ref errors);
                }
            }
            catch (Exception e)
            {
                throw new Exception(o.FullName + ":" + "\n" + e.Message, e);
            }
            Console.WriteLine(o.FullName + ":" + "\n" + errors);
        }
    }
}