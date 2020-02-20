using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.Framework;

namespace SiteServer.CMS.Repositories
{
    public partial class DatabaseRepository
    {
        public async Task InstallDatabaseAsync(string adminName, string adminPassword)
        {
            await SyncDatabaseAsync();

            if (!string.IsNullOrEmpty(adminName) && !string.IsNullOrEmpty(adminPassword))
            {
                var administrator = new Administrator
                {
                    UserName = adminName,
                };

                await _administratorRepository.InsertAsync(administrator, adminPassword);
                await _administratorRepository.AddUserToRoleAsync(adminName, PredefinedRole.ConsoleAdministrator.GetValue());
            }
        }

        public async Task CreateSiteServerTablesAsync()
        {
            var database = _configRepository.Database;
            foreach (var provider in GetAllRepositories())
            {
                if (string.IsNullOrEmpty(provider.TableName) || provider.TableColumns == null || provider.TableColumns.Count <= 0) continue;

                if (!await database.IsTableExistsAsync(provider.TableName))
                {
                    await database.CreateTableAsync(provider.TableName, provider.TableColumns);
                }
                else
                {
                    await database.AlterTableAsync(provider.TableName, provider.TableColumns);
                }
            }
        }

        public async Task SyncDatabaseAsync()
        {
            //CacheUtils.ClearAll();

            //await CreateSiteServerTablesAsync();

            //await SyncContentTablesAsync();

            //await UpdateConfigVersionAsync();

            CacheUtils.ClearAll();

            await SyncSystemTablesAsync(GetAllRepositories());

            await SyncContentTablesAsync();

            await UpdateConfigVersionAsync();
        }

        private async Task SyncSystemTablesAsync(IList<IRepository> repositories)
        {
            var database = new Database(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString);

            await _configRepository.ClearAllCache();

            var configInfo = await _configRepository.GetAsync();

            if (!await database.IsTableExistsAsync(_configRepository.TableName))
            {
                await CreateTableAsync(_configRepository.TableName, _configRepository.TableColumns, string.Empty, false);
            }
            else
            {
                await AlterTableAsync(_configRepository.TableName, _configRepository.TableColumns, string.Empty);
            }

            if (configInfo.Id == 0)
            {
                await _configRepository.InsertAsync(configInfo);
            }

            foreach (var repository in repositories)
            {
                if (string.IsNullOrEmpty(repository.TableName) || repository.TableName == _configRepository.TableName || repository.TableColumns == null || repository.TableColumns.Count <= 0) continue;

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

        public async Task SyncContentTablesAsync()
        {
            var database = new Database(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString);
            var tableNameList = await _siteRepository.GetAllTableNamesAsync();

            foreach (var tableName in tableNameList)
            {
                if (!await database.IsTableExistsAsync(tableName))
                {
                    await database.CreateTableAsync(tableName, _contentRepository.GetTableColumns(tableName));
                }
                else
                {
                    await AlterTableAsync(tableName, _contentRepository.GetTableColumns(tableName), string.Empty, ContentAttribute.DropAttributes.Value);
                }
            }
        }

        private async Task UpdateConfigVersionAsync()
        {
            var configInfo = await _configRepository.GetAsync();

            if (configInfo.Id > 0)
            {
                configInfo.DatabaseVersion = SystemManager.ProductVersion;
                configInfo.UpdateDate = DateTime.UtcNow;
                await _configRepository.UpdateAsync(configInfo);
            }
        }

        private async Task<(bool IsSuccess, Exception Ex)> CreateTableAsync(string tableName, IList<TableColumn> tableColumns, string pluginId, bool isContentTable)
        {
            var database = new Database(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString);

            try
            {
                await database.CreateTableAsync(tableName, tableColumns);
            }
            catch (Exception ex)
            {
                await DataProvider.ErrorLogRepository.AddErrorLogAsync(pluginId, ex, string.Empty);
                return (false, ex);
            }

            if (isContentTable)
            {
                try
                {
                    await database.CreateIndexAsync(tableName, $"IX_{tableName}_General", $"{nameof(Content.Top)} DESC", $"{nameof(Content.Taxis)} DESC", $"{nameof(Content.Id)} DESC");


                    //sqlString =
                    //    $@"CREATE INDEX {DatorySql.GetQuotedIdentifier(DatabaseType, $"IX_{tableName}_General")} ON {DatorySql.GetQuotedIdentifier(DatabaseType, tableName)}({DatorySql.GetQuotedIdentifier(DatabaseType, ContentAttribute.IsTop)} DESC, {DatorySql.GetQuotedIdentifier(DatabaseType, ContentAttribute.Taxis)} DESC, {DatorySql.GetQuotedIdentifier(DatabaseType, ContentAttribute.Id)} DESC)";

                    //ExecuteNonQuery(ConnectionString, sqlString);
                }
                catch (Exception ex)
                {
                    await DataProvider.ErrorLogRepository.AddErrorLogAsync(pluginId, ex, string.Empty);
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
                    await DataProvider.ErrorLogRepository.AddErrorLogAsync(pluginId, ex, string.Empty);
                    return (false, ex);
                }
            }

            return (true, null);
        }

        private async Task AlterTableAsync(string tableName, IList<TableColumn> tableColumns, string pluginId, IList<string> dropColumnNames = null)
        {
            var database = new Database(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString);

            try
            {
                await database.AlterTableAsync(tableName,
                    GetRealTableColumns(tableColumns), dropColumnNames);
            }
            catch (Exception ex)
            {
                await DataProvider.ErrorLogRepository.AddErrorLogAsync(pluginId, ex, string.Empty);
            }
        }

        private IList<TableColumn> GetRealTableColumns(IEnumerable<TableColumn> tableColumns)
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
