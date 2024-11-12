using System;
using System.Collections;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ConsoleApp.Controllers;
using DataAccess.PostgreSql;
using Entities;
using NetTopologySuite.Geometries;


namespace ConsoleApp
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
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
                var validateOptionCommandResult = commandResult.FindResultFor(validateOption);
                var fixOptionCommandResult = commandResult.FindResultFor(fixOption);
                if (!(fixOptionCommandResult is null) &&
                    (fixOptionCommandResult.GetValueOrDefault() is null ||
                     !(fixOptionCommandResult.GetValueOrDefault() is null) && fixOptionCommandResult.GetValueOrDefault()!.ToString() == true.ToString())
                    && (validateOptionCommandResult is null || !(validateOptionCommandResult.GetValueOrDefault() is null) &&
                    validateOptionCommandResult.GetValueOrDefault()!.ToString() == false.ToString()))
                {
                    commandResult.ErrorMessage = "Can not fix geometry without validation";
                }
            });
            save.SetHandler((files, validate) =>
                {
                    foreach (var o in files)
                    {
                        Console.WriteLine(o + "|" + validate);
                    }
                },
                filesInfo, validateOption);
            //add connection string for db
            //"Host=localhost;Port=5432;Database=demo;Username=postgres;Password=admin"
            //--host localhost --port 5432 --username postgres --password admin
            //save only after connect
            //flag to control connection?
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
            connect.AddOption(usernameOption);
            connect.AddOption(passwordOption);
            connect.SetHandler((host, port, username, password) =>
            {
                Console.WriteLine(host + "|" + port + "|" + username + "|" + password);
            }, hostOption, portOption, usernameOption, passwordOption);
            rootCommand.Add(save);
            rootCommand.Add(connect);
            return await rootCommand.InvokeAsync(args);
        }
        

        static void ReadFile(FileInfo file)
        {
            File.ReadLines(file.FullName).ToList()
                .ForEach(line => Console.WriteLine(line));
        }
    }
}