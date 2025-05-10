using ConsoleApp.Builders;
using DomainModels;
using GeometrySlicerTypes;
using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using Newtonsoft.Json;
using UseCases.Interfaces;
using Utils;

namespace ConsoleApp.CommandHandlers;

internal static class SaveCommandHandler
{
    internal static void Handle(string connectionString, IEnumerable<FileInfo> files, string layerAlias, int srid, GeometrySlicerType type, int? points)
    {
        IServiceCollection serviceCollection = new ServiceCollection();
        using var geometrySaverProvider =
            serviceCollection.BuildGeometrySaverServiceProvider(connectionString, points,
                Epsilons.EpsilonCoordinateComparator, Epsilons.Epsilon, type);
        var geometrySaver = geometrySaverProvider
            .GetService<
                IGeometrySaver<Geometry, FragmentWithNonRenderingBorder<Geometry, Geometry>, int>>();
        if (geometrySaver == null)
        {
            throw new NullReferenceException("Geometry controller is null");
        }
        
        geometrySaver.StartTransaction();
        foreach (var o in files)
        {
            string errors = "";
            try
            {
                var featureCollection = new GeometryReader().ReadGeometriesFromFile(o.FullName);
                foreach (var feature in featureCollection)
                {
                    feature.Geometry.SRID = srid;
                    geometrySaver.SaveGeometry(feature.Geometry, layerAlias, GetAttributesAsJson(feature), out errors);
                }
            }
            catch (Exception e)
            {
                geometrySaver.RollbackTransaction();
                throw new Exception(o.FullName + ":" + "\n" + e.Message, e);
            }
            Console.WriteLine(o.FullName + ":" + "\n" + errors);
        }
        geometrySaver.CommitTransaction();
    }
    
    private static string GetAttributesAsJson(IFeature feature)
    {
        if (feature.Attributes == null)
            return "{}"; 
        
        var attributes = feature.Attributes.GetNames()
            .ToDictionary(name => name, name => feature.Attributes[name]);
        
        return JsonConvert.SerializeObject(attributes, Formatting.Indented);
    }
}