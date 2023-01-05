using System;
using System.Threading.Tasks;
using Mono.Options;
using SSCMS.Cli.Abstractions;
using SSCMS.Cli.Core;
using SSCMS.Plugins;
using SSCMS.Utils;

namespace SSCMS.Cli.Jobs
{
    public class ThemeUnPublishJob : IJobService
    {
        public string CommandName => "theme unpublish";

        private string _name;
        private bool _isHelp;
        private readonly OptionSet _options;

        private readonly ICliApiService _cliApiService;

        public ThemeUnPublishJob(ICliApiService cliApiService)
        {
            _options = new OptionSet
            {
                { "n|name=", "theme name",
                    v => _name = v },
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

            if (string.IsNullOrEmpty(_name))
            {
                if (context.Extras != null && context.Extras.Length > 0)
                {
                    _name = context.Extras[0];
                }
            }
            if (string.IsNullOrEmpty(_name))
            {
                await console.WriteErrorAsync("missing required name");
                return;
            }

            var (status, failureMessage) = await _cliApiService.GetStatusAsync();
            if (status == null)
            {
                await console.WriteErrorAsync(failureMessage);
                return;
            }

            bool success;
            (success, failureMessage) = await _cliApiService.ThemeUnPublishAsync(_name);
            if (success)
            {
                await console.WriteSuccessAsync($"Theme {_name} unpublished .");
            }
            else
            {
                await console.WriteErrorAsync(failureMessage);
            }
        }
    }
}
