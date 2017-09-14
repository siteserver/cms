using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using BaiRong.Core;
using BaiRong.Core.AuxiliaryTable;
using BaiRong.Core.Data;
using BaiRong.Core.IO;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;
using BaiRong.Core.Net;
using SiteServer.CMS.Core;
using SiteServer.CMS.Plugin.Apis;
using SiteServer.Plugin;
using SiteServer.Plugin.Features;
using SiteServer.Plugin.Models;

namespace SiteServer.CMS.Plugin
{
    /// <summary>
    /// The entry for managing SiteServer plugins
    /// </summary>
    public static class PluginManager
    {
        public static PluginEnvironment Environment { get; private set; }

        private static FileSystemWatcher _watcher;

        public static void Load(PluginEnvironment environment)
        {
            Environment = environment;

            try
            {
                var pluginsPath = PathUtils.GetPluginsPath();
                if (!Directory.Exists(pluginsPath))
                {
                    Directory.CreateDirectory(pluginsPath);
                }

                Parallel.ForEach(DirectoryUtils.GetDirectoryPaths(pluginsPath), PluginUtils.ActivePlugin);

                _watcher = new FileSystemWatcher
                {
                    Path = pluginsPath,
                    NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName,
                    IncludeSubdirectories = true
                };
                _watcher.Created += Watcher_EventHandler;
                _watcher.Changed += Watcher_EventHandler;
                _watcher.Deleted += Watcher_EventHandlerDelete;
                _watcher.Renamed += Watcher_EventHandler;
                _watcher.EnableRaisingEvents = true;
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex, "载入插件时报错");
            }
        }

        private static void Watcher_EventHandler(object sender, FileSystemEventArgs e)
        {
            var fullPath = e.FullPath.ToLower();
            if (!fullPath.Contains("-") || !fullPath.EndsWith(PluginUtils.PluginConfigName) && !fullPath.EndsWith(".dll")) return;

            try
            {
                _watcher.EnableRaisingEvents = false;
                PluginUtils.OnConfigOrDllChanged(sender, e);
            }
            finally
            {
                _watcher.EnableRaisingEvents = true;
            }
        }

        private static void Watcher_EventHandlerDelete(object sender, FileSystemEventArgs e)
        {
            if (!PathUtils.IsDirectoryPath(e.FullPath)) return;

            try
            {
                _watcher.EnableRaisingEvents = false;
                PluginUtils.OnDirectoryDeleted(sender, e);
            }
            finally
            {
                _watcher.EnableRaisingEvents = true;
            }
        }

        public static void DeactiveAndRemove(PluginPair pluginPair)
        {
            pluginPair.Plugin.OnPluginDeactive?.Invoke(pluginPair.Context);
            PluginCache.Remove(pluginPair.Metadata.Id);
        }

        public static bool ActiveAndAdd(PluginMetadata metadata, IPlugin plugin)
        {
            if (metadata == null || plugin == null) return false;
            
            var s = Stopwatch.StartNew();

            var context = new PluginContext
            {
                Environment = Environment,
                Metadata = metadata,
                AuthApi = new AuthApi(metadata),
                ConfigApi = new ConfigApi(metadata),
                ContentApi = ContentApi.Instance,
                DataApi = new DataApi(metadata),
                FilesApi = FilesApi.Instance,
                NodeApi = NodeApi.Instance,
                PageApi = new PageApi(metadata),
                ParseApi = ParseApi.Instance,
                PaymentApi = PaymentApi.Instance,
                PublishmentSystemApi = PublishmentSystemApi.Instance,
                SmsApi = SmsApi.Instance
            };
            plugin.OnPluginActive?.Invoke(context);

            var contentTable = plugin as IContentTable;
            if (!string.IsNullOrEmpty(contentTable?.ContentTableName) && contentTable.ContentTableColumns != null && contentTable.ContentTableColumns.Count > 0)
            {
                var tableName = contentTable.ContentTableName;

                if (!BaiRongDataProvider.DatabaseDao.IsTableExists(tableName))
                {
                    BaiRongDataProvider.TableCollectionDao.Delete(tableName);
                    BaiRongDataProvider.TableMetadataDao.Delete(tableName);
                    BaiRongDataProvider.TableStyleDao.Delete(tableName);

                    BaiRongDataProvider.TableCollectionDao.Insert(new AuxiliaryTableInfo(tableName,
                        $"插件内容表：{metadata.DisplayName}", 0,
                        EAuxiliaryTableType.Custom, false, false, false, string.Empty));

                    var tableMetadataInfoList = new List<TableMetadataInfo>();
                    foreach (var tableColumn in contentTable.ContentTableColumns)
                    {
                        if (string.IsNullOrEmpty(tableColumn.AttributeName) || ContentAttribute.AllAttributes.Contains(tableColumn.AttributeName.ToLower())) continue;

                        tableMetadataInfoList.Add(new TableMetadataInfo(0, tableName, tableColumn.AttributeName,
                            tableColumn.DataType, tableColumn.DataLength, 0, true));
                    }

                    tableMetadataInfoList.Reverse();
                    foreach (var tableMetadataInfo in tableMetadataInfoList)
                    {
                        BaiRongDataProvider.TableMetadataDao.Insert(tableMetadataInfo);
                    }

                    BaiRongDataProvider.TableMetadataDao.CreateAuxiliaryTable(tableName);
                }
                else
                {
                    var columnNameList = BaiRongDataProvider.TableStructureDao.GetColumnNameList(tableName, true);
                    foreach (var tableColumn in contentTable.ContentTableColumns)
                    {
                        if (columnNameList.Contains(tableColumn.AttributeName.ToLower())) continue;

                        var tableMetadataInfo = new TableMetadataInfo(0, tableName, tableColumn.AttributeName,
                            tableColumn.DataType, tableColumn.DataLength, 0, true);
                        BaiRongDataProvider.TableMetadataDao.Insert(tableMetadataInfo);

                        var columnSqlString = SqlUtils.GetColumnSqlString(tableColumn.DataType, tableColumn.AttributeName, tableColumn.DataLength);
                        string sqlString = $"ALTER TABLE {tableName} ADD ({columnSqlString})";

                        BaiRongDataProvider.DatabaseDao.ExecuteSql(sqlString);
                    }

                    BaiRongDataProvider.TableCollectionDao.UpdateIsChangedAfterCreatedInDb(false, tableName);
                }

                var tableStyleInfoList = new List<TableStyleInfo>();
                foreach (var tableColumn in contentTable.ContentTableColumns)
                {
                    var tableStyleInfo = TableStyleManager.GetTableStyleInfo(ETableStyle.Custom, tableName,
                            tableColumn.AttributeName, new List<int> { 0 });
                    tableStyleInfo.DisplayName = tableColumn.DisplayName;
                    tableStyleInfo.InputType = InputTypeUtils.GetValue(tableColumn.InputType);
                    tableStyleInfo.DefaultValue = tableColumn.DefaultValue;
                    tableStyleInfo.IsVisible = tableColumn.IsVisibleInEdit;
                    tableStyleInfo.IsVisibleInList = tableColumn.IsVisibleInList;
                    tableStyleInfo.Additional.IsValidate = true;
                    tableStyleInfo.Additional.IsRequired = tableColumn.IsRequired;
                    tableStyleInfo.Additional.ValidateType = tableColumn.ValidateType;
                    tableStyleInfo.Additional.MinNum = tableColumn.MinNum;
                    tableStyleInfo.Additional.MaxNum = tableColumn.MaxNum;
                    tableStyleInfo.Additional.RegExp = tableColumn.RegExp;
                    tableStyleInfo.Additional.Width = tableColumn.Width;
                    tableStyleInfoList.Add(tableStyleInfo);
                }

                tableStyleInfoList.Reverse();
                foreach (var tableStyleInfo in tableStyleInfoList)
                {
                    TableStyleManager.InsertOrUpdate(tableStyleInfo, ETableStyle.Custom);
                }
            }

            var table = plugin as ITable;
            if (table?.Tables != null)
            {
                foreach (var tableName in table.Tables.Keys)
                {
                    var tableColumns = table.Tables[tableName];
                    if (tableColumns == null || tableColumns.Count <= 0) continue;

                    if (!BaiRongDataProvider.DatabaseDao.IsTableExists(tableName))
                    {
                        BaiRongDataProvider.DatabaseDao.CreatePluginTable(tableName, tableColumns);
                    }
                    else
                    {
                        var columnNameList = BaiRongDataProvider.TableStructureDao.GetColumnNameList(tableName, true);
                        foreach (var tableColumn in tableColumns)
                        {
                            if (!columnNameList.Contains(tableColumn.AttributeName.ToLower()))
                            {
                                var columnSqlString = SqlUtils.GetColumnSqlString(tableColumn.DataType, tableColumn.AttributeName, tableColumn.DataLength);
                                string sqlString = $"ALTER TABLE {tableName} ADD ({columnSqlString})";

                                BaiRongDataProvider.DatabaseDao.ExecuteSql(sqlString);
                            }
                        }
                    }
                }
            }

            var milliseconds = s.ElapsedMilliseconds;

            metadata.InitTime = milliseconds;

            var pair = new PluginPair(context, plugin);

            PluginCache.Set(metadata.Id, pair);
            return true;
        }

        public static bool Install(string pluginId, string version, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (string.IsNullOrEmpty(pluginId)) return false;

            try
            {
                if (PluginCache.IsExists(pluginId))
                {
                    errorMessage = "插件已存在";
                    return false;
                }
                var directoryPath = PathUtils.GetPluginsPath(pluginId);
                DirectoryUtils.DeleteDirectoryIfExists(directoryPath);

                var zipFilePath = PathUtility.GetTemporaryFilesPath(pluginId + ".zip");
                FileUtils.DeleteFileIfExists(zipFilePath);

                var downloadUrl = PluginUtils.GetDownloadUrl(pluginId, version);
                WebClientUtils.SaveRemoteFileToLocal(downloadUrl, zipFilePath);
                
                ZipUtils.UnpackFiles(zipFilePath, directoryPath);
                FileUtils.DeleteFileIfExists(zipFilePath);

                var jsonPath = PathUtils.Combine(directoryPath, PluginUtils.PluginConfigName);
                if (!FileUtils.IsFileExists(jsonPath))
                {
                    errorMessage = $"插件配置文件{PluginUtils.PluginConfigName}不存在";
                    return false;
                }

                var metadata = PluginUtils.GetMetadataFromJson(directoryPath);
                if (metadata == null)
                {
                    errorMessage = "插件配置文件不正确";
                    return false;
                }

                metadata.Disabled = false;
                metadata.DatabaseType = string.Empty;
                metadata.ConnectionString = string.Empty;

                Update(metadata);
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
                return false;
            }

            return true;
        }

        public static PluginMetadata Delete(string pluginId)
        {
            var metadata = PluginCache.GetMetadata(pluginId);
            if (metadata != null)
            {
                if (DirectoryUtils.DeleteDirectoryIfExists(metadata.DirectoryPath))
                {
                    PluginCache.Remove(pluginId);
                }
            }
            Thread.Sleep(1200);
            return metadata;
        }

        public static PluginMetadata UpdateDisabled(string pluginId, bool isDisabled)
        {
            var metadata = PluginCache.GetMetadata(pluginId);
            if (metadata != null)
            {
                metadata.Disabled = isDisabled;
                Update(metadata);
            }
            return metadata;
        }

        public static PluginMetadata UpdateDatabase(string pluginId, string databaseType, string connectionString)
        {
            var metadata = PluginCache.GetMetadata(pluginId);
            if (metadata != null)
            {
                if (WebConfigUtils.IsProtectData && !string.IsNullOrEmpty(databaseType))
                {
                    databaseType = TranslateUtils.EncryptStringBySecretKey(databaseType);
                }
                if (WebConfigUtils.IsProtectData && !string.IsNullOrEmpty(connectionString))
                {
                    connectionString = TranslateUtils.EncryptStringBySecretKey(connectionString);
                }
                metadata.DatabaseType = databaseType;
                metadata.ConnectionString = connectionString;
                Update(metadata);
            }
            return metadata;
        }

        private static void Update(PluginMetadata metadata)
        {
            PluginCache.SetMetadata(metadata);
            PluginUtils.SaveMetadataToJson(metadata);
            Thread.Sleep(1200);
        }
    }
}
