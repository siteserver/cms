using System;
using System.Collections.Generic;
using System.Text;
using NDesk.Options;
using SiteServer.Utils;

namespace SiteServer.Cli.Core
{
    public static class CliUtils
    {
        private const string DirectoryDatabase = "Database";
        private const string DirectoryFiles = "Files";

        public static readonly string PhysicalApplicationPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

        public static string GetBackupDirectoryPath(string folderName)
        {
            return PathUtils.Combine(PhysicalApplicationPath, folderName);
        }

        public static string GetTablesFilePath(string folderName)
        {
            return PathUtils.Combine(GetBackupDirectoryPath(folderName), DirectoryDatabase, "_tables.json");
        }

        public static string GetTableMetadataFilePath(string folderName, string tableName)
        {
            return PathUtils.Combine(GetBackupDirectoryPath(folderName), DirectoryDatabase, tableName, "_metadata.json");
        }

        public static string GetTableContentFilePath(string folderName, string tableName, string fileName)
        {
            return PathUtils.Combine(GetBackupDirectoryPath(folderName), DirectoryDatabase, tableName, fileName);
        }

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
            catch (OptionException ex)
            {
                PrintError(ex);
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

        public static void PrintError(Exception ex)
        {
            Console.Error.WriteLine($"error: {StringUtils.RemoveNewline(ex.Message)}");
            Console.Error.WriteLine("Try '--help' for more information.");
        }

        public static void LogErrors(List<Exception> exceptions)
        {
            var builder = new StringBuilder();
            if (exceptions != null && exceptions.Count > 0)
            {
                foreach (var exception in exceptions)
                {
                    builder.Append($"Message: {exception.Message}");
                    builder.Append($"StackTrace: {exception.StackTrace}");
                    builder.AppendLine();
                }
            }
            FileUtils.WriteText(PathUtils.Combine(CliUtils.PhysicalApplicationPath, "error.log"), Encoding.UTF8, builder.ToString());
        }
    }
}
