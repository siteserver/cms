using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NDesk.Options;
using Newtonsoft.Json.Linq;
using SiteServer.Cli.Core;
using SiteServer.CMS.Core;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.Cli.Jobs
{
    public static class RestoreJob
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

        public static async Task Execute(IJobContext context)
        {
            if (!CliUtils.ParseArgs(Options, context.Args)) return;

            if (_isHelp)
            {
                PrintUsage();
                return;
            }

            if (string.IsNullOrEmpty(_directory))
            {
                await CliUtils.PrintErrorAsync("Error, the restore {directory} name is empty");
                return;
            }

            var treeInfo = new TreeInfo(_directory);

            if (!DirectoryUtils.IsDirectoryExists(treeInfo.DirectoryPath))
            {
                await CliUtils.PrintErrorAsync($"Error, directory {treeInfo.DirectoryPath} not exists");
                return;
            }

            var tablesFilePath = treeInfo.TablesFilePath;
            if (!FileUtils.IsFileExists(tablesFilePath))
            {
                await CliUtils.PrintErrorAsync($"Error, file {treeInfo.TablesFilePath} not exists");
                return;
            }

            ConfigInfo configInfo;
            if (!string.IsNullOrEmpty(_databaseType) && !string.IsNullOrEmpty(_connectionString))
            {
                configInfo = CliUtils.LoadConfigByArgs(_databaseType, _connectionString);
            }
            else
            {
                configInfo = await CliUtils.LoadConfigByFileAsync(_configFileName);
            }

            if (configInfo == null)
            {
                await CliUtils.PrintErrorAsync("Error, config not exists");
                return;
            }

            if (string.IsNullOrEmpty(WebConfigUtils.ConnectionString))
            {
                await CliUtils.PrintErrorAsync("Error, connection string is empty");
                return;
            }

            if (!DataProvider.DatabaseDao.IsConnectionStringWork(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString))
            {
                await CliUtils.PrintErrorAsync("Error, can not connect to the database");
                return;
            }

            await Console.Out.WriteLineAsync($"Database Type: {WebConfigUtils.DatabaseType.Value}");
            await Console.Out.WriteLineAsync($"Connection String: {WebConfigUtils.ConnectionString}");
            await Console.Out.WriteLineAsync($"Restore Directory: {treeInfo.DirectoryPath}");

            var tableNames = TranslateUtils.JsonDeserialize<List<string>>(await FileUtils.ReadTextAsync(tablesFilePath, Encoding.UTF8));

            await CliUtils.PrintRowLineAsync();
            await CliUtils.PrintRowAsync("Import Table Name", "Total Count");
            await CliUtils.PrintRowLineAsync();

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

                var tableInfo = TranslateUtils.JsonDeserialize<TableInfo>(await FileUtils.ReadTextAsync(metadataFilePath, Encoding.UTF8));

                await CliUtils.PrintRowAsync(tableName, tableInfo.TotalCount.ToString("#,0"));

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

                        var objects = TranslateUtils.JsonDeserialize<List<JObject>>(
                            await FileUtils.ReadTextAsync(treeInfo.GetTableContentFilePath(tableName, fileName),
                                Encoding.UTF8));

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

                await CliUtils.AppendErrorLogsAsync(errorLogFilePath, logs);
            }

            await CliUtils.PrintRowLineAsync();

            if (configInfo.RestoreConfig.SyncDatabase)
            {
                SystemManager.SyncDatabase();
            }

            await Console.Out.WriteLineAsync("Well done! Thanks for Using SiteServer Cli Tool");
        }
    }
}
