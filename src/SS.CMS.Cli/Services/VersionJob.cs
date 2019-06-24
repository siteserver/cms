using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using Mono.Options;
using SS.CMS.Cli.Core;
using SS.CMS.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace SS.CMS.Cli.Services
{
    public class VersionJob
    {
        public const string CommandName = "version";

        public static async Task Execute(IJobContext context)
        {
            var application = CliUtils.Provider.GetService<VersionJob>();
            await application.RunAsync(context);
        }

        public static void PrintUsage()
        {
            Console.WriteLine("显示当前版本: siteserver version");
            var job = new VersionJob();
            job._options.WriteOptionDescriptions(Console.Out);
            Console.WriteLine();
        }
        private string _configFile;
        private string _databaseType;
        private string _connectionString;
        private bool _isHelp;
        private readonly OptionSet _options;
        public VersionJob()
        {
            _options = new OptionSet {
                { "c|config-file=", "指定配置文件Web.config路径或文件名",
                    v => _configFile = v },
                { "database-type=", "指定需要查看的数据库类型",
                    v => _databaseType = v },
                { "connection-string=", "指定需要查看的数据库连接字符串",
                    v => _connectionString = v },
                { "h|help",  "命令说明",
                    v => _isHelp = v != null }
            };
        }

        public async Task RunAsync(IJobContext context)
        {
            if (!CliUtils.ParseArgs(_options, context.Args)) return;

            if (_isHelp)
            {
                PrintUsage();
                return;
            }

            var version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            await Console.Out.WriteLineAsync($"SiteServer CLI 版本号: {version.Substring(0, version.Length - 2)}");
            await Console.Out.WriteLineAsync($"当前文件夹: {CliUtils.PhysicalApplicationPath}");
            await Console.Out.WriteLineAsync();

            var (db, errorMessage) = await CliUtils.GetDatabaseAsync(_databaseType, _connectionString, _configFile);
            if (db != null)
            {
                try
                {
                    var cmsVersion = FileVersionInfo.GetVersionInfo(PathUtils.Combine(CliUtils.PhysicalApplicationPath, "Bin", "SiteServer.CMS.dll")).ProductVersion;
                    await Console.Out.WriteLineAsync($"SitServer CMS Version: {cmsVersion}");
                }
                catch
                {
                    // ignored
                }

                await Console.Out.WriteLineAsync($"数据库类型: {db.DatabaseType.Value}");
                await Console.Out.WriteLineAsync($"连接字符串: {db.ConnectionString}");
            }
        }
    }
}
