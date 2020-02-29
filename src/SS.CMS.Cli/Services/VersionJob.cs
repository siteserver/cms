using System;
using System.Reflection;
using System.Threading.Tasks;
using Mono.Options;
using SS.CMS.Abstractions;
using SS.CMS.Cli.Core;

namespace SS.CMS.Cli.Services
{
    public class VersionJob : IJobService
    {
        public string CommandName => "version";

        private string _configFile;
        private bool _isHelp;

        private readonly ISettingsManager _settingsManager;
        private readonly OptionSet _options;

        public VersionJob(ISettingsManager settingsManager)
        {
            _settingsManager = settingsManager;
            _options = new OptionSet {
                { "c|config-file=", "指定配置文件Web.config路径或文件名",
                    v => _configFile = v },
                { "h|help",  "命令说明",
                    v => _isHelp = v != null }
            };
        }

        public void PrintUsage()
        {
            Console.WriteLine("显示当前版本: siteserver version");
            _options.WriteOptionDescriptions(Console.Out);
            Console.WriteLine();
        }

        public async Task ExecuteAsync(IJobContext context)
        {
            if (!CliUtils.ParseArgs(_options, context.Args)) return;

            if (_isHelp)
            {
                PrintUsage();
                return;
            }

            var version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            await Console.Out.WriteLineAsync($"SiteServer CLI 版本号: {version.Substring(0, version.Length - 2)}");
            await Console.Out.WriteLineAsync($"当前文件夹: {_settingsManager.ContentRootPath}");
            await Console.Out.WriteLineAsync();

            var webConfigPath = CliUtils.GetWebConfigPath(_configFile, _settingsManager);

            if (FileUtils.IsFileExists(webConfigPath))
            {
                //WebConfigUtils.Load(_settingsManager.ContentRootPath, webConfigPath);

                //try
                //{
                //    var cmsVersion = FileVersionInfo.GetVersionInfo(PathUtils.Combine(_settingsManager.ContentRootPath, "Bin", "SS.CMS.dll")).ProductVersion;
                //    await Console.Out.WriteLineAsync($"SitServer CMS Version: {cmsVersion}");
                //}
                //catch
                //{
                //    // ignored
                //}

                //await Console.Out.WriteLineAsync($"数据库类型: {_settingsManager.Database.DatabaseType.GetValue()}");
                //await Console.Out.WriteLineAsync($"连接字符串: {WebConfigUtils.ConnectionString}");
                //await Console.Out.WriteLineAsync($"连接字符串（加密）: {TranslateUtils.EncryptStringBySecretKey(WebConfigUtils.ConnectionString, WebConfigUtils.SecretKey)}");
            }
        }
    }
}
