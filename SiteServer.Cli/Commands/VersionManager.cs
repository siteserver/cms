using System;
using System.Diagnostics;
using System.Reflection;
using NDesk.Options;
using SiteServer.Cli.Core;
using SiteServer.Utils;

namespace SiteServer.Cli.Commands
{
    public static class VersionManager
    {
        public const string CommandName = "version";

        private static bool _isHelp;
        private static string _webConfigFileName;

        private static readonly OptionSet Options = new OptionSet() {
            { "c|config=", "the {web.config} file name.",
                v => _webConfigFileName = v },
            { "h|help",  "show this message and exit",
                v => _isHelp = v != null }
        };

        public static void PrintUsage()
        {
            Console.WriteLine("Version command usage: siteserver version");
            Options.WriteOptionDescriptions(Console.Out);
            Console.WriteLine();
        }

        public static void Execute(string[] args)
        {
            if (!CliUtils.ParseArgs(Options, args)) return;

            if (_isHelp)
            {
                PrintUsage();
                return;
            }

            var version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Console.WriteLine($"SiteServer CLI Version: {version.Substring(0, version.Length - 2)}");
            Console.WriteLine($"Current Directory: {CliUtils.PhysicalApplicationPath}");

            if (!string.IsNullOrEmpty(_webConfigFileName))
            {
                WebConfigUtils.Load(CliUtils.PhysicalApplicationPath, _webConfigFileName);

                try
                {
                    var cmsVersion = FileVersionInfo.GetVersionInfo(PathUtils.Combine(CliUtils.PhysicalApplicationPath, "Bin", "SiteServer.CMS.dll")).ProductVersion;
                    Console.WriteLine($"SitServer CMS Version: {cmsVersion}");
                }
                catch
                {
                    // ignored
                }

                Console.WriteLine($"Database Type: {WebConfigUtils.DatabaseType.Value}");
                Console.WriteLine($"Connection String Decode: {WebConfigUtils.ConnectionString}");
                Console.WriteLine($"Connection String Encode: {TranslateUtils.EncryptStringBySecretKey(WebConfigUtils.ConnectionString, WebConfigUtils.SecretKey)}");
            }
        }
    }
}
