using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using Mono.Options;
using SSCMS.Cli.Abstractions;
using SSCMS.Cli.Core;
using SSCMS.Plugins;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Cli.Jobs
{
    public class DataBackupJob : IJobService
    {
        public string CommandName => "data backup";

        private string _databaseType;
        private string _connectionString;
        private string _directory;
        private List<string> _includes;
        private List<string> _excludes;
        private int _maxRows;
        private bool _isHelp;

        private readonly ISettingsManager _settingsManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly OptionSet _options;

        public DataBackupJob(ISettingsManager settingsManager, IDatabaseManager databaseManager)
        {
            _settingsManager = settingsManager;
            _databaseManager = databaseManager;
            _options = new OptionSet
            {
                {
                    "t|database-type=", "Database type",
                    v => _databaseType = v
                },
                {
                    "c|connection-string=", "Database connection string",
                    v => _connectionString = v
                },
                {
                    "d|directory=", "Backup folder name",
                    v => _directory = v
                },
                {
                    "includes=", "Include table names, separated by commas, default backup all tables",
                    v => _includes = v == null ? null : ListUtils.GetStringList(v)
                },
                {
                    "excludes=", "Exclude table names, separated by commas",
                    v => _excludes = v == null ? null : ListUtils.GetStringList(v)
                },
                {
                    "max-rows=", "Maximum number of rows to backup. All data is backed up by default",
                    v => _maxRows = v == null ? 0 : TranslateUtils.ToInt(v)
                },
                {
                    "h|help", "Display help",
                    v => _isHelp = v != null
                }
            };
        }

        public void PrintUsage()
        {
            Console.WriteLine($"Usage: sscms {CommandName}");
            Console.WriteLine("Summary: backup database to folder");
            Console.WriteLine("Options:");
            _options.WriteOptionDescriptions(Console.Out);
            Console.WriteLine();
        }

        public async Task ExecuteAsync(IPluginJobContext context)
        {
            if (!CliUtils.ParseArgs(_options, context.Args)) return;

            if (_isHelp)
            {
                PrintUsage();
                return;
            }

            var directory = _directory;
            if (string.IsNullOrEmpty(directory))
            {
                directory = $"backup/{DateTime.Now:yyyy-MM-dd}";
            }

            var treeInfo = new TreeInfo(_settingsManager, directory);
            DirectoryUtils.CreateDirectoryIfNotExists(treeInfo.DirectoryPath);

            var configPath = CliUtils.GetConfigPath(_settingsManager);

            var databaseType = TranslateUtils.ToEnum(_databaseType, DatabaseType.MySql);
            var connectionString = _connectionString;
            if (string.IsNullOrEmpty(connectionString))
            {
                if (FileUtils.IsFileExists(configPath))
                {
                    databaseType = _settingsManager.Database.DatabaseType;
                    connectionString = _settingsManager.DatabaseConnectionString;
                }
                else
                {
                    await WriteUtils.PrintErrorAsync($"The sscms.json file does not exist: {configPath}");
                    return;
                }
            }

            await Console.Out.WriteLineAsync($"Database type: {databaseType.GetDisplayName()}");
            await Console.Out.WriteLineAsync($"Database connection string: {connectionString}");
            await Console.Out.WriteLineAsync($"Backup folder: {treeInfo.DirectoryPath}");

            //WebConfigUtils.Load(_settingsManager.ContentRootPath, webConfigPath);

            //if (string.IsNullOrEmpty(WebConfigUtils.ConnectionString))
            //{
            //    await CliUtils.PrintErrorAsync($"{webConfigPath} 中数据库连接字符串 connectionString 未设置");
            //    return;
            //}

            //await Console.Out.WriteLineAsync($"数据库类型: {_settingsManager.Database.DatabaseType.GetValue()}");
            //await Console.Out.WriteLineAsync($"连接字符串: {WebConfigUtils.ConnectionString}");
            //await Console.Out.WriteLineAsync($"备份文件夹: {treeInfo.DirectoryPath}");

            //var (isConnectionWorks, errorMessage) = await _settingsManager.Database.IsConnectionWorksAsync();
            //if (!isConnectionWorks)
            //{
            //    await CliUtils.PrintErrorAsync($"数据库连接错误：{errorMessage}");
            //    return;
            //}

            var database = new Database(databaseType, connectionString);
            var (isConnectionWorks, errorMessage) = await database.IsConnectionWorksAsync();
            if (!isConnectionWorks)
            {
                await WriteUtils.PrintErrorAsync($"Unable to connect to database, error message:{errorMessage}");
                return;
            }

            if (_excludes == null)
            {
                _excludes = new List<string>();
            }
            _excludes.Add("bairong_Log");
            _excludes.Add("bairong_ErrorLog");
            _excludes.Add("siteserver_ErrorLog");
            _excludes.Add("siteserver_Log");
            _excludes.Add("siteserver_Tracking");

            await Backup(database, _databaseManager, _includes, _excludes, _maxRows, treeInfo);

            await WriteUtils.PrintRowLineAsync();
            await WriteUtils.PrintSuccessAsync("backup database to folder successfully!");
        }

        public static async Task Backup(IDatabase database, IDatabaseManager databaseManager, List<string> includes, List<string> excludes, int maxRows, TreeInfo treeInfo)
        {
            var allTableNames = await database.GetTableNamesAsync();

            var tableNames = new List<string>();

            foreach (var tableName in allTableNames)
            {
                if (includes != null && !ListUtils.ContainsIgnoreCase(includes, tableName)) continue;
                if (ListUtils.ContainsIgnoreCase(excludes, tableName)) continue;
                if (ListUtils.ContainsIgnoreCase(tableNames, tableName)) continue;
                tableNames.Add(tableName);
            }

            await FileUtils.WriteTextAsync(treeInfo.TablesFilePath, TranslateUtils.JsonSerialize(tableNames));

            await WriteUtils.PrintRowLineAsync();
            await WriteUtils.PrintRowAsync("Backup table name", "Count");
            await WriteUtils.PrintRowLineAsync();

            foreach (var tableName in tableNames)
            {
                var columns = await database.GetTableColumnsAsync(tableName);
                var repository = new Repository(database, tableName, columns);

                var tableInfo = new TableInfo
                {
                    Columns = repository.TableColumns,
                    TotalCount = await repository.CountAsync(),
                    RowFiles = new List<string>()
                };

                if (maxRows > 0 && tableInfo.TotalCount > maxRows)
                {
                    tableInfo.TotalCount = maxRows;
                }

                await WriteUtils.PrintRowAsync(tableName, tableInfo.TotalCount.ToString("#,0"));

                var identityColumnName = await database.AddIdentityColumnIdIfNotExistsAsync(tableName, tableInfo.Columns);

                if (tableInfo.TotalCount > 0)
                {
                    var current = 1;
                    if (tableInfo.TotalCount > CliConstants.PageSize)
                    {
                        var pageCount = (int) Math.Ceiling((double) tableInfo.TotalCount / CliConstants.PageSize);

                        using (var progress = new ProgressBar())
                        {
                            for (; current <= pageCount; current++)
                            {
                                progress.Report((double) (current - 1) / pageCount);

                                var fileName = $"{current}.json";
                                tableInfo.RowFiles.Add(fileName);
                                var offset = (current - 1) * CliConstants.PageSize;
                                var limit = tableInfo.TotalCount - offset < CliConstants.PageSize
                                    ? tableInfo.TotalCount - offset
                                    : CliConstants.PageSize;

                                var rows = databaseManager.GetPageObjects(tableName, identityColumnName, offset,
                                    limit);

                                await FileUtils.WriteTextAsync(treeInfo.GetTableContentFilePath(tableName, fileName), TranslateUtils.JsonSerialize(rows));
                            }
                        }
                    }
                    else
                    {
                        var fileName = $"{current}.json";
                        tableInfo.RowFiles.Add(fileName);
                        var rows = databaseManager.GetObjects(tableName);

                        await FileUtils.WriteTextAsync(treeInfo.GetTableContentFilePath(tableName, fileName), TranslateUtils.JsonSerialize(rows));
                    }
                }

                await FileUtils.WriteTextAsync(treeInfo.GetTableMetadataFilePath(tableName),
                    TranslateUtils.JsonSerialize(tableInfo));
            }
        }
    }
}
