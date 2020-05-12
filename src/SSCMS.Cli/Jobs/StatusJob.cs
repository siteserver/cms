using System;
using System.Threading.Tasks;
using Mono.Options;
using SSCMS.Cli.Abstractions;
using SSCMS.Cli.Core;
using SSCMS.Services;

namespace SSCMS.Cli.Jobs
{
    public class StatusJob : IJobService
    {
        public string CommandName => "status";

        private bool _isHelp;

        private readonly ISettingsManager _settingsManager;
        private readonly IApiService _apiService;
        private readonly OptionSet _options;

        public StatusJob(ISettingsManager settingsManager, IApiService apiService)
        {
            _settingsManager = settingsManager;
            _apiService = apiService;
            _options = new OptionSet
            {
                {
                    "h|help", "命令说明",
                    v => _isHelp = v != null
                }
            };
        }

        public void PrintUsage()
        {
            Console.WriteLine($"Usage: sscms-cli {CommandName}");
            Console.WriteLine("Summary: show user login status");
            Console.WriteLine("Options:");
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

            await WriteUtils.PrintInfoAsync(_settingsManager);

            var (success, successContent, failureContent) = _apiService.GetStatus();
            if (success)
            {
                await WriteUtils.PrintSuccessAsync(successContent);
            }
            else
            {
                await WriteUtils.PrintErrorAsync(failureContent);
            }
        }
    }
}