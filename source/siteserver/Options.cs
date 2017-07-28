using System.Text;
using CommandLine;

namespace siteserver
{
    internal class BuildSubOptions
    {
        [Option('a', "all", HelpText = "Tell the command to automatically stage files.")]
        public bool All { get; set; }
    }

    internal class RunSubOptions
    {
        // Remainder omitted
    }

    internal class TestSubOptions
    {
        // Remainder omitted
    }

    internal class Options
    {
        [Option('i', "input", Required = true, HelpText = "Input file to read.")]
        public string InputFile { get; set; }

        [Option('v', null, HelpText = "Print details during execution.")]
        public bool Verbose { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            // this without using CommandLine.Text
            //  or using HelpText.AutoBuild
            var usage = new StringBuilder();
            usage.AppendLine("Quickstart Application 1.0");
            usage.AppendLine("Read user manual for usage instructions...");
            return usage.ToString();
        }

        [VerbOption("build", HelpText = "Record changes to the repository.")]
        public BuildSubOptions BuildVerb { get; set; }

        [VerbOption("run", HelpText = "Update remote refs along with associated objects.")]
        public RunSubOptions WatchVerb { get; set; }

        [VerbOption("test", HelpText = "Update remote refs along with associated objects.")]
        public TestSubOptions TestVerb { get; set; }
    }
}
