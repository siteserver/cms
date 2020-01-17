using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SiteServer.Abstractions;
using SiteServer.CMS.Context;
using SiteServer.CMS.Core;

namespace SiteServer.CMS.Repositories
{
    public static class RepositoryManager
    {
        public static async Task SyncDatabaseAsync()
        {
            CacheUtils.ClearAll();

            await SyncSystemTablesAsync(DataProvider.AllProviders);

            await SyncContentTablesAsync();

            await UpdateConfigVersionAsync();
        }

        private static async Task SyncSystemTablesAsync(IList<IRepository> repositories)
        {
            var database = new Database(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString);

            var configInfo = await DataProvider.ConfigRepository.GetAsync();

            if (!await database.IsTableExistsAsync(DataProvider.ConfigRepository.TableName))
            {
                await CreateTableAsync(DataProvider.ConfigRepository.TableName, DataProvider.ConfigRepository.TableColumns, string.Empty, false);
            }
            else
            {
                await AlterTableAsync(DataProvider.ConfigRepository.TableName, DataProvider.ConfigRepository.TableColumns, string.Empty);
            }

            if (configInfo.Id == 0)
            {
                await DataProvider.ConfigRepository.InsertAsync(configInfo);
            }

            foreach (var repository in repositories)
            {
                if (string.IsNullOrEmpty(repository.TableName) || repository.TableName == DataProvider.ConfigRepository.TableName || repository.TableColumns == null || repository.TableColumns.Count <= 0) continue;

                if (!await database.IsTableExistsAsync(repository.TableName))
                {
                    await CreateTableAsync(repository.TableName, repository.TableColumns, string.Empty, false);
                }
                else
                {
                    await AlterTableAsync(repository.TableName, repository.TableColumns, string.Empty);
                }
            }
        }

        //private static void SyncContentTables()
        //{
        //    // var contentDaoList = ContentRepository.GetContentDaoList();
        //    // foreach (var contentDao in contentDaoList)
        //    // {
        //    //     if (!AppContext.Db.IsTableExists(contentDao.TableName))
        //    //     {
        //    //         TableColumnManager.CreateTable(contentDao.TableName, contentDao.TableColumns, string.Empty, true, out _);
        //    //     }
        //    //     else
        //    //     {
        //    //         TableColumnManager.AlterTable(contentDao.TableName, contentDao.TableColumns, string.Empty, ContentAttribute.DropAttributes.Value);
        //    //     }
        //    // }
        //}

        private static async Task SyncContentTablesAsync()
        {
            var database = new Database(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString);
            var tableNameList = await DataProvider.SiteRepository.GetAllTableNameListAsync();

            foreach (var tableName in tableNameList)
            {
                if (!await database.IsTableExistsAsync(tableName))
                {
                    await database.CreateTableAsync(tableName, DataProvider.ContentRepository.GetTableColumns(tableName));
                }
                else
                {
                    await AlterTableAsync(tableName, DataProvider.ContentRepository.GetTableColumns(tableName), string.Empty, ContentAttribute.DropAttributes.Value);
                }
            }
        }

        private static async Task UpdateConfigVersionAsync()
        {
            var configInfo = await DataProvider.ConfigRepository.GetAsync();

            if (configInfo.Id > 0)
            {
                configInfo.DatabaseVersion = SystemManager.ProductVersion;
                configInfo.UpdateDate = DateTime.UtcNow;
                await DataProvider.ConfigRepository.UpdateAsync(configInfo);
            }
        }

