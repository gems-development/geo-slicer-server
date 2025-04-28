using System.CommandLine;
using UseCases;
using UseCases.Interfaces;
using DataAccess.PostgreSql;
using DataAccess.Repositories.ConsoleApp;
using DomainModels;
using GeometrySlicerTypes;
using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Services.GeometryValidateErrors;
using Utils;


namespace ConsoleApp;

class Program
{
    private const double EpsilonCoordinateComparator = 1e-9;
    private const double Epsilon = 1e-15;
    
    static async Task<int> Main(string[] args)
    {
        //Console usage example:
        //geosaver save -cs Host=localhost;Port=5432;Database=demo;Username=postgres;Password=admin -fs a.txt b.txt -mp 1500 -la water --srid 4326
        //geosaver validate -fs a.txt b.txt
        var rootCommand = new RootCommand("rootCommand")
        {
            Name = "geosaver"
        };
        var stringOption = new Option<string>(
            aliases: new[] { "-cs", "--connectionString" },
            description: "Option to set connectionString.")
        {
            IsRequired = true,
            Arity = ArgumentArity.ExactlyOne
        };
        var filesInfo = new Option<FileInfo[]>(
            aliases: new[] { "-fs", "--files" },
            description: "Input file names")
        {
            IsRequired = true,
            Arity = ArgumentArity.OneOrMore,
            AllowMultipleArgumentsPerToken = true
        };
        var numberOfPointsOption = new Option<int>(
            aliases: new[] { "-mp", "--maxNumberOfPointsInFragment" },
            description: "Option to set maximum number of points in fragment after slicing.")
        {
            IsRequired = true,
            Arity = ArgumentArity.ExactlyOne
        };
        var layerAliasOption = new Option<string>(
            aliases: new[] { "-la", "--layerAlias" },
            description: "Option to set layer alias for geometry.")
        {
            IsRequired = true,
            Arity = ArgumentArity.ExactlyOne
        };
        var sridOption = new Option<int>(
            aliases: new[] { "-s", "--srid" },
            description: "Option to set srid for geometry.")
        {
            IsRequired = true,
            Arity = ArgumentArity.ExactlyOne
        };
        var save = new Command("save",
            "Save geometries from {--files} in database using {--connectionString} to connect.");
        save.AddOption(stringOption);
        save.AddOption(filesInfo);
        save.AddOption(numberOfPointsOption);
        save.AddOption(layerAliasOption);
        save.AddOption(sridOption);
        save.SetHandler((connectionString, files, points, layerAlias, srid) =>
            {
                Console.WriteLine(points);
                Console.WriteLine(layerAlias);
                IServiceCollection serviceCollection = new ServiceCollection();
                using var geometrySaverProvider =
                    serviceCollection.BuildGeometrySaverServiceProvider(connectionString, points,
                        EpsilonCoordinateComparator, Epsilon, GeometrySlicerType.OppositeSlicer);
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
                            geometrySaver.SaveGeometry(feature.Geometry, layerAlias, feature.Attributes.ToString() ?? "", out errors);
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
            },
            stringOption, filesInfo, numberOfPointsOption, layerAliasOption, sridOption);
        var validate = new Command("validate",
            "Validate geometries from {--files}.");
        validate.AddOption(filesInfo);
        validate.SetHandler(files =>
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            using var geometryCorrectorProvider = serviceCollection.BuildGeometryCorrectorProvider(EpsilonCoordinateComparator);
            var correctionService = geometryCorrectorProvider.GetService<IGeometryCorrector<Polygon>>();
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
                    var polygon = ReadPolygonFromGeojsonFile(o);
                    correctionService.ValidateGeometry(ref geometryValidateErrors, polygon, ref errors);
                }
                catch (Exception e)
                {
                    throw new Exception(o.FullName + ":" + "\n" + e.Message, e);
                }
                Console.WriteLine(o.FullName + ":" + "\n" + errors);
            }
        }, filesInfo);
        rootCommand.Add(save);
        rootCommand.Add(validate);
        return await rootCommand.InvokeAsync(args);
    }

    static Polygon ReadPolygonFromGeojsonFile(FileInfo file)
    {
        var polygon = (Polygon)ReadGeometryFromFile<MultiPolygon>(file.FullName).Geometries[0];
        polygon.SRID = 0;
        return polygon;
    }

    private static T ReadGeometryFromFile<T>(string path) where T : class
    {
        string geoJson = File.ReadAllText(path);
        var geometry = new GeoJsonReader().Read<T>(geoJson);
        return geometry;
    }
}