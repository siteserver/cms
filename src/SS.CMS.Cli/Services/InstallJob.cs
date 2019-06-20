using System;
using System.Threading.Tasks;
using Mono.Options;
using SS.CMS.Cli.Core;
using SS.CMS.Repositories;
using SS.CMS.Services;
using SS.CMS.Utils.Enumerations;
using Microsoft.Extensions.DependencyInjection;

namespace SS.CMS.Cli.Services
{
    public class InstallJob
    {
        public const string CommandName = "install";

        public static async Task Execute(IJobContext context)
        {
            var application = CliUtils.Provider.GetService<InstallJob>();
            await application.RunAsync(context);
        }

        public static void PrintUsage()
        {
            Console.WriteLine("系统安装: siteserver install");
            var job = new InstallJob(null, null);
            job._options.WriteOptionDescriptions(Console.Out);
            Console.WriteLine();
        }

        private string _configFile;
        private string _databaseType;
        private string _connectionString;
        private string _userName;
        private string _password;
        private bool _isHelp;
        private readonly OptionSet _options;
        private IConfigRepository _configRepository;
        private ITableManager _tableManager;

        public InstallJob(IConfigRepository configRepository, ITableManager tableManager)
        {
            _configRepository = configRepository;
            _tableManager = tableManager;

            _options = new OptionSet {
                { "c|config-file=", "指定配置文件Web.config路径或文件名",
                    v => _configFile = v },
                { "database-type=", "指定需要安装的数据库类型",
                    v => _databaseType = v },
                { "connection-string=", "指定需要安装的数据库连接字符串",
                    v => _connectionString = v },
                { "u|userName=", "超级管理员用户名",
                    v => _userName = v },
                { "p|password=", "超级管理员密码",
                    v => _password = v },
                { "h|help",  "命令说明",
                    v => _isHelp = v != null }
            };
        }

        public async Task RunAsync(IJobContext context)
        {
            if (!CliUtils.ParseArgs(_options, context.Args)) return;

            if (_isHelp)
            {
                PrintUsage();
                return;
            }

            var db = CliUtils.GetDatabase(_databaseType, _connectionString, _configFile, out var errorMessage);
            if (db == null)
            {
                await CliUtils.PrintErrorAsync(errorMessage);
                return;
            }

            if (string.IsNullOrEmpty(_userName))
            {
                await CliUtils.PrintErrorAsync("未设置参数管理员用户名：{userName} ！");
                return;
            }

            if (string.IsNullOrEmpty(_password))
            {
                await CliUtils.PrintErrorAsync("未设置参数管理员密码：{password} ！");
                return;
            }

            if (_password.Length < 6)
            {
                await CliUtils.PrintErrorAsync("管理员密码必须大于6位 ！");
                return;
            }

            if (!EUserPasswordRestrictionUtils.IsValid(_password, EUserPasswordRestrictionUtils.GetValue(EUserPasswordRestriction.LetterAndDigit)))
            {
                await CliUtils.PrintErrorAsync($"管理员密码不符合规则，请包含{EUserPasswordRestrictionUtils.GetText(EUserPasswordRestriction.LetterAndDigit)}");
                return;
            }

            await Console.Out.WriteLineAsync($"数据库类型: {db.DatabaseType.Value}");
            await Console.Out.WriteLineAsync($"连接字符串: {db.ConnectionString}");
            await Console.Out.WriteLineAsync($"系统文件夹: {CliUtils.PhysicalApplicationPath}");

            if (_configRepository.Instance != null)
            {
                await CliUtils.PrintErrorAsync("系统已安装在 web.config 指定的数据库中，命令执行失败");
                return;
            }

            // db.UpdateWebConfig(db.IsProtectData, db.DatabaseType, db.ConnectionString, db.ApiPrefix, db.AdminDirectory, db.HomeDirectory, StringUtils.GetShortGuid(), false);

            // DataProvider.Reset();

            _tableManager.InstallDatabase(_userName, _password);

            await Console.Out.WriteLineAsync("恭喜，系统安装成功！");
        }
    }
}