        private static async Task<(bool IsSuccess, Exception Ex)> CreateTableAsync(string tableName, IList<TableColumn> tableColumns, string pluginId, bool isContentTable)
        {
            var database = new Database(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString);

            try
            {
                await database.CreateTableAsync(tableName, tableColumns);
            }
            catch (Exception ex)
            {
                await LogUtils.AddErrorLogAsync(pluginId, ex, string.Empty);
                return (false, ex);
            }

            if (isContentTable)
            {
                try
                {
                    await database.CreateIndexAsync(tableName, $"IX_{tableName}_General", $"{ContentAttribute.IsTop} DESC", $"{nameof(Content.Taxis)} DESC", $"{nameof(Content.Id)} DESC");


                    //sqlString =
                    //    $@"CREATE INDEX {DatorySql.GetQuotedIdentifier(DatabaseType, $"IX_{tableName}_General")} ON {DatorySql.GetQuotedIdentifier(DatabaseType, tableName)}({DatorySql.GetQuotedIdentifier(DatabaseType, ContentAttribute.IsTop)} DESC, {DatorySql.GetQuotedIdentifier(DatabaseType, ContentAttribute.Taxis)} DESC, {DatorySql.GetQuotedIdentifier(DatabaseType, ContentAttribute.Id)} DESC)";

                    //ExecuteNonQuery(ConnectionString, sqlString);
                }
                catch (Exception ex)
                {
                    await LogUtils.AddErrorLogAsync(pluginId, ex, string.Empty);
                    return (false, ex);
                }

                try
                {
                    await database.CreateIndexAsync(tableName, $"IX_{tableName}_Taxis", $"{ContentAttribute.Taxis} DESC");

                    //sqlString =
                    //    $@"CREATE INDEX {DatorySql.GetQuotedIdentifier(DatabaseType, $"IX_{tableName}_Taxis")} ON {DatorySql.GetQuotedIdentifier(DatabaseType, tableName)}({DatorySql.GetQuotedIdentifier(DatabaseType, ContentAttribute.Taxis)} DESC)";

                    //ExecuteNonQuery(ConnectionString, sqlString);
                }
                catch (Exception ex)
                {
                    await LogUtils.AddErrorLogAsync(pluginId, ex, string.Empty);
                    return (false, ex);
                }
            }

            return (true, null);
        }

        private static async Task AlterTableAsync(string tableName, IList<TableColumn> tableColumns, string pluginId, IList<string> dropColumnNames = null)
        {
            var database = new Database(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString);

            try
            {
                await database.AlterTableAsync(tableName,
                    GetRealTableColumns(tableColumns), dropColumnNames);
            }
            catch (Exception ex)
            {
                await LogUtils.AddErrorLogAsync(pluginId, ex, string.Empty);
            }
        }

        private static IList<TableColumn> GetRealTableColumns(IEnumerable<TableColumn> tableColumns)
        {
            var realTableColumns = new List<TableColumn>();
            foreach (var tableColumn in tableColumns)
            {
                if (string.IsNullOrEmpty(tableColumn.AttributeName) || StringUtils.EqualsIgnoreCase(tableColumn.AttributeName, nameof(Entity.Id)) || StringUtils.EqualsIgnoreCase(tableColumn.AttributeName, nameof(Entity.Guid)) || StringUtils.EqualsIgnoreCase(tableColumn.AttributeName, nameof(Entity.CreatedDate)) || StringUtils.EqualsIgnoreCase(tableColumn.AttributeName, nameof(Entity.LastModifiedDate)))
                {
                    continue;
                }

                if (tableColumn.DataType == DataType.VarChar && tableColumn.DataLength == 0)
                {
                    tableColumn.DataLength = 2000;
                }
                realTableColumns.Add(tableColumn);
            }

            realTableColumns.InsertRange(0, new List<TableColumn>
            {
                new TableColumn
                {
                    AttributeName = nameof(Entity.Id),
                    DataType = DataType.Integer,
                    IsIdentity = true,
                    IsPrimaryKey = true
                },
                new TableColumn
                {
                    AttributeName = nameof(Entity.Guid),
                    DataType = DataType.VarChar,
                    DataLength = 50
                },
                new TableColumn
                {
                    AttributeName = nameof(Entity.CreatedDate),
                    DataType = DataType.DateTime
                },
                new TableColumn
                {
                    AttributeName = nameof(Entity.LastModifiedDate),
                    DataType = DataType.DateTime
                }
            });

            return realTableColumns;
        }
    }
}
