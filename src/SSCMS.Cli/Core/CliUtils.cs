using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Datory;
using Microsoft.Extensions.DependencyInjection;
using Mono.Options;
using SSCMS.Cli.Abstractions;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Cli.Core
{
    public static class CliUtils
    {
        private static ServiceProvider Provider { get; set; }

        

        public static void SetProvider(ServiceProvider provider)
        {
            Provider = provider;
        }

        public static Application GetApplication()
        {
            return Provider.GetRequiredService<Application>();
        }

        public static IJobService GetJobService(string commandName)
        {
            var services = Provider.GetServices<IJobService>();
            return services.FirstOrDefault(x => StringUtils.EqualsIgnoreCase(x.CommandName, commandName));
        }

        public static List<string> GetJobServiceCommandNames()
        {
            var services = Provider.GetServices<IJobService>();
            return services.Select(x => x.CommandName).ToList();
        }

        public static IEnumerable<IJobService> GetJobServices()
        {
            return Provider.GetServices<IJobService>();
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
            catch
            {
                return false;
            }
        }

        public static async Task PrintRowLineAsync()
        {
            await Console.Out.WriteLineAsync(new string('-', CliConstants.ConsoleTableWidth));
        }

        public static async Task PrintRowAsync(params string[] columns)
        {
            int width = (CliConstants.ConsoleTableWidth - columns.Length) / columns.Length;
            string row = "|";

            foreach (string column in columns)
            {
                row += AlignCentre(column, width) + "|";
            }

            await Console.Out.WriteLineAsync(row);
        }

        public static async Task PrintRowLine()
        {
            await Console.Out.WriteLineAsync(new string('-', CliConstants.ConsoleTableWidth));
        }

        public static async Task PrintRow(params string[] columns)
        {
            int width = (CliConstants.ConsoleTableWidth - columns.Length) / columns.Length;
            string row = "|";

            foreach (string column in columns)
            {
                row += AlignCentre(column, width) + "|";
            }

            await Console.Out.WriteLineAsync(row);
        }

        public static async Task PrintInfoAsync(ISettingsManager settingsManager)
        {
            await Console.Out.WriteLineAsync($"Cli version: {settingsManager.Version}");
            var entryAssembly = Assembly.GetExecutingAssembly();
            await Console.Out.WriteLineAsync($"Cli location: {entryAssembly.Location}");
            await Console.Out.WriteLineAsync($"Work location: {settingsManager.ContentRootPath}");

            var configPath = PathUtils.Combine(settingsManager.ContentRootPath, Constants.ConfigFileName);

            if (FileUtils.IsFileExists(configPath))
            {
                await Console.Out.WriteLineAsync($"Database type: {settingsManager.Database.DatabaseType.GetDisplayName()}");
                await Console.Out.WriteLineAsync($"Database connection string: {settingsManager.DatabaseConnectionString}");
            }
        }

        public static async Task PrintErrorAsync(string errorMessage)
        {
            var backgroundColor = Console.BackgroundColor;
            //var foregroundColor = Console.ForegroundColor;
            Console.BackgroundColor = ConsoleColor.DarkRed;
            //Console.ForegroundColor = ConsoleColor.Black;
            await Console.Out.WriteAsync(" ERROR ");
            Console.BackgroundColor = backgroundColor;
            //Console.ForegroundColor = foregroundColor;

            await Console.Out.WriteAsync($" {errorMessage}");
        }

        public static async Task PrintSuccessAsync(string successMessage)
        {
            var backgroundColor = Console.BackgroundColor;
            //var foregroundColor = Console.ForegroundColor;
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            //Console.ForegroundColor = ConsoleColor.Black;
            await Console.Out.WriteAsync(" SUCCESS ");
            Console.BackgroundColor = backgroundColor;
            //Console.ForegroundColor = foregroundColor;

            await Console.Out.WriteAsync($" {successMessage}");
        }

        public static string CreateErrorLogFile(string commandName, ISettingsManager settingsManager)
        {
            var filePath = PathUtils.Combine(settingsManager.ContentRootPath, $"{commandName}.error.log");
            FileUtils.DeleteFileIfExists(filePath);
            return filePath;
        }

        public static async Task AppendErrorLogsAsync(string filePath, List<TextLogInfo> logs)
        {
            if (logs == null || logs.Count <= 0) return;

            if (!FileUtils.IsFileExists(filePath))
            {
                await FileUtils.WriteTextAsync(filePath, string.Empty);
            }

            var builder = new StringBuilder();

            foreach (var log in logs)
            {
                builder.AppendLine();
                builder.Append(log);
                builder.AppendLine();
            }

            await FileUtils.AppendTextAsync(filePath, Encoding.UTF8, builder.ToString());
        }

        public static async Task AppendErrorLogAsync(string filePath, TextLogInfo log)
        {
            if (log == null) return;

            if (!FileUtils.IsFileExists(filePath))
            {
                await FileUtils.WriteTextAsync(filePath, string.Empty);
            }

            var builder = new StringBuilder();

            builder.AppendLine();
            builder.Append(log);
            builder.AppendLine();

            await FileUtils.AppendTextAsync(filePath, Encoding.UTF8, builder.ToString());
        }

        public static string GetWebConfigPath(string configFile, ISettingsManager settingsManager)
        {
            return PathUtils.IsFilePath(configFile)
                ? configFile
                : PathUtils.Combine(settingsManager.ContentRootPath,
                    !string.IsNullOrEmpty(configFile) ? configFile : Constants.ConfigFileName);
        }
    }
}
