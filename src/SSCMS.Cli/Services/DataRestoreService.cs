using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Datory;
using Newtonsoft.Json.Linq;
using SSCMS.Cli.Abstractions;
using SSCMS.Cli.Core;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Cli.Services
{
    public class DataRestoreService : IDataRestoreService
    {
        private readonly ISettingsManager _settingsManager;
        private readonly IConfigRepository _configRepository;
        private readonly IDatabaseManager _databaseManager;
        private readonly IOldPluginManager _pluginManager;

        public DataRestoreService(ISettingsManager settingsManager, IConfigRepository configRepository, IDatabaseManager databaseManager, IOldPluginManager pluginManager)
        {
            _settingsManager = settingsManager;
            _configRepository = configRepository;
            _databaseManager = databaseManager;
            _pluginManager = pluginManager;
        }

        public async Task RestoreAsync(List<string> includes, List<string> excludes, bool dataOnly, string tablesFilePath, TreeInfo treeInfo, string errorLogFilePath)
        {
            var tableNames =
                TranslateUtils.JsonDeserialize<List<string>>(await FileUtils.ReadTextAsync(tablesFilePath, Encoding.UTF8));

            foreach (var tableName in tableNames)
            {
                try
                {
                    if (includes != null)
                    {
                        if (!ListUtils.ContainsIgnoreCase(includes, tableName)) continue;
                    }

                    if (excludes != null)
                    {
                        if (ListUtils.ContainsIgnoreCase(excludes, tableName)) continue;
                    }

                    var metadataFilePath = treeInfo.GetTableMetadataFilePath(tableName);

                    if (!FileUtils.IsFileExists(metadataFilePath)) continue;

                    var tableInfo =
                        TranslateUtils.JsonDeserialize<TableInfo>(
                            await FileUtils.ReadTextAsync(metadataFilePath, Encoding.UTF8));

                    await WriteUtils.PrintRowAsync(tableName, tableInfo.TotalCount.ToString("#,0"));

                    if (await _settingsManager.Database.IsTableExistsAsync(tableName))
                    {
                        await _settingsManager.Database.DropTableAsync(tableName);
                    }

                    await _settingsManager.Database.CreateTableAsync(tableName, tableInfo.Columns);

                    if (tableInfo.RowFiles.Count > 0)
                    {
                        using var progress = new ProgressBar();
                        for (var i = 0; i < tableInfo.RowFiles.Count; i++)
                        {
                            progress.Report((double)i / tableInfo.RowFiles.Count);

                            var fileName = tableInfo.RowFiles[i];

                            var objects = TranslateUtils.JsonDeserialize<List<JObject>>(
                                await FileUtils.ReadTextAsync(treeInfo.GetTableContentFilePath(tableName, fileName), Encoding.UTF8));

                            try
                            {
                                var repository = new Repository(_settingsManager.Database, tableName,
                                    tableInfo.Columns);
                                await repository.BulkInsertAsync(objects);
                            }
                            catch (Exception exception)
                            {
                                await CliUtils.AppendErrorLogAsync(errorLogFilePath, new TextLogInfo
                                {
                                    DateTime = DateTime.Now,
                                    Detail = $"插入表 {tableName}, 文件名 {fileName}",
                                    Exception = exception
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    await CliUtils.AppendErrorLogAsync(errorLogFilePath, new TextLogInfo
                    {
                        DateTime = DateTime.Now,
                        Detail = $"插入表 {tableName}",
                        Exception = ex
                    });
                }
            }

            await WriteUtils.PrintRowLineAsync();

            if (!dataOnly)
            {
                // 恢复后同步表，确保内容辅助表字段与系统一致
                await _databaseManager.SyncContentTablesAsync(_pluginManager);
                await _configRepository.UpdateConfigVersionAsync(_settingsManager.Version);
            }
        }
    }
}
