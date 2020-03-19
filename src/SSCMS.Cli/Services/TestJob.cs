using System;
using System.Reflection;
using System.Threading.Tasks;
using Mono.Options;
using SSCMS;
using SSCMS.Cli.Core;

namespace SSCMS.Cli.Services
{
    public class TestJob : IJobService
    {
        public string CommandName => "test add";

        private bool _isHelp;
        private string _webConfigFileName;

        private readonly ISettingsManager _settingsManager;
        private readonly OptionSet _options;

        public TestJob(ISettingsManager settingsManager)
        {
            _settingsManager = settingsManager;
            _options = new OptionSet
            {
                {
                    "c|config=", "the {web.config} file name.",
                    v => _webConfigFileName = v
                },
                {
                    "h|help", "命令说明",
                    v => _isHelp = v != null
                }
            };
        }

        public void PrintUsage()
        {
            Console.WriteLine("测试: siteserver test add");
            _options.WriteOptionDescriptions(Console.Out);
            Console.WriteLine();
        }

        public async Task ExecuteAsync(IJobContext context)
        {
            if (!CliUtils.ParseArgs(_options, context.Args)) return;

            if (_isHelp)
            {
                return;
            }

            var version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            await Console.Out.WriteLineAsync($"SiteServer CLI Version: {version.Substring(0, version.Length - 2)}");
            await Console.Out.WriteLineAsync($"Work Directory: {_settingsManager.ContentRootPath}");
            await Console.Out.WriteLineAsync($"siteserver.exe Path: {Assembly.GetExecutingAssembly().Location}");
            await Console.Out.WriteLineAsync();
        }
    }
}
