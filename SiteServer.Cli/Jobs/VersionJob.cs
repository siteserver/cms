using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using NDesk.Options;
using SiteServer.Cli.Core;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.Cli.Jobs
{
    public static class VersionJob
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

        public static async Task Execute(IJobContext context)
        {
            if (!CliUtils.ParseArgs(Options, context.Args)) return;

            if (_isHelp)
            {
                PrintUsage();
                return;
            }

            var version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            await Console.Out.WriteLineAsync($"SiteServer CLI Version: {version.Substring(0, version.Length - 2)}");
            await Console.Out.WriteLineAsync($"Work Directory: {CliUtils.PhysicalApplicationPath}");
            await Console.Out.WriteLineAsync();

            if (string.IsNullOrEmpty(_webConfigFileName))
            {
                _webConfigFileName = "web.config";
            }

            if (FileUtils.IsFileExists(PathUtils.Combine(CliUtils.PhysicalApplicationPath, _webConfigFileName)))
            {
                WebConfigUtils.Load(CliUtils.PhysicalApplicationPath, _webConfigFileName);

                try
                {
                    var cmsVersion = FileVersionInfo.GetVersionInfo(PathUtils.Combine(CliUtils.PhysicalApplicationPath, "Bin", "SiteServer.CMS.dll")).ProductVersion;
                    await Console.Out.WriteLineAsync($"SitServer CMS Version: {cmsVersion}");
                }
                catch
                {
                    // ignored
                }

                await Console.Out.WriteLineAsync($"Database Type: {WebConfigUtils.DatabaseType.Value}");
                await Console.Out.WriteLineAsync($"Connection String Decode: {WebConfigUtils.ConnectionString}");
                await Console.Out.WriteLineAsync($"Connection String Encode: {TranslateUtils.EncryptStringBySecretKey(WebConfigUtils.ConnectionString, WebConfigUtils.SecretKey)}");
            }
        }
    }
}
