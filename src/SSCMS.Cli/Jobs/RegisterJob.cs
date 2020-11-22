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

        private readonly IApiService _apiService;

        public RegisterJob(IApiService apiService)
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

            _apiService = apiService;
        }

        public void PrintUsage()
        {
            Console.WriteLine($"Usage: sscms {CommandName}");
            Console.WriteLine("Summary: register a new user");
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

            if (string.IsNullOrEmpty(_userName))
            {
                await WriteUtils.PrintErrorAsync("missing required options '--username'");
                return;
            }

            if (!StringUtils.IsStrictName(_userName))
            {
                await WriteUtils.PrintErrorAsync(
                    $@"Invalid username: ""{_userName}"", string does not match the pattern of ""{StringUtils.StrictNameRegex}""");
                return;
            }

            if (string.IsNullOrEmpty(_password))
            {
                await WriteUtils.PrintErrorAsync("missing required options '--password'");
                return;
            }

            var (success, failureMessage) = _apiService.Register(_userName, _mobile, _email, _password);
            if (success)
            {
                await WriteUtils.PrintSuccessAsync("you have registered successfully, run sscms login to log in.");
            }
            else
            {
                await WriteUtils.PrintErrorAsync(failureMessage);
            }
        }
    }
}