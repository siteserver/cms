using System;
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
            catch (OptionException e)
            {
                PrintError(e.Message);
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
            Console.Error.WriteLine($"error: {errorMessage}");
            Console.Error.WriteLine("Try '--help' for more information.");
        }
    }
}
