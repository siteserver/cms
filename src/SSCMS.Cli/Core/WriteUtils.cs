using System;
using System.Reflection;
using System.Threading.Tasks;
using Datory;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Cli.Core
{
    public static class WriteUtils
    {
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

        private static string AlignCentre(string text, int width)
        {
            text = text.Length > width ? text.Substring(0, width - 3) + "..." : text;

            return string.IsNullOrEmpty(text)
                ? new string(' ', width)
                : text.PadRight(width - (width - text.Length) / 2).PadLeft(width);
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
    }
}
