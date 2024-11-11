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
            var filesInfo = new Option<FileInfo[]>(
                aliases: new[] { "-fs", "--files" },
                description: "Files to save")
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
            var save = new RootCommand("save: Command, which is used to save geometries from {--file} in database");
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

            return await save.InvokeAsync(args);
        }
        //add connection string for db

        static void ReadFile(FileInfo file)
        {
            File.ReadLines(file.FullName).ToList()
                .ForEach(line => Console.WriteLine(line));
        }
    }
}