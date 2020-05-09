using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SSCMS.Core.Utils;
using SSCMS.Models;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.Services
{
    public partial class DatabaseManager
    {
        public async Task<(bool success, string errorMessage)> InstallAsync(IOldPluginManager pluginManager, string userName, string password, string email, string mobile)
        {
            try
            {
                await SyncDatabaseAsync(pluginManager);

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

        public async Task SyncDatabaseAsync(IOldPluginManager pluginManager)
        {
            var repositories = GetAllRepositories();

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

            var config = await ConfigRepository.GetAsync();
            if (config.Id == 0)
            {
                await ConfigRepository.InsertAsync(config);
            }

            await SyncContentTablesAsync(pluginManager);

            await UpdateConfigVersionAsync();
        }

        public async Task SyncContentTablesAsync(IOldPluginManager pluginManager)
        {
            var tableNameList = await SiteRepository.GetAllTableNamesAsync(pluginManager);

            foreach (var tableName in tableNameList)
            {
                if (!await _settingsManager.Database.IsTableExistsAsync(tableName))
                {
                    await _settingsManager.Database.CreateTableAsync(tableName, ContentRepository.GetTableColumns(tableName));
                }
                else
                {
                    await AlterTableAsync(tableName, ContentRepository.GetTableColumns(tableName), string.Empty, ColumnsManager.DropAttributes.Value);
                }
            }
        }

        private async Task UpdateConfigVersionAsync()
        {
            var configInfo = await ConfigRepository.GetAsync();

            if (configInfo.Id > 0)
            {
                configInfo.DatabaseVersion = _settingsManager.Version;
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
                }
                catch (Exception ex)
                {
                    await ErrorLogRepository.AddErrorLogAsync(pluginId, ex, string.Empty);
                    return (false, ex);
                }

                try
                {
                    await _settingsManager.Database.CreateIndexAsync(tableName, $"IX_{tableName}_Taxis", $"{nameof(Content.Taxis)} DESC");
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
