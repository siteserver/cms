using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using SS.CMS.Utils;

namespace SS.CMS.Cli
{
    class Program
    {
        static int Main(string[] args)
        {
            // Create some options and a parser
            var optionThatTakesInt = new Option(
                "--int-option",
                "An option whose argument is parsed as an int",
                new Argument<int>(defaultValue: 42));
            optionThatTakesInt.AddAlias("-i");

            var optionThatTakesBool = new Option(
                "--bool-option",
                "An option whose argument is parsed as a bool",
                new Argument<bool>());
            var optionThatTakesFileInfo = new Option(
                "--file-option",
                "An option whose argument is parsed as a FileInfo",
                new Argument<FileInfo>());

            // Add them to the root command
            var rootCommand = new RootCommand();

            rootCommand.Description = "My sample app";
            rootCommand.AddOption(optionThatTakesInt);
            rootCommand.AddOption(optionThatTakesBool);
            rootCommand.AddOption(optionThatTakesFileInfo);

            rootCommand.Handler = CommandHandler.Create<int, bool, FileInfo>((intOption, boolOption, fileOption) =>
            {
                Console.WriteLine($"The value for --int-option is: {intOption}");
                Console.WriteLine($"The value for --bool-option is: {boolOption}");
                Console.WriteLine($"The value for --file-option is: {fileOption?.FullName ?? "null"}");

                var path = PathUtils.Combine(Directory.GetParent(Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).FullName).ToString(), ".sscms");

                Console.WriteLine(path);
            });

            var command = new Command("subcommand")
            {
                new Command("subsubcommand")
            };

            rootCommand.AddCommand(command);

            // Parse the incoming args and invoke the handler
            return rootCommand.InvokeAsync(args).Result;
        }
    }
}
