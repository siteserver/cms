using System;
using System.Reflection;
using System.Threading.Tasks;
using Mono.Options;
using SS.CMS.Cli.Core;
using SS.CMS.Services;
using Microsoft.Extensions.DependencyInjection;

namespace SS.CMS.Cli.Services
{
    public class TestJob
    {
        public const string CommandName = "test add";

        public static async Task Execute(IJobContext context)
        {
            var application = CliUtils.Provider.GetService<TestJob>();
            await application.RunAsync(context);
        }

        private bool _isHelp;
        private string _configFileName;
        private readonly OptionSet _options;
        private readonly ISettingsManager _settingsManager;
        public TestJob(ISettingsManager settingsManager)
        {
            _settingsManager = settingsManager;
            _options = new OptionSet()
            {
                {
                    "c|config=", "the {web.config} file name.",
                    v => _configFileName = v
                },
                {
                    "h|help", "命令说明",
                    v => _isHelp = v != null
                }
            };
        }

        public async Task RunAsync(IJobContext context)
        {
            if (!CliUtils.ParseArgs(_options, context.Args)) return;

            if (_isHelp)
            {
                return;
            }

            var version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            await Console.Out.WriteLineAsync($"SiteServer CLI Version: {version.Substring(0, version.Length - 2)}");
            await Console.Out.WriteLineAsync($"Work Directory: {CliUtils.PhysicalApplicationPath}");
            await Console.Out.WriteLineAsync($"siteserver.exe Path: {Assembly.GetExecutingAssembly().Location}");
            await Console.Out.WriteLineAsync();
        }
    }
}
