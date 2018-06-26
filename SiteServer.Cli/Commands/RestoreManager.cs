using System;
using System.Collections.Generic;
using System.Text;
using NDesk.Options;
using Newtonsoft.Json.Linq;
using SiteServer.Cli.Core;
using SiteServer.CMS.Core;
using SiteServer.Utils;

namespace SiteServer.Cli.Commands
{
    public static class RestoreManager
    {
        public const string CommandName = "restore";

        private static bool _isHelp;
        private static string _directory;
        private static string _configFileName;
        private static string _databaseType;
        private static string _connectionString;

        private static readonly OptionSet Options = new OptionSet() {
            { "c|config=", "the {cli.json} file name.",
                v => _configFileName = v },
            { "d|directory=", "the restore {directory} name.",
                v => _directory = v },
            { "database=", "the database type.",
                v => _databaseType = v },
            { "connection=", "the connection string.",
                v => _connectionString = v },
            { "h|help",  "show this message and exit",
                v => _isHelp = v != null }
        };

        public static void PrintUsage()
        {
            Console.WriteLine("Restore command usage: siteserver restore");
            Options.WriteOptionDescriptions(Console.Out);
            Console.WriteLine();
        }

        public static void Execute(string[] args)
        {
            if (!CliUtils.ParseArgs(Options, args)) return;

            if (_isHelp)
            {
                PrintUsage();
                return;
            }

            if (string.IsNullOrEmpty(_directory))
            {
                CliUtils.PrintError("Error, the restore {directory} name is empty");
                return;
            }

            var treeInfo = new TreeInfo(_directory);

            if (!DirectoryUtils.IsDirectoryExists(treeInfo.DirectoryPath))
            {
                CliUtils.PrintError($"Error, directory {treeInfo.DirectoryPath} not exists");
                return;
            }

            var tablesFilePath = treeInfo.TablesFilePath;
            if (!FileUtils.IsFileExists(tablesFilePath))
            {
                CliUtils.PrintError($"Error, file {treeInfo.TablesFilePath} not exists");
                return;
            }

            ConfigInfo configInfo;
            if (!string.IsNullOrEmpty(_databaseType) && !string.IsNullOrEmpty(_connectionString))
            {
                configInfo = CliUtils.LoadConfigByArgs(_databaseType, _connectionString);
            }
            else
            {
                configInfo = CliUtils.LoadConfigByFile(_configFileName);
            }

            if (configInfo == null)
            {
                CliUtils.PrintError("Error, config not exists");
                return;
            }

            if (string.IsNullOrEmpty(WebConfigUtils.ConnectionString))
            {
                CliUtils.PrintError("Error, connection string is empty");
                return;
            }

            if (!DataProvider.DatabaseDao.IsConnectionStringWork(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString))
            {
                CliUtils.PrintError("Error, can not connect to the database");
                return;
            }

            Console.WriteLine($"Database Type: {WebConfigUtils.DatabaseType.Value}");
            Console.WriteLine($"Connection String: {WebConfigUtils.ConnectionString}");
            Console.WriteLine($"Restore Directory: {treeInfo.DirectoryPath}");

            var tableNames = TranslateUtils.JsonDeserialize<List<string>>(FileUtils.ReadText(tablesFilePath, Encoding.UTF8));

            CliUtils.PrintLine();
            CliUtils.PrintRow("Import Table Name", "Total Count");
            CliUtils.PrintLine();

            var errorLogFilePath = CliUtils.CreateErrorLogFile(CommandName);

            foreach (var tableName in tableNames)
            {
                var logs = new List<TextLogInfo>();

                if (configInfo.RestoreConfig.Includes != null)
                {
                    if (!StringUtils.ContainsIgnoreCase(configInfo.RestoreConfig.Includes, tableName)) continue;
                }
                if (configInfo.RestoreConfig.Excludes != null)
                {
                    if (StringUtils.ContainsIgnoreCase(configInfo.RestoreConfig.Excludes, tableName)) continue;
                }

                var metadataFilePath = treeInfo.GetTableMetadataFilePath(tableName);

                if (!FileUtils.IsFileExists(metadataFilePath)) continue;

                var tableInfo = TranslateUtils.JsonDeserialize<TableInfo>(FileUtils.ReadText(metadataFilePath, Encoding.UTF8));

                CliUtils.PrintRow(tableName, tableInfo.TotalCount.ToString("#,0"));

                if (!DataProvider.DatabaseDao.IsTableExists(tableName))
                {
                    if (!DataProvider.DatabaseDao.CreateSystemTable(tableName, tableInfo.Columns, out var ex, out var sqlString))
                    {
                        logs.Add(new TextLogInfo
                        {
                            DateTime = DateTime.Now,
                            Detail = $"create table {tableName}: {sqlString}",
                            Exception = ex
                        });

                        continue;
                    }
                }
                else
                {
                    DataProvider.DatabaseDao.AlterSystemTable(tableName, tableInfo.Columns);
                }

                using (var progress = new ProgressBar())
                {
                    for (var i = 0; i < tableInfo.RowFiles.Count; i++)
                    {
                        progress.Report((double)i / tableInfo.RowFiles.Count);

                        var fileName = tableInfo.RowFiles[i];

                        var objects = TranslateUtils.JsonDeserialize<List<JObject>>(FileUtils.ReadText(treeInfo.GetTableContentFilePath(tableName, fileName), Encoding.UTF8));

                        try
                        {
                            DataProvider.DatabaseDao.InsertMultiple(tableName, objects, tableInfo.Columns);
                        }
                        catch (Exception ex)
                        {
                            logs.Add(new TextLogInfo
                            {
                                DateTime = DateTime.Now,
                                Detail = $"insert table {tableName}, fileName {fileName}",
                                Exception = ex
                            });
                        }
                    }
                }

                CliUtils.AppendErrorLogs(errorLogFilePath, logs);
            }

            CliUtils.PrintLine();

            if (configInfo.RestoreConfig.SyncDatabase)
            {
                SystemManager.SyncDatabase();
            }

            Console.WriteLine("Well done! Thanks for Using SiteServer Cli Tool");
        }
    }
}
