using System;
using System.Threading.Tasks;
using Mono.Options;
using SSCMS.Cli.Abstractions;
using SSCMS.Cli.Core;

namespace SSCMS.Cli.Jobs
{
    public class LoginJob : IJobService
    {
        public string CommandName => "login";

        private string _account;
        private string _password;
        private bool _isHelp;

        private readonly IApiService _apiService;
        private readonly OptionSet _options;

        public LoginJob(IApiService apiService)
        {
            _apiService = apiService;
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
                    "h|help", "命令说明",
                    v => _isHelp = v != null
                }
            };
        }

        public void PrintUsage()
        {
            Console.WriteLine($"Usage: sscms-cli {CommandName}");
            Console.WriteLine("Summary: user login");
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

            if (string.IsNullOrEmpty(_account))
            {
                await WriteUtils.PrintErrorAsync("missing required options '--username' or '--mobile' or 'email'");
                return;
            }

            if (string.IsNullOrEmpty(_password))
            {
                await WriteUtils.PrintErrorAsync("missing required options '--password'");
                return;
            }

            var (success, failureMessage) = await _apiService.LoginAsync(_account, _password);
            if (success)
            {
                await WriteUtils.PrintSuccessAsync("you have successful logged in");
            }
            else
            {
                await WriteUtils.PrintErrorAsync(failureMessage);
            }
        }
    }
}