using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using NDesk.Options;
using SiteServer.Cli.Core;
using SiteServer.Utils;

namespace SiteServer.Cli.Commands
{
    public static class TestManager
    {
        public const string CommandName = "test";

        private static bool _isHelp;
        private static string _webConfigFileName;

        private static readonly OptionSet Options = new OptionSet() {
            { "c|config=", "the {web.config} file name.",
                v => _webConfigFileName = v },
            { "h|help",  "show this message and exit",
                v => _isHelp = v != null }
        };

        public static void Execute(string[] args)
        {
            if (!CliUtils.ParseArgs(Options, args)) return;

            if (_isHelp)
            {
                return;
            }

            var version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Console.WriteLine($"SiteServer CLI Version: {version.Substring(0, version.Length - 2)}");
            Console.WriteLine($"Work Directory: {CliUtils.PhysicalApplicationPath}");
            Console.WriteLine();

            var content = FileUtils.ReadText(PathUtils.Combine(CliUtils.PhysicalApplicationPath, "_metadata.json"), Encoding.UTF8);
            var table = TranslateUtils.JsonDeserialize<TableInfo>(content);

            Console.WriteLine($"_metadata: {TranslateUtils.JsonSerialize(table.Columns)}");
        }
    }
}
