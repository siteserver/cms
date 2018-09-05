using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NDesk.Options;
using SiteServer.Utils;

namespace SiteServer.Cli.Core
{
    public static class CliUtils
    {
        public const int PageSize = 500;

        public static readonly string PhysicalApplicationPath = Environment.CurrentDirectory;

        private const int ConsoleTableWidth = 77;

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
            await Console.Out.WriteLineAsync(new string('-', ConsoleTableWidth));
        }

        public static async Task PrintRowAsync(params string[] columns)
        {
            int width = (ConsoleTableWidth - columns.Length) / columns.Length;
            string row = "|";

            foreach (string column in columns)
            {
                row += AlignCentre(column, width) + "|";
            }

            await Console.Out.WriteLineAsync(row);
        }

        public static async Task PrintErrorAsync(string errorMessage)
        {
            await Console.Out.WriteLineAsync();
            await Console.Out.WriteLineAsync(errorMessage);
        }

        public static async Task PrintRowLine()
        {
            await Console.Out.WriteLineAsync(new string('-', ConsoleTableWidth));
        }

        public static async Task PrintRow(params string[] columns)
        {
            int width = (ConsoleTableWidth - columns.Length) / columns.Length;
            string row = "|";

            foreach (string column in columns)
            {
                row += AlignCentre(column, width) + "|";
            }

            await Console.Out.WriteLineAsync(row);
        }

        public static async Task PrintError(string errorMessage)
        {
            await Console.Out.WriteLineAsync();
            await Console.Out.WriteLineAsync(errorMessage);
        }

        public static string CreateErrorLogFile(string commandName)
        {
            var filePath = PathUtils.Combine(PhysicalApplicationPath, $"{commandName}.error.log");
            FileUtils.DeleteFileIfExists(filePath);
            return filePath;
        }

        public static async Task AppendErrorLogsAsync(string filePath, List<TextLogInfo> logs)
        {
            if (logs == null || logs.Count <= 0) return;

            if (!FileUtils.IsFileExists(filePath))
            {
                await FileUtils.WriteTextAsync(filePath, Encoding.UTF8, string.Empty);
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
                await FileUtils.WriteTextAsync(filePath, Encoding.UTF8, string.Empty);
            }

            var builder = new StringBuilder();

            builder.AppendLine();
            builder.Append(log);
            builder.AppendLine();

            await FileUtils.AppendTextAsync(filePath, Encoding.UTF8, builder.ToString());
        }
    }
}
