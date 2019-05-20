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

        private static string _configFile;
        private static bool _isHelp;

        private static readonly OptionSet Options = new OptionSet {
            { "c|config-file=", "指定配置文件Web.config路径或文件名",
                v => _configFile = v },
            { "h|help",  "命令说明",
                v => _isHelp = v != null }
        };

        public static void PrintUsage()
        {
            Console.WriteLine("显示当前版本: siteserver version");
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
            await Console.Out.WriteLineAsync($"SiteServer CLI 版本号: {version.Substring(0, version.Length - 2)}");
            await Console.Out.WriteLineAsync($"当前文件夹: {CliUtils.PhysicalApplicationPath}");
            await Console.Out.WriteLineAsync();

            var webConfigPath = CliUtils.GetWebConfigPath(_configFile);

            if (FileUtils.IsFileExists(webConfigPath))
            {
                AppSettings.Load("/", CliUtils.PhysicalApplicationPath, webConfigPath);

                try
                {
                    var cmsVersion = FileVersionInfo.GetVersionInfo(PathUtils.Combine(CliUtils.PhysicalApplicationPath, "Bin", "SiteServer.CMS.dll")).ProductVersion;
                    await Console.Out.WriteLineAsync($"SitServer CMS Version: {cmsVersion}");
                }
                catch
                {
                    // ignored
                }

                await Console.Out.WriteLineAsync($"数据库类型: {AppSettings.DatabaseType.Value}");
                await Console.Out.WriteLineAsync($"连接字符串: {AppSettings.ConnectionString}");
                await Console.Out.WriteLineAsync($"连接字符串（加密）: {TranslateUtils.EncryptStringBySecretKey(AppSettings.ConnectionString, AppSettings.SecretKey)}");
            }
        }
    }
}
