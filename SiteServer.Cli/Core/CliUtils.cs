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

        public static ConfigInfo LoadConfig(string configFileName)
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

                WebConfigUtils.Load(PhysicalApplicationPath, configInfo.RestoreConfig.DatabaseType, configInfo.RestoreConfig.ConnectionString);
            }
            else if (FileUtils.IsFileExists(PathUtils.Combine(PhysicalApplicationPath, "web.config")))
            {
                configInfo = new ConfigInfo();
                WebConfigUtils.Load(PhysicalApplicationPath, "web.config");
            }

            if (configInfo != null)
            {
                if (configInfo.BackupConfig == null)
                {
                    configInfo.BackupConfig = new BackupConfigInfo();
                }
                if (configInfo.RestoreConfig == null)
                {
                    configInfo.RestoreConfig = new RestoreConfigInfo();
                }
            }

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

        public static void LogErrors(string commandName, List<TextLogInfo> logs)
        {
            var builder = new StringBuilder();
            if (logs != null && logs.Count > 0)
            {
                foreach (var log in logs)
                {
                    builder.AppendLine();
                    builder.Append(log);
                    builder.AppendLine();
                }
            }
            FileUtils.WriteText(PathUtils.Combine(PhysicalApplicationPath, $"{commandName}.error.log"), Encoding.UTF8, builder.ToString());
        }
    }
}
