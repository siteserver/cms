using System;
using System.Threading.Tasks;
using Mono.Options;
using SSCMS.Cli.Abstractions;
using SSCMS.Cli.Core;
using SSCMS.Plugins;
using SSCMS.Utils;

namespace SSCMS.Cli.Jobs
{
    public class PluginSearchJob : IJobService
    {
        public string CommandName => "plugin search";

        private bool _isHelp;

        private readonly IApiService _apiService;
        private readonly OptionSet _options;

        public PluginSearchJob(IApiService apiService)
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
            Console.WriteLine($"Usage: sscms {CommandName} <word>");
            Console.WriteLine("Summary: search plugins by word");
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

            var (success, pluginAndUserList, failureMessage) = _apiService.PluginSearch(string.Join(' ', context.Extras));

            if (success)
            {
                await WriteUtils.PrintSuccessAsync(TranslateUtils.JsonSerialize(pluginAndUserList));
            }
            else
            {
                await WriteUtils.PrintErrorAsync(failureMessage);
            }
        }
    }
}
