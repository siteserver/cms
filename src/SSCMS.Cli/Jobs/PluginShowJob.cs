using System;
using System.Threading.Tasks;
using Mono.Options;
using SSCMS.Cli.Abstractions;
using SSCMS.Cli.Core;
using SSCMS.Plugins;
using SSCMS.Utils;

namespace SSCMS.Cli.Jobs
{
    public class PluginShowJob : IJobService
    {
        public string CommandName => "plugin show";

        private bool _isHelp;

        private readonly IApiService _apiService;
        private readonly OptionSet _options;

        public PluginShowJob(IApiService apiService)
        {
            _apiService = apiService;
            _options = new OptionSet
            {
                {
                    "h|help", "Display help",
                    v => _isHelp = v != null
                }
            };
        }

        public void PrintUsage()
        {
            Console.WriteLine($"Usage: sscms {CommandName} <pluginId>");
            Console.WriteLine("Summary: show plugin metadata");
            Console.WriteLine("Options:");
            _options.WriteOptionDescriptions(Console.Out);
            Console.WriteLine();
        }

        public async Task ExecuteAsync(IPluginJobContext context)
        {
            if (!CliUtils.ParseArgs(_options, context.Args)) return;

            if (_isHelp)
            {
                PrintUsage();
                return;
            }

            var (success, result, failureMessage) = _apiService.PluginShow(string.Join(' ', context.Extras));

            if (success)
            {
                await WriteUtils.PrintSuccessAsync(TranslateUtils.JsonSerialize(result));
            }
            else
            {
                await WriteUtils.PrintErrorAsync(failureMessage);
            }
        }
    }
}
