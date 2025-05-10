using System.CommandLine;
using ConsoleApp.CommandHandlers;
using GeometrySlicerTypes;


namespace ConsoleApp;
//todo утечка памяти при сохранении большого количества объектов
class Program
{
    static async Task<int> Main(string[] args)
    {
        //Console usage example:
        //geosaver save -cs Host=localhost;Port=5432;Database=demo;Username=postgres;Password=admin -fs a.txt b.txt -mp 1500 -la water --srid 4326 --algorithm OppositeSlicer
        //geosaver save -cs Host=localhost;Port=5432;Database=demo;Username=postgres;Password=admin -fs a.txt b.txt -la water --srid 4326 --algorithm NonConvexSlicer
        //geosaver validate -fs a.txt b.txt --srid 4326
        var rootCommand = new RootCommand("rootCommand")
        {
            Name = "geosaver"
        };
        var connectionStringOption = new Option<string>(
            aliases: ["-cs", "--connectionString"],
            description: "Option to set connectionString.")
        {
            IsRequired = true,
            Arity = ArgumentArity.ExactlyOne
        };
        var filesInfo = new Option<FileInfo[]>(
            aliases: ["-fs", "--files"],
            description: "Input file names")
        {
            IsRequired = true,
            Arity = ArgumentArity.OneOrMore,
            AllowMultipleArgumentsPerToken = true
        };
        var numberOfPointsOption = new Option<int?>(
            aliases: ["-mp", "--maxNumberOfPointsInFragment"],
            description: "Option to set maximum number of points in fragment after slicing.")
        {
            IsRequired = false,
            Arity = ArgumentArity.ExactlyOne
        };
        var layerAliasOption = new Option<string>(
            aliases: ["-la", "--layerAlias"],
            description: "Option to set layer alias for geometry.")
        {
            IsRequired = true,
            Arity = ArgumentArity.ExactlyOne
        };
        var sridOption = new Option<int>(
            aliases: ["-s", "--srid"],
            description: "Option to set srid for geometry.")
        {
            IsRequired = true,
            Arity = ArgumentArity.ExactlyOne
        };
        var algorithmOption = new Option<GeometrySlicerType>(
            aliases: ["-a", "--algorithm"],
            description: "Option to select the algorithm to use.")
        {
            IsRequired = true,
            Arity = ArgumentArity.ExactlyOne
        };
        
        //save
        var save = new Command("save",
            "Save geometries from {--files} in database using {--connectionString} to connect.");
        save.AddOption(connectionStringOption);
        save.AddOption(filesInfo);
        save.AddOption(layerAliasOption);
        save.AddOption(sridOption);
        save.AddOption(algorithmOption);
        save.AddOption(numberOfPointsOption);
        save.SetHandler((connectionString, files, layerAlias, srid, type, points) =>
            {
                if (type == GeometrySlicerType.OppositeSlicer && !points.HasValue)
                {
                    Console.WriteLine("Error: --maxNumberOfPointsInFragment is required for OppositeSlicer.");
                }
                else SaveCommandHandler.Handle(connectionString, files, layerAlias, srid, type, points);
            },
            connectionStringOption, filesInfo, layerAliasOption, sridOption, algorithmOption, numberOfPointsOption);
        
        //validate
        var validate = new Command("validate",
            "Validate geometries from {--files}.");
        validate.AddOption(filesInfo);
        validate.AddOption(sridOption);
        validate.SetHandler(ValidateCommandHandler.Handle, filesInfo, sridOption);
        rootCommand.Add(save);
        rootCommand.Add(validate);
        return await rootCommand.InvokeAsync(args);
    }
}