using System;
using System.Collections.Generic;
using System.Text;
using SiteServer.Cli.Commands;
using SiteServer.Cli.Core;
using SiteServer.Utils;

namespace SiteServer.Cli
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            var commandName = string.Empty;
            var commandArgs = new List<string>();

            if (args.Length >= 1)
            {
                for (var i = 0; i < args.Length; i++)
                {
                    if (i == 0)
                    {
                        commandName = args[i];
                    }
                    else
                    {
                        commandArgs.Add(args[i]);
                    }
                }
            }

            Console.WriteLine("Welcome to SiteServer Cli Tool");
            Console.WriteLine();

            try
            {
                if (StringUtils.EqualsIgnoreCase(commandName, BackupManager.CommandName))
                {
                    BackupManager.Execute(commandArgs.ToArray());
                }
                else if (StringUtils.EqualsIgnoreCase(commandName, RestoreManager.CommandName))
                {
                    RestoreManager.Execute(commandArgs.ToArray());
                }
                else if (StringUtils.EqualsIgnoreCase(commandName, UpdateManager.CommandName))
                {
                    UpdateManager.Execute(commandArgs.ToArray());
                }
                else if (StringUtils.EqualsIgnoreCase(commandName, VersionManager.CommandName))
                {
                    VersionManager.Execute(commandArgs);
                }
                else
                {
                    CliUtils.PrintLine();
                    CliUtils.PrintRow("Usage");
                    CliUtils.PrintLine();
                    BackupManager.PrintUsage();
                    RestoreManager.PrintUsage();
                    UpdateManager.PrintUsage();
                    CliUtils.PrintLine();
                    CliUtils.PrintRow("http://www.siteserver.cn/docs/cli");
                    CliUtils.PrintLine();
                    Console.ReadLine();
                }
            }
            catch (Exception ex)
            {
                CliUtils.PrintError(ex.Message);
                CliUtils.LogErrors(commandName, new List<TextLogInfo> {new TextLogInfo
                {
                    DateTime = DateTime.Now,
                    Detail = "Console Error",
                    Exception = ex
                }});
            }
        }
    }
}
