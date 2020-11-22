using System;
using System.Threading.Tasks;
using Mono.Options;
using SSCMS.Cli.Abstractions;
using SSCMS.Cli.Core;
using SSCMS.Plugins;

namespace SSCMS.Cli.Jobs
{
    public class ThemeUnPublishJob : IJobService
    {
        public string CommandName => "theme unpublish";

        private string _name;
        private bool _isHelp;
        private readonly OptionSet _options;

        private readonly IApiService _apiService;

        public ThemeUnPublishJob(IApiService apiService)
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

            _apiService = apiService;
        }

        public void PrintUsage()
        {
            Console.WriteLine($"Usage: sscms {CommandName} <pluginId>");
            Console.WriteLine("Summary: unpublishes a theme.");
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

            if (string.IsNullOrEmpty(_name))
            {
                if (context.Extras != null && context.Extras.Length > 0)
                {
                    _name = context.Extras[0];
                }
            }
            if (string.IsNullOrEmpty(_name))
            {
                await WriteUtils.PrintErrorAsync("missing required name");
                return;
            }

            var (status, failureMessage) = _apiService.GetStatus();
            if (status == null)
            {
                await WriteUtils.PrintErrorAsync(failureMessage);
                return;
            }

            bool success;
            (success, failureMessage) = _apiService.ThemeUnPublish(_name);
            if (success)
            {
                await WriteUtils.PrintSuccessAsync($"Theme {_name} unpublished .");
            }
            else
            {
                await WriteUtils.PrintErrorAsync(failureMessage);
            }
        }
    }
}
