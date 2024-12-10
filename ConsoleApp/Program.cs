using System;
using System.Collections.Generic;
using System.CommandLine;
using System.IO;
using System.Threading.Tasks;
using ConsoleApp.Controllers.Helpers;
using ConsoleApp.Controllers;
using DataAccess.Interfaces;
using DataAccess.PostgreSql;
using DataAccess.Repositories.ConsoleApp;
using DomainModels;
using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Services.Creators;
using Services.Fixers;
using Services.Validators;
using Slicers;


namespace ConsoleApp
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            //Console usage example:
            //geosaver save -cs Host=localhost;Port=5432;Database=demo;Username=postgres;Password=admin -fs a.txt b.txt -v -fx
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
                description: "Files to save in {save}.")
            {
                IsRequired = true,
                Arity = ArgumentArity.OneOrMore,
                AllowMultipleArgumentsPerToken = true
            };
            var validateOption = new Option<bool>(
                aliases: new[] { "-v", "--val", "--valid", "--validate" },
                description: "Option to enable geometry validation.",
                getDefaultValue: () => false)
            {
                Arity = ArgumentArity.ZeroOrOne
            };
            var fixOption = new Option<bool>(
                aliases: new[] { "-fx", "--fix" },
                description: "Option to enable geometry fix after validation.",
                getDefaultValue: () => false)
            {
                Arity = ArgumentArity.ZeroOrOne
            };
            var save = new Command("save",
                "Save geometries from {--files} in database using {--connectionString} to connect.");
            save.AddOption(stringOption);
            save.AddOption(filesInfo);
            save.AddOption(validateOption);
            save.AddOption(fixOption);
            save.AddValidator(commandResult =>
            {
                var validateOptionCommandResult = commandResult.FindResultFor(validateOption);
                var fixOptionCommandResult = commandResult.FindResultFor(fixOption);
                if (!(fixOptionCommandResult is null) &&
                    (fixOptionCommandResult.GetValueOrDefault() is null ||
                     !(fixOptionCommandResult.GetValueOrDefault() is null) &&
                     fixOptionCommandResult.GetValueOrDefault()!.ToString() == true.ToString())
                    && (validateOptionCommandResult is null ||
                        !(validateOptionCommandResult.GetValueOrDefault() is null) &&
                        validateOptionCommandResult.GetValueOrDefault()!.ToString() == false.ToString()))
                {
                    commandResult.ErrorMessage = "Can not fix geometry without validation.";
                }
            });
            save.SetHandler((connectionString, files, validate, fix) =>
                {
                    IServiceCollection serviceCollection = new ServiceCollection();
                    serviceCollection.AddGeometryDbContext(connectionString);
                    serviceCollection.AddSaveRepository();
                    serviceCollection.AddGeometryFixer();
                    serviceCollection.AddConcreteValidator();
                    serviceCollection.AddGeometryValidator();
                    serviceCollection.AddSlicers();
                    serviceCollection.AddGeometryWithFragmentsCreator();
                    serviceCollection.AddGeometryController();
                    using var serviceProvider = serviceCollection.BuildServiceProvider();

                    var applicationContext = serviceProvider.GetService<GeometryDbContext>();
                    var geometryController = serviceProvider
                        .GetService<GeometryController<Polygon, FragmentWithNonRenderingBorder<Polygon, MultiLineString>, int>>();
                    if (applicationContext == null)
                    {
                        throw new NullReferenceException("Application context is null");
                    }
                    if (geometryController == null)
                    {
                        throw new NullReferenceException("Geometry controller is null");
                    }
                    if (!applicationContext.Database.CanConnect())
                    {
                        throw new Exception("Can not connect to database");
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
                                {Parameter.Validate, validate},
                                {Parameter.Fix, fix}
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
                stringOption, filesInfo, validateOption, fixOption);
            rootCommand.Add(save);
            return await rootCommand.InvokeAsync(args);
        }

        static Polygon ReadPolygonFromGeojsonFile(FileInfo file)
        {
            return ReadGeometryFromFile<Polygon>(file.FullName);
        }

        private static T ReadGeometryFromFile<T>(string path) where T : class
        {
            string geoJson = File.ReadAllText(path);
            var geometry = new GeoJsonReader().Read<T>(geoJson);
            return geometry;
        }
    }
}