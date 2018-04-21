using System;
using System.Collections.Generic;
using System.Reflection;
using SiteServer.Cli.Core;

namespace SiteServer.Cli.Commands
{
    public static class VersionManager
    {
        public const string CommandName = "version";

        public static void PrintUsage()
        {
            CliUtils.PrintRow("./siteserver.exe version");
        }

        public static void Execute(List<string> commandValues)
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Console.WriteLine($"Version: {version.Substring(0, version.Length - 2)}");
        }
    }
}
