using System;
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
            //geosaver connect --host localhost --port 5432  --database demo --username postgres --password admin
            //geosaver save -fs a.txt b.txt
            
            //application context will be declared in dependency container
            PostgreApplicationContext? applicationContext = null;
            var rootCommand = new RootCommand("rootCommand")
            {
                Name = "geosaver"
            };
            var filesInfo = new Option<FileInfo[]>(
                aliases: new[] { "-fs", "--files" },
                description: "Files to save in {save}")
            {
                IsRequired = true,
                Arity = ArgumentArity.OneOrMore,
                AllowMultipleArgumentsPerToken = true
            };
            var validateOption = new Option<bool>(
                aliases: new[] { "-v", "--val", "--valid", "--validate" },
                description: "Option to enable geometry validation",
                getDefaultValue: () => false)
            {
                Arity = ArgumentArity.ZeroOrOne
            };
            var fixOption = new Option<bool>(
                aliases: new[] { "-fx", "--fix" },
                description: "Option to enable geometry fix after validation",
                getDefaultValue: () => false)
            {
                Arity = ArgumentArity.ZeroOrOne
            };
            var save = new Command("save", "Save geometries from {--files} in database");
            save.AddOption(filesInfo);
            save.AddOption(validateOption);
            save.AddOption(fixOption);
            save.AddValidator(commandResult =>
            {
                if (applicationContext == null || !applicationContext.Database.CanConnect())
                {
                    Console.WriteLine(applicationContext);
                    // Console.WriteLine(applicationContext!.Database.CanConnect());
                    commandResult.ErrorMessage = "Can not connect to database";
                }
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
                    commandResult.ErrorMessage = "Can not fix geometry without validation";
                }
            });
            save.SetHandler((files, validate) =>
                {
                    foreach (var o in files)
                    {
                        var polygon = ReadPolygonFromGeojsonFile(o);
                        // Call controller method to save polygon
                    }
                },
                filesInfo, validateOption);
            var hostOption = new Option<string>(
                aliases: new[] { "-h", "--hst", "--host" },
                description: "Option to set hostname")
            {
                Arity = ArgumentArity.ExactlyOne
            };
            var portOption = new Option<long>(
                aliases: new[] { "-p", "--prt", "--port" },
                description: "Option to set port")
            {
                Arity = ArgumentArity.ExactlyOne
            };
            var databaseOption = new Option<string>(
                aliases: new[] { "-d", "--dtbs", "--dtbase", "--database" },
                description: "Option to set database name")
            {
                Arity = ArgumentArity.ExactlyOne
            };
            var usernameOption = new Option<string>(
                aliases: new[] { "-u", "--usr", "--usrnm", "--usrname", "--user", "--name", "--usernm", "--username" },
                description: "Option to set username")
            {
                Arity = ArgumentArity.ExactlyOne
            };
            var passwordOption = new Option<string>(
                aliases: new[] { "-pd", "--pswrd", "--passwd", "--password" },
                description: "Option to set password")
            {
                Arity = ArgumentArity.ExactlyOne
            };
            var connect = new Command("connect", "Connect to database using given arguments");
            connect.AddOption(hostOption);
            connect.AddOption(portOption);
            connect.AddOption(databaseOption);
            connect.AddOption(usernameOption);
            connect.AddOption(passwordOption);
            connect.SetHandler((host, port, database, username, password) =>
            {
                if (applicationContext != null && applicationContext.Database.CanConnect()) return;
                applicationContext =
                    new PostgreApplicationContext(
                        $"Host={host};Port={port};Database={database};Username={username};Password={password}");
                Console.WriteLine(applicationContext.Database.CanConnect()
                    ? "Connection succeeded!"
                    : "Connection failed.");
            }, hostOption, portOption, databaseOption, usernameOption, passwordOption);
            rootCommand.Add(save);
            rootCommand.Add(connect);
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

