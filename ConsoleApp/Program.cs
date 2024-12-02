using System.CommandLine;
using System.IO;
using System.Threading.Tasks;
using DataAccess.PostgreSql;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;


namespace ConsoleApp
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            //Console usage example:
            //geosaver save Host=localhost;Port=5432;Database=demo;Username=postgres;Password=admin -fs a.txt b.txt -v -f
            
            //application context will be declared in dependency container
            PostgreApplicationContext? applicationContext = null;
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
            var save = new Command("save", "Save geometries from {--files} in database using {--connectionString} to connect.");
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
            save.SetHandler((connectionString, files, validate) =>
                {
                    if (applicationContext == null)
                    {
                        //error
                    }
                    applicationContext = new PostgreApplicationContext(connectionString);
                    if (!applicationContext.Database.CanConnect())
                    {
                        //error
                    }
                    //start transaction
                    foreach (var o in files)
                    {
                        //check file reading error
                        //if error - rollback
                        var polygon = ReadPolygonFromGeojsonFile(o);
                        //call controller method to save polygon and get error list
                    }
                    //commit transaction
                },
                stringOption, filesInfo, validateOption);
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

