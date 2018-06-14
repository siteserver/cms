using System;
using System.Collections.Generic;
using System.Text;
using NDesk.Options;
using SiteServer.Utils;

namespace SiteServer.Cli.Core
{
    public static class CliUtils
    {
        public const int PageSize = 500;

        public static readonly string PhysicalApplicationPath = Environment.CurrentDirectory;

        private const int ConsoleTableWidth = 77;

        public static ConfigInfo LoadConfigByFile(string configFileName)
        {
            ConfigInfo configInfo = null;

            if (string.IsNullOrEmpty(configFileName))
            {
                configFileName = "cli.json";
            }

            if (FileUtils.IsFileExists(PathUtils.Combine(PhysicalApplicationPath, configFileName)))
            {
                configInfo = TranslateUtils.JsonDeserialize<ConfigInfo>(
                    FileUtils.ReadText(PathUtils.Combine(PhysicalApplicationPath, configFileName), Encoding.UTF8));

                if (configInfo != null)
                {
                    WebConfigUtils.Load(PhysicalApplicationPath, configInfo.DatabaseType, configInfo.ConnectionString);

                    if (configInfo.BackupConfig == null)
                    {
                        configInfo.BackupConfig = new BackupConfigInfo();
                    }
                    if (configInfo.RestoreConfig == null)
                    {
                        configInfo.RestoreConfig = new RestoreConfigInfo();
                    }
                }
            }
            else if (FileUtils.IsFileExists(PathUtils.Combine(PhysicalApplicationPath, "web.config")))
            {
                WebConfigUtils.Load(PhysicalApplicationPath, "web.config");

                configInfo = new ConfigInfo
                {
                    DatabaseType = WebConfigUtils.DatabaseType.Value,
                    ConnectionString = WebConfigUtils.ConnectionString,
                    BackupConfig = new BackupConfigInfo(),
                    RestoreConfig = new RestoreConfigInfo(),
                };
            }

            return configInfo;
        }

        public static ConfigInfo LoadConfigByArgs(string databaseType, string connectionString)
        {
            var configInfo = new ConfigInfo
            {
                DatabaseType = databaseType,
                ConnectionString = connectionString,
                BackupConfig = new BackupConfigInfo(),
                RestoreConfig = new RestoreConfigInfo(),
            };

            WebConfigUtils.Load(PhysicalApplicationPath, configInfo.DatabaseType, configInfo.ConnectionString);

            return configInfo;
        }

        private static string AlignCentre(string text, int width)
        {
            text = text.Length > width ? text.Substring(0, width - 3) + "..." : text;

            return string.IsNullOrEmpty(text)
                ? new string(' ', width)
                : text.PadRight(width - (width - text.Length) / 2).PadLeft(width);
        }

        // https://stackoverflow.com/questions/491595/best-way-to-parse-command-line-arguments-in-c
        public static bool ParseArgs(OptionSet options, string[] args)
        {
            try
            {
                options.Parse(args);
                return true;
            }
            catch (OptionException ex)
            {
                PrintError(ex.Message);
                return false;
            }
        }

        public static void PrintLine()
        {
            Console.WriteLine(new string('-', ConsoleTableWidth));
        }

        public static void PrintRow(params string[] columns)
        {
            int width = (ConsoleTableWidth - columns.Length) / columns.Length;
            string row = "|";

            foreach (string column in columns)
            {
                row += AlignCentre(column, width) + "|";
            }

            Console.WriteLine(row);
        }

        public static void PrintError(string errorMessage)
        {
            Console.WriteLine();
            Console.Error.WriteLine(errorMessage);
        }

        public static string CreateErrorLogFile(string commandName)
        {
            var filePath = PathUtils.Combine(PhysicalApplicationPath, $"{commandName}.error.log");
            FileUtils.DeleteFileIfExists(filePath);
            return filePath;
        }

        public static void AppendErrorLogs(string filePath, List<TextLogInfo> logs)
        {
            if (logs == null || logs.Count <= 0) return;

            if (!FileUtils.IsFileExists(filePath))
            {
                FileUtils.WriteText(filePath, Encoding.UTF8, string.Empty);
            }

            var builder = new StringBuilder();

            foreach (var log in logs)
            {
                builder.AppendLine();
                builder.Append(log);
                builder.AppendLine();
            }

            FileUtils.AppendText(filePath, Encoding.UTF8, builder.ToString());
        }
    }
}
