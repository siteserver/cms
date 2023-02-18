using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Datory;
using Newtonsoft.Json.Linq;
using SSCMS.Dto;
using SSCMS.Utils;

namespace SSCMS.Core.Services
{
    public partial class DatabaseManager
    {
        public async Task<List<string>> RestoreAsync(IConsoleUtils console, List<string> includes, List<string> excludes, string tablesFilePath, Tree tree, string errorLogFilePath)
        {
            var tableNames =
                TranslateUtils.JsonDeserialize<List<string>>(await FileUtils.ReadTextAsync(tablesFilePath, Encoding.UTF8));
            var errorTableNames = new List<string>();

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

                    var metadataFilePath = tree.GetTableMetadataFilePath(tableName);

                    if (!FileUtils.IsFileExists(metadataFilePath)) continue;

                    var table =
                        TranslateUtils.JsonDeserialize<Table>(
                            await FileUtils.ReadTextAsync(metadataFilePath, Encoding.UTF8));

                    await console.WriteRowAsync(tableName, table.TotalCount.ToString("#,0"));

                    if (await _settingsManager.Database.IsTableExistsAsync(tableName))
                    {
                        await _settingsManager.Database.DropTableAsync(tableName);
                    }

                    await _settingsManager.Database.CreateTableAsync(tableName, table.Columns);

                    if (table.Rows.Count > 0)
                    {
                        for (var i = 0; i < table.Rows.Count; i++)
                        {
                            console.Report((double)i / table.Rows.Count);

                            var fileName = table.Rows[i];

                            var objects = TranslateUtils.JsonDeserialize<List<JObject>>(
                                await FileUtils.ReadTextAsync(tree.GetTableContentFilePath(tableName, fileName), Encoding.UTF8));

                            try
                            {
                                var repository = new Repository(_settingsManager.Database, tableName,
                                    table.Columns);
                                await repository.BulkInsertAsync(objects);
                            }
                            catch (Exception exception)
                            {
                                errorTableNames.Add(tableName);
                                await FileUtils.AppendErrorLogAsync(errorLogFilePath, new TextLog
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
                    errorTableNames.Add(tableName);
                    await FileUtils.AppendErrorLogAsync(errorLogFilePath, new TextLog
                    {
                        DateTime = DateTime.Now,
                        Detail = $"插入表 {tableName}",
                        Exception = ex
                    });
                }
            }

            await console.WriteRowLineAsync();

            return errorTableNames;

            //if (!dataOnly)
            //{
            //    // 恢复后同步表，确保内容辅助表字段与系统一致
            //    await _databaseManager.SyncDatabaseAsync();
            //    //await _databaseManager.SyncContentTablesAsync();
            //    await _configRepository.UpdateConfigVersionAsync(_settingsManager.Version);
            //}
        }
    }

}
