using System;
using System.Threading.Tasks;
using NDesk.Options;
using SiteServer.Cli.Core;
using SiteServer.CMS.Core;
using SiteServer.Plugin;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.Cli.Jobs
{
    public static class InstallJob
    {
        public const string CommandName = "install";

        private static bool _isHelp;
        private static string _userName;
        private static string _password;

        private static readonly OptionSet Options = new OptionSet {
            { "u|userName=", "管理员用户名",
                v => _userName = v },
            { "p|password=", "管理员密码",
                v => _password = v },
            { "h|help",  "命令说明",
                v => _isHelp = v != null }
        };

        public static void PrintUsage()
        {
            Console.WriteLine("安装系统: siteserver install");
            Options.WriteOptionDescriptions(Console.Out);
            Console.WriteLine();
        }

        public static async Task Execute(IJobContext context)
        {
            if (!CliUtils.ParseArgs(Options, context.Args)) return;

            if (_isHelp)
            {
                PrintUsage();
                return;
            }

            try
            {
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

                var webConfigPath = PathUtils.Combine(CliUtils.PhysicalApplicationPath, "web.config");
                if (!FileUtils.IsFileExists(webConfigPath))
                {
                    await CliUtils.PrintErrorAsync($"系统配置文件不存在：{webConfigPath}！");
                    return;
                }

                if (string.IsNullOrEmpty(WebConfigUtils.ConnectionString))
                {
                    await CliUtils.PrintErrorAsync("web.config 中数据库连接字符串 connectionString 未设置");
                    return;
                }

                WebConfigUtils.Load(CliUtils.PhysicalApplicationPath, "web.config");

                await Console.Out.WriteLineAsync($"数据库类型: {WebConfigUtils.DatabaseType.Value}");
                await Console.Out.WriteLineAsync($"连接字符串: {WebConfigUtils.ConnectionString}");
                await Console.Out.WriteLineAsync($"系统文件夹: {CliUtils.PhysicalApplicationPath}");

                if (!DataProvider.DatabaseDao.IsConnectionStringWork(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString))
                {
                    await CliUtils.PrintErrorAsync("系统无法连接到 web.config 中设置的数据库");
                    return;
                }

                if (!SystemManager.IsNeedInstall())
                {
                    await CliUtils.PrintErrorAsync("系统已安装在 web.config 指定的数据库中，命令执行失败");
                    return;
                }

                WebConfigUtils.UpdateWebConfig(WebConfigUtils.IsProtectData, WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString, WebConfigUtils.ApiPrefix, WebConfigUtils.AdminDirectory, WebConfigUtils.HomeDirectory, StringUtils.GetShortGuid(), false);

                DataProvider.Reset();

                SystemManager.InstallDatabase(_userName, _password);
            }
            catch (Exception e)
            {
                await CliUtils.PrintErrorAsync(e.Message);
                return;
            }

            await Console.Out.WriteLineAsync("恭喜，系统安装成功！");
        }
    }
}
