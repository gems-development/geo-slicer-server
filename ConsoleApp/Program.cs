using System;
using System.Collections.Generic;
using System.CommandLine;
using System.IO;
using System.Threading.Tasks;
using ConsoleApp.Controllers.Helpers;
using ConsoleApp.Controllers;
using ConsoleApp.Services;
using DataAccess.PostgreSql;
using DataAccess.Repositories.ConsoleApp;
using DomainModels;
using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Services.Creators;
using Services.Fixers;
using Services.ValidateErrors;
using Services.Validators;
using Slicers;


namespace ConsoleApp
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            //Console usage example:
            //geosaver save -cs Host=localhost;Port=5432;Database=demo;Username=postgres;Password=admin -fs a.txt b.txt
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
            var save = new Command("save",
                "Save geometries from {--files} in database using {--connectionString} to connect.");
            save.AddOption(stringOption);
            save.AddOption(filesInfo);
            save.SetHandler((connectionString, files) =>
                {
                    IServiceCollection serviceCollection = new ServiceCollection();
                    serviceCollection.AddGeometryDbContext(connectionString);
                    serviceCollection.AddSaveRepository();
                    serviceCollection.AddGeometryFixer();
                    serviceCollection.AddConcreteValidator();
                    serviceCollection.AddGeometryValidator();
                    serviceCollection.AddSlicers();
                    serviceCollection.AddGeometryWithFragmentsCreator();
                    serviceCollection.AddCorrectionService();
                    serviceCollection.AddGeometryController();
                    using var serviceProvider = serviceCollection.BuildServiceProvider();
                    var geometryController = serviceProvider
                        .GetService<GeometryController<Polygon, FragmentWithNonRenderingBorder<Polygon, MultiLineString>, int>>();
                    if (geometryController == null)
                    {
                        throw new NullReferenceException("Geometry controller is null");
                    }
                    geometryController.StartTransaction();
                    foreach (var o in files)
                    {
                        string errors;
                        try
                        {
                            var polygon = ReadPolygonFromGeojsonFile(o);
                            geometryController.SaveGeometry(polygon, out errors, new Dictionary<Parameter, bool>
                            {
                                {Parameter.Validate, true},
                                {Parameter.Fix, true}
                            });
                        }
                        catch (Exception e)
                        {
                            geometryController.RollbackTransaction();
                            throw new Exception(o.FullName + ":" + "\n" + e.Message);
                        }
                        Console.WriteLine(o.FullName + ":" + "\n" + errors);
                    }
                    geometryController.CommitTransaction();
                },
                stringOption, filesInfo);
            var validate = new Command("validate",
                "Validate geometries from {--files}.");
            validate.AddOption(filesInfo);
            validate.SetHandler(files =>
            {
                IServiceCollection serviceCollection = new ServiceCollection();
                serviceCollection.AddGeometryFixer();
                serviceCollection.AddConcreteValidator();
                serviceCollection.AddGeometryValidator();
                serviceCollection.AddCorrectionService();
                using var serviceProvider = serviceCollection.BuildServiceProvider();
                var correctionService = serviceProvider
                    .GetService<CorrectionService<Polygon>>();
                if (correctionService == null)
                {
                    throw new NullReferenceException("Correction service is null");
                }
                foreach (var o in files)
                {
                    string errors = "";
                    GeometryValidateError[]? geometryValidateErrors = null;
                    try
                    {
                        var polygon = ReadPolygonFromGeojsonFile(o);
                        correctionService.ValidateGeometry(true, ref geometryValidateErrors, polygon, ref errors);
                    }
                    catch (Exception e)
                    {
                        throw new Exception(o.FullName + ":" + "\n" + e.Message);
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
            return (Polygon)ReadGeometryFromFile<MultiPolygon>(file.FullName)[0];
        }

        private static T ReadGeometryFromFile<T>(string path) where T : class
        {
            string geoJson = File.ReadAllText(path);
            var geometry = new GeoJsonReader().Read<T>(geoJson);
            return geometry;
        }
    }
}