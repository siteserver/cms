using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SSCMS.Dto;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.Services
{
    public partial class DatabaseManager
    {
        public async Task<List<string>> BackupAsync(IConsoleUtils console, List<string> includes, List<string> excludes, int maxRows, int pageSize, Tree tree, string errorLogFilePath)
        {
            if (excludes == null)
            {
                excludes = new List<string>();
            }
            excludes.Add("bairong_Log");
            excludes.Add("bairong_ErrorLog");
            excludes.Add("siteserver_ErrorLog");
            excludes.Add("siteserver_Log");
            excludes.Add("siteserver_SiteLog");
            excludes.Add("siteserver_Tracking");

            var allTableNames = await _settingsManager.Database.GetTableNamesAsync();

            var tableNames = new List<string>();
            var errorTableNames = new List<string>();

            foreach (var tableName in allTableNames)
            {
                if (includes != null && !ListUtils.ContainsIgnoreCase(includes, tableName)) continue;
                if (ListUtils.ContainsIgnoreCase(excludes, tableName)) continue;
                if (ListUtils.ContainsIgnoreCase(tableNames, tableName)) continue;
                tableNames.Add(tableName);
            }

            await FileUtils.WriteTextAsync(tree.TablesFilePath, TranslateUtils.JsonSerialize(tableNames));

            await console.WriteRowLineAsync();
            await console.WriteRowAsync("Backup table name", "Count");
            await console.WriteRowLineAsync();

            foreach (var tableName in tableNames)
            {
                try
                {
                    var columns = await _settingsManager.Database.GetTableColumnsAsync(tableName);
                    var repository = new Repository(_settingsManager.Database, tableName, columns);

                    var table = new Table
                    {
                        Columns = repository.TableColumns,
                        TotalCount = await repository.CountAsync(),
                        Rows = new List<string>()
                    };

                    if (maxRows > 0 && table.TotalCount > maxRows)
                    {
                        table.TotalCount = maxRows;
                    }

                    await console.WriteRowAsync(tableName, table.TotalCount.ToString("#,0"));

                    var identityColumnName =
                        await _settingsManager.Database.AddIdentityColumnIdIfNotExistsAsync(tableName, table.Columns);

                    if (table.TotalCount > 0)
                    {
                        var current = 1;
                        if (table.TotalCount > pageSize)
                        {
                            var pageCount = (int)Math.Ceiling((double)table.TotalCount / pageSize);
                            for (; current <= pageCount; current++)
                            {
                                console.Report((double)(current - 1) / pageCount);

                                var fileName = $"{current}.json";
                                table.Rows.Add(fileName);
                                var offset = (current - 1) * pageSize;
                                var limit = table.TotalCount - offset < pageSize
                                    ? table.TotalCount - offset
                                    : pageSize;

                                var rows = await GetPageObjectsAsync(tableName, identityColumnName, offset, limit);

                                await FileUtils.WriteTextAsync(
                                    tree.GetTableContentFilePath(tableName, fileName),
                                    TranslateUtils.JsonSerialize(rows));
                            }
                        }
                        else
                        {
                            var fileName = $"{current}.json";
                            table.Rows.Add(fileName);
                            var rows = await GetObjectsAsync(tableName);

                            await FileUtils.WriteTextAsync(tree.GetTableContentFilePath(tableName, fileName),
                                TranslateUtils.JsonSerialize(rows));
                        }
                    }

                    await FileUtils.WriteTextAsync(tree.GetTableMetadataFilePath(tableName),
                        TranslateUtils.JsonSerialize(table));
                }
                catch (Exception ex)
                {
                    errorTableNames.Add(tableName);
                    await FileUtils.AppendErrorLogAsync(errorLogFilePath, new TextLog
                    {
                        Exception = ex,
                        DateTime = DateTime.Now,
                        Detail = tableName
                    });
                }
            }

            return errorTableNames;
        }

    }

}
