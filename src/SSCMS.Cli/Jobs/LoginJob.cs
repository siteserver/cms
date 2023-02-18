using System;
using System.Threading.Tasks;
using Mono.Options;
using SSCMS.Cli.Abstractions;
using SSCMS.Cli.Core;
using SSCMS.Plugins;
using SSCMS.Utils;

namespace SSCMS.Cli.Jobs
{
    public class LoginJob : IJobService
    {
        public string CommandName => "login";

        private string _account;
        private string _password;
        private bool _isHelp;

        private readonly ICliApiService _cliApiService;
        private readonly OptionSet _options;

        public LoginJob(ICliApiService cliApiService)
        {
            _cliApiService = cliApiService;
            _options = new OptionSet
            {
                { "u|username=", "登录用户名",
                    v => _account = v },
                { "mobile=", "登录手机号",
                    v => _account = v },
                { "email=", "登录邮箱",
                    v => _account = v },
                { "p|password=", "登录密码",
                    v => _password = v },
                {
                    "h|help", "Display help",
                    v => _isHelp = v != null
                }
            };
        }

        public async Task WriteUsageAsync(IConsoleUtils console)
        {
            await console.WriteLineAsync($"Usage: sscms {CommandName}");
            await console.WriteLineAsync("Summary: user login");
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

            if (string.IsNullOrEmpty(_account))
            {
                _account = console.GetString("Username:");
            }

            if (string.IsNullOrEmpty(_password))
            {
                _password = console.GetPassword("Password:");
            }

            var (success, failureMessage) = await _cliApiService.LoginAsync(_account, _password);
            if (success)
            {
                await console.WriteSuccessAsync("you have successful logged in");
            }
            else
            {
                await console.WriteErrorAsync(failureMessage);
            }
        }
    }
}