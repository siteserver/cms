using System;
using System.Linq;
using System.Threading.Tasks;

namespace SSCMS.Cli.Core
{
    public static class WriteUtils
    {
        public static async Task PrintSuccessAsync(string successMessage)
        {
            var backgroundColor = Console.BackgroundColor;
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            await Console.Out.WriteAsync(" SUCCESS ");
            Console.BackgroundColor = backgroundColor;

            await Console.Out.WriteAsync($" {successMessage}");
            await Console.Out.WriteLineAsync();
        }

        public static async Task PrintErrorAsync(string errorMessage)
        {
            var backgroundColor = Console.BackgroundColor;
            Console.BackgroundColor = ConsoleColor.DarkRed;
            await Console.Out.WriteAsync(" ERROR ");
            Console.BackgroundColor = backgroundColor;

            await Console.Out.WriteAsync($" {errorMessage}");
            await Console.Out.WriteLineAsync();
        }

        public static async Task PrintRowLineAsync()
        {
            await Console.Out.WriteLineAsync(new string('-', CliConstants.ConsoleTableWidth));
        }

        public static async Task PrintRowAsync(params string[] columns)
        {
            var width = (CliConstants.ConsoleTableWidth - columns.Length) / columns.Length;
            var row = columns.Aggregate("|", (current, column) => current + AlignCentre(column, width) + "|");

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
            var width = (CliConstants.ConsoleTableWidth - columns.Length) / columns.Length;
            var row = columns.Aggregate("|", (current, column) => current + AlignCentre(column, width) + "|");

            await Console.Out.WriteLineAsync(row);
        }
    }
}
