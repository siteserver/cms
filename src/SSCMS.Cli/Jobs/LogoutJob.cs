using System;
using System.Threading.Tasks;
using Mono.Options;
using SSCMS.Cli.Abstractions;
using SSCMS.Cli.Core;
using SSCMS.Cli.Models;

namespace SSCMS.Cli.Jobs
{
    public class LogoutJob : IJobService
    {
        public string CommandName => "logout";

        private bool _isHelp;

        private readonly IConfigService _configService;
        private readonly OptionSet _options;

        public LogoutJob(IConfigService configService)
        {
            _configService = configService;
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
            Console.WriteLine("Summary: user logout");
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

            var status = _configService.Status;
            if (status == null || string.IsNullOrEmpty(status.UserName) || string.IsNullOrEmpty(status.AccessToken))
            {
                await WriteUtils.PrintErrorAsync("you have not logged in");
                return;
            }

            status = new ConfigStatus
            {
                UserName = string.Empty,
                AccessToken = string.Empty
            };

            await _configService.SaveStatusAsync(status);

            await WriteUtils.PrintSuccessAsync("you have successful logged out");
        }
    }
}