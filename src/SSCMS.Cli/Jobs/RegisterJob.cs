using System;
using System.Threading.Tasks;
using Mono.Options;
using SSCMS.Cli.Abstractions;
using SSCMS.Cli.Core;
using SSCMS.Plugins;
using SSCMS.Utils;

namespace SSCMS.Cli.Jobs
{
    public class RegisterJob : IJobService
    {
        public string CommandName => "register";

        private string _userName;
        private string _mobile;
        private string _email;
        private string _password;
        private bool _isHelp;
        private readonly OptionSet _options;

        private readonly ICliApiService _cliApiService;

        public RegisterJob(ICliApiService cliApiService)
        {
            _options = new OptionSet
            {
                { "u|username=", "用户名",
                    v => _userName = v },
                { "mobile=", "手机号",
                    v => _mobile = v },
                { "email=", "邮箱",
                    v => _email = v },
                { "p|password=", "登录密码",
                    v => _password = v },
                {
                    "h|help", "Display help",
                    v => _isHelp = v != null
                }
            };

            _cliApiService = cliApiService;
        }

        public async Task WriteUsageAsync(IConsoleUtils console)
        {
            await console.WriteLineAsync($"Usage: sscms {CommandName}");
            await console.WriteLineAsync("Summary: register a new user");
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

            if (string.IsNullOrEmpty(_userName))
            {
                await console.WriteErrorAsync("missing required options '--username'");
                return;
            }

            if (!StringUtils.IsStrictName(_userName))
            {
                await console.WriteErrorAsync(
                    $@"Invalid username: ""{_userName}"", string does not match the pattern of ""{StringUtils.StrictNameRegex}""");
                return;
            }

            if (string.IsNullOrEmpty(_password))
            {
                await console.WriteErrorAsync("missing required options '--password'");
                return;
            }

            var (success, failureMessage) = await _cliApiService.RegisterAsync(_userName, _mobile, _email, _password);
            if (success)
            {
                await console.WriteSuccessAsync("you have registered successfully, run sscms login to log in.");
            }
            else
            {
                await console.WriteErrorAsync(failureMessage);
            }
        }
    }
}