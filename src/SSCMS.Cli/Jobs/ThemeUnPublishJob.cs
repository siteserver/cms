using System.Threading.Tasks;
using Mono.Options;
using SSCMS.Cli.Abstractions;
using SSCMS.Cli.Core;
using SSCMS.Configuration;
using SSCMS.Plugins;
using SSCMS.Utils;

namespace SSCMS.Cli.Jobs
{
    public class ThemeUnPublishJob : IJobService
    {
        public string CommandName => "theme unpublish";

        private string _directory;
        private bool _isHelp;
        private readonly OptionSet _options;

        private readonly ICliApiService _cliApiService;

        public ThemeUnPublishJob(ICliApiService cliApiService)
        {
            _options = new OptionSet
            {
                { "d|directory=", "theme name",
                    v => _directory = v },
                {
                    "h|help", "Display help",
                    v => _isHelp = v != null
                }
            };

            _cliApiService = cliApiService;
        }

        public async Task WriteUsageAsync(IConsoleUtils console)
        {
            await console.WriteLineAsync($"Usage: sscms {CommandName} <pluginId>");
            await console.WriteLineAsync("Summary: unpublishes a theme.");
            await console.WriteLineAsync($"Docs: {Constants.OfficialHost}/docs/v7/cli/commands/theme-unpublish.html");
            await console.WriteLineAsync("Options:");
            _options.WriteOptionDescriptions(console.Out);
            await console.WriteLineAsync();
        }

        public async Task ExecuteAsync(IPluginJobContext context)
        {
            if (!CliUtils.ParseArgs(_options, context.Args)) return;

            using var console = new ConsoleUtils(false);
            if (_isHelp)
            {
                await WriteUsageAsync(console);
                return;
            }

            if (string.IsNullOrEmpty(_directory))
            {
                await console.WriteErrorAsync("missing required theme name");
                return;
            }

            var (status, failureMessage) = await _cliApiService.GetStatusAsync();
            if (status == null)
            {
                await console.WriteErrorAsync(failureMessage);
                return;
            }

            bool success;
            (success, failureMessage) = await _cliApiService.ThemeUnPublishAsync(_directory);
            if (success)
            {
                await console.WriteSuccessAsync($"Theme {_directory} unpublished.");
            }
            else
            {
                await console.WriteErrorAsync(failureMessage);
            }
        }
    }
}
