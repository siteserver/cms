using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using SS.CMS.Core.Models.Attributes;
using SS.CMS.Data;
using SS.CMS.Models;
using SS.CMS.Repositories;
using SS.CMS.Services;
using SS.CMS.Utils;

namespace SS.CMS.Core.Repositories
{
    public partial class DatabaseRepository
    {
        public async Task<(bool IsSuccess, string ErrorMessage)> InstallDatabaseAsync(string adminName, string adminPassword, IList<IRepository> repositories)
        {
            // SyncDatabase();

            // var userInfo = new UserInfo
            // {
            //     UserName = userName,
            //     Password = password
            // };

            // _userRepository.Insert(userInfo, out _);
            // _userRoleRepository.AddUserToRole(userName, AuthTypes.Roles.SuperAdministrator);

            await SyncDatabaseAsync(repositories);

            var configInfo = new Config
            {
                DatabaseVersion = _settingsManager.ProductVersion,
                UpdateDate = DateTime.UtcNow,
                ExtendValues = string.Empty
            };
            await _configRepository.DeleteAllAsync();
            await _configRepository.InsertAsync(configInfo);

            var userInfo = new User
            {
                UserName = adminName,
                Password = adminPassword,
                RoleName = AuthTypes.Roles.SuperAdministrator
            };

            var (isSuccess, userId, errorMessage) = await _userRepository.InsertAsync(userInfo);

            return (isSuccess, errorMessage);
        }

        public async Task<(bool IsSuccess, Exception Ex)> CreateTableAsync(string tableName, IList<TableColumn> tableColumns, string pluginId, bool isContentTable)
        {
            try
            {
                await _database.CreateTableAsync(tableName, tableColumns);
            }
            catch (Exception ex)
            {
                await _errorLogRepository.AddErrorLogAsync(pluginId, ex, string.Empty);
                return (false, ex);
            }

            if (isContentTable)
            {
                try
                {
                    await _database.CreateIndexAsync(tableName, $"IX_{tableName}_General", $"{ContentAttribute.IsTop} DESC", $"{ContentAttribute.Taxis} DESC", $"{ContentAttribute.Id} DESC");


                    //sqlString =
                    //    $@"CREATE INDEX {DatorySql.GetQuotedIdentifier(DatabaseType, $"IX_{tableName}_General")} ON {DatorySql.GetQuotedIdentifier(DatabaseType, tableName)}({DatorySql.GetQuotedIdentifier(DatabaseType, ContentAttribute.IsTop)} DESC, {DatorySql.GetQuotedIdentifier(DatabaseType, ContentAttribute.Taxis)} DESC, {DatorySql.GetQuotedIdentifier(DatabaseType, ContentAttribute.Id)} DESC)";

                    //ExecuteNonQuery(ConnectionString, sqlString);
                }
                catch (Exception ex)
                {
                    await _errorLogRepository.AddErrorLogAsync(pluginId, ex, string.Empty);
                    return (false, ex);
                }

                try
                {
                    await _database.CreateIndexAsync(tableName, $"IX_{tableName}_Taxis", $"{ContentAttribute.Taxis} DESC");

                    //sqlString =
                    //    $@"CREATE INDEX {DatorySql.GetQuotedIdentifier(DatabaseType, $"IX_{tableName}_Taxis")} ON {DatorySql.GetQuotedIdentifier(DatabaseType, tableName)}({DatorySql.GetQuotedIdentifier(DatabaseType, ContentAttribute.Taxis)} DESC)";

                    //ExecuteNonQuery(ConnectionString, sqlString);
                }
                catch (Exception ex)
                {
                    await _errorLogRepository.AddErrorLogAsync(pluginId, ex, string.Empty);
                    return (false, ex);
                }
            }

            await _cache.RemoveAsync(_cacheKey);
            return (true, null);
        }

        public async Task AlterTableAsync(string tableName, IList<TableColumn> tableColumns, string pluginId, IList<string> dropColumnNames = null)
        {
            try
            {
                await _database.AlterTableAsync(tableName,
                    GetRealTableColumns(tableColumns), dropColumnNames);

                await _cache.RemoveAsync(_cacheKey);
            }
            catch (Exception ex)
            {
                await _errorLogRepository.AddErrorLogAsync(pluginId, ex, string.Empty);
            }
        }

        public async Task AlterSystemTableAsync(string tableName, IList<TableColumn> tableColumns, IList<string> dropColumnNames = null)
        {
            await _database.AlterTableAsync(tableName, tableColumns, dropColumnNames);

            await _cache.RemoveAsync(_cacheKey);
        }

        public async Task SyncSystemTablesAsync(IList<IRepository> repositories)
        {
            var configInfo = await _configRepository.GetConfigInfoAsync();

            if (!await _database.IsTableExistsAsync(_configRepository.TableName))
            {
                await CreateTableAsync(_configRepository.TableName, _configRepository.TableColumns, string.Empty, false);
            }
            else
            {
                await AlterTableAsync(_configRepository.TableName, _configRepository.TableColumns, string.Empty);
            }

            if (configInfo == null)
            {
                configInfo = new Config();
                await _configRepository.InsertAsync(configInfo);
            }

            foreach (var repository in repositories)
            {
                if (string.IsNullOrEmpty(repository.TableName) || repository.TableName == _configRepository.TableName || repository.TableColumns == null || repository.TableColumns.Count <= 0) continue;

                if (!await _database.IsTableExistsAsync(repository.TableName))
                {
                    await CreateTableAsync(repository.TableName, repository.TableColumns, string.Empty, false);
                }
                else
                {
                    await AlterTableAsync(repository.TableName, repository.TableColumns, string.Empty);
                }
            }
        }

        public async Task UpdateConfigVersionAsync()
        {
            var configInfo = await _configRepository.GetConfigInfoAsync();

            if (configInfo != null)
            {
                configInfo.DatabaseVersion = _settingsManager.ProductVersion;
                configInfo.UpdateDate = DateTime.UtcNow;
                await _configRepository.UpdateAsync(configInfo);
            }
        }

        public async Task SyncDatabaseAsync(IList<IRepository> repositories)
        {
            await SyncSystemTablesAsync(repositories);

            SyncContentTables();

            await UpdateConfigVersionAsync();
        }

        public async Task CreateContentTableAsync(string tableName, IList<TableColumn> tableColumns)
        {
            var isDbExists = await _database.IsTableExistsAsync(tableName);
            if (isDbExists) return;

            await _database.CreateTableAsync(tableName, tableColumns);
            await _database.CreateIndexAsync(tableName, $"IX_{tableName}", $"{ContentAttribute.IsTop} DESC", $"{ContentAttribute.Taxis} DESC", $"{ContentAttribute.Id} DESC");
            await _database.CreateIndexAsync(tableName, $"IX_{tableName}_Taxis", ContentAttribute.Taxis);

            await _cache.RemoveAsync(_cacheKey);
        }

        public void SyncContentTables()
        {
            // var contentDaoList = ContentRepository.GetContentDaoList();
            // foreach (var contentDao in contentDaoList)
            // {
            //     if (!AppContext.Db.IsTableExists(contentDao.TableName))
            //     {
            //         TableColumnManager.CreateTable(contentDao.TableName, contentDao.TableColumns, string.Empty, true, out _);
            //     }
            //     else
            //     {
            //         TableColumnManager.AlterTable(contentDao.TableName, contentDao.TableColumns, string.Empty, ContentAttribute.DropAttributes.Value);
            //     }
            // }
        }
    }
}
