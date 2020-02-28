using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SS.CMS.Abstractions;
using SS.CMS.Core;

namespace SS.CMS.Services
{
    public partial class DatabaseManager
    {
        public async Task<(bool success, string errorMessage)> InstallAsync(string userName, string password, string email, string mobile)
        {
            try
            {
                await SyncDatabaseAsync();

                var administrator = new Administrator
                {
                    UserName = userName,
                    Email = email,
                    Mobile = mobile
                };

                await AdministratorRepository.InsertAsync(administrator, password);
                await AdministratorRepository.AddUserToRoleAsync(userName, PredefinedRole.ConsoleAdministrator.GetValue());

                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task CreateSiteServerTablesAsync()
        {
            var database = ConfigRepository.Database;
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
            _cacheManager.Clear();

            foreach (var repository in repositories)
            {
                if (string.IsNullOrEmpty(repository.TableName) || repository.TableColumns == null || repository.TableColumns.Count <= 0) continue;

                if (!await _settingsManager.Database.IsTableExistsAsync(repository.TableName))
                {
                    await CreateTableAsync(repository.TableName, repository.TableColumns, string.Empty, false);
                }
                else
                {
                    await AlterTableAsync(repository.TableName, repository.TableColumns, string.Empty);
                }
            }

            //if (!await _settingsManager.Database.IsTableExistsAsync(ConfigRepository.TableName))
            //{
            //    await CreateTableAsync(ConfigRepository.TableName, ConfigRepository.TableColumns, string.Empty, false);
            //}
            //else
            //{
            //    await AlterTableAsync(ConfigRepository.TableName, ConfigRepository.TableColumns, string.Empty);
            //}

            var config = await ConfigRepository.GetAsync();
            if (config.Id == 0)
            {
                await ConfigRepository.InsertAsync(config);
            }
        }

        public async Task SyncContentTablesAsync()
        {
            var tableNameList = await SiteRepository.GetAllTableNamesAsync();

            foreach (var tableName in tableNameList)
            {
                if (!await _settingsManager.Database.IsTableExistsAsync(tableName))
                {
                    await _settingsManager.Database.CreateTableAsync(tableName, ContentRepository.GetTableColumns(tableName));
                }
                else
                {
                    await AlterTableAsync(tableName, ContentRepository.GetTableColumns(tableName), string.Empty, ContentAttribute.DropAttributes.Value);
                }
            }
        }

        private async Task UpdateConfigVersionAsync()
        {
            var configInfo = await ConfigRepository.GetAsync();

            if (configInfo.Id > 0)
            {
                configInfo.DatabaseVersion = _settingsManager.ProductVersion;
                configInfo.UpdateDate = DateTime.UtcNow;
                await ConfigRepository.UpdateAsync(configInfo);
            }
        }

        private async Task<(bool IsSuccess, Exception Ex)> CreateTableAsync(string tableName, IList<TableColumn> tableColumns, string pluginId, bool isContentTable)
        {
            try
            {
                await _settingsManager.Database.CreateTableAsync(tableName, tableColumns);
            }
            catch (Exception ex)
            {
                await ErrorLogRepository.AddErrorLogAsync(pluginId, ex, string.Empty);
                return (false, ex);
            }

            if (isContentTable)
            {
                try
                {
                    await _settingsManager.Database.CreateIndexAsync(tableName, $"IX_{tableName}_General", $"{nameof(Content.Top)} DESC", $"{nameof(Content.Taxis)} DESC", $"{nameof(Content.Id)} DESC");


                    //sqlString =
                    //    $@"CREATE INDEX {DatorySql.GetQuotedIdentifier(DatabaseType, $"IX_{tableName}_General")} ON {DatorySql.GetQuotedIdentifier(DatabaseType, tableName)}({DatorySql.GetQuotedIdentifier(DatabaseType, ContentAttribute.IsTop)} DESC, {DatorySql.GetQuotedIdentifier(DatabaseType, ContentAttribute.Taxis)} DESC, {DatorySql.GetQuotedIdentifier(DatabaseType, ContentAttribute.Id)} DESC)";

                    //ExecuteNonQuery(ConnectionString, sqlString);
                }
                catch (Exception ex)
                {
                    await ErrorLogRepository.AddErrorLogAsync(pluginId, ex, string.Empty);
                    return (false, ex);
                }

                try
                {
                    await _settingsManager.Database.CreateIndexAsync(tableName, $"IX_{tableName}_Taxis", $"{ContentAttribute.Taxis} DESC");

                    //sqlString =
                    //    $@"CREATE INDEX {DatorySql.GetQuotedIdentifier(DatabaseType, $"IX_{tableName}_Taxis")} ON {DatorySql.GetQuotedIdentifier(DatabaseType, tableName)}({DatorySql.GetQuotedIdentifier(DatabaseType, ContentAttribute.Taxis)} DESC)";

                    //ExecuteNonQuery(ConnectionString, sqlString);
                }
                catch (Exception ex)
                {
                    await ErrorLogRepository.AddErrorLogAsync(pluginId, ex, string.Empty);
                    return (false, ex);
                }
            }

            return (true, null);
        }

        private async Task AlterTableAsync(string tableName, IList<TableColumn> tableColumns, string pluginId, IList<string> dropColumnNames = null)
        {
            try
            {
                await _settingsManager.Database.AlterTableAsync(tableName,
                    GetRealTableColumns(tableColumns), dropColumnNames);
            }
            catch (Exception ex)
            {
                await ErrorLogRepository.AddErrorLogAsync(pluginId, ex, string.Empty);
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
