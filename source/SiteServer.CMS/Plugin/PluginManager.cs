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
using SiteServer.Plugin;
using SiteServer.Plugin.Features;

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
            if (!fullPath.EndsWith(PluginUtils.PluginConfigName) && !fullPath.EndsWith(".dll")) return;

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
            pluginPair.Plugin.Deactive(pluginPair.Context);
            PluginCache.Remove(pluginPair.Metadata.Id);
        }

        public static bool ActiveAndAdd(PluginMetadata metadata, IPlugin plugin)
        {
            if (metadata == null || plugin == null) return false;
            
            var s = Stopwatch.StartNew();

            var context = new PluginContext(Environment, metadata, new PublicApiInstance(metadata));
            plugin.Active(context);

            var contentTable = plugin as IContentModel;
            if (!string.IsNullOrEmpty(contentTable?.CustomContentTableName) && contentTable.CustomContentTableColumns != null)
            {
                var tableName = contentTable.CustomContentTableName;
                if (!BaiRongDataProvider.DatabaseDao.IsTableExists(tableName))
                {
                    var tableInfo = new AuxiliaryTableInfo(tableName, $"插件内容表：{metadata.DisplayName}", 0,
                        EAuxiliaryTableType.Custom, false, false, false, string.Empty);
                    BaiRongDataProvider.TableCollectionDao.Insert(tableInfo);

                    foreach (var tableColumn in contentTable.CustomContentTableColumns)
                    {
                        if (string.IsNullOrEmpty(tableColumn.AttributeName)) continue;

                        var tableMetadataInfo = new TableMetadataInfo(0, tableName, tableColumn.AttributeName,
                            tableColumn.DataType, tableColumn.DataLength, 0, true);
                        BaiRongDataProvider.TableMetadataDao.Insert(tableMetadataInfo);

                        var tableStyleInfo = TableStyleManager.GetTableStyleInfo(ETableStyle.BackgroundContent, tableName,
                            tableColumn.AttributeName, new List<int> { 0 });
                        tableStyleInfo.DisplayName = tableColumn.DisplayName;
                        tableStyleInfo.InputType = InputTypeUtils.GetValue(tableColumn.InputType);
                        tableStyleInfo.DefaultValue = tableColumn.DefaultValue;
                        tableStyleInfo.Additional.IsValidate = true;
                        tableStyleInfo.Additional.IsRequired = tableColumn.IsRequired;
                        tableStyleInfo.Additional.MinNum = tableColumn.MinNum;
                        tableStyleInfo.Additional.MaxNum = tableColumn.MaxNum;
                        tableStyleInfo.Additional.RegExp = tableColumn.RegExp;
                        tableStyleInfo.Additional.Width = tableColumn.Width;
                        TableStyleManager.Insert(tableStyleInfo, ETableStyle.BackgroundContent);
                    }

                    BaiRongDataProvider.TableMetadataDao.CreateAuxiliaryTable(tableName);
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
                                string sqlString = $"ALTER TABLE {tableName} ADD {columnSqlString}";

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

        public static bool Install(string pluginId, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (string.IsNullOrEmpty(pluginId)) return false;

            try
            {
                var zipFilePath = PathUtility.GetSiteTemplatesPath(pluginId + ".zip");
                FileUtils.DeleteFileIfExists(zipFilePath);

                var downloadUrl = PluginUtils.GetDownloadUrl(pluginId);
                WebClientUtils.SaveRemoteFileToLocal(downloadUrl, zipFilePath);

                var directoryPath = PathUtils.GetPluginsPath(pluginId);
                ZipUtils.UnpackFiles(zipFilePath, directoryPath);

                var jsonPath = PathUtils.Combine(directoryPath, PluginUtils.PluginConfigName);
                if (!FileUtils.IsFileExists(jsonPath))
                {
                    errorMessage = $"插件配置文件{PluginUtils.PluginConfigName}不存在";
                    return false;
                }

                var plugin = PluginUtils.GetMetadataFromJson(directoryPath);
                if (plugin == null)
                {
                    errorMessage = "插件配置文件不正确";
                    return false;
                }

                if (PluginCache.IsExists(plugin.Id))
                {
                    errorMessage = "插件已存在";
                    return false;
                    //errorMessage = $"Do you want to update following plugin?{Environment.NewLine}{Environment.NewLine}" +
                    //          $"Name: {plugin.Name}{Environment.NewLine}" +
                    //          $"Old Version: {existingPlugin.Metadata.Version}" +
                    //          $"{Environment.NewLine}New Version: {plugin.Version}" +
                    //          $"{Environment.NewLine}Author: {plugin.Author}";
                }

                ZipUtils.UnpackFiles(zipFilePath, PathUtils.GetPluginsPath(pluginId));

                FileUtils.DeleteFileIfExists(zipFilePath);
                DirectoryUtils.DeleteDirectoryIfExists(directoryPath);
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

        public static PluginMetadata Enable(string pluginId)
        {
            var metadata = PluginCache.GetMetadata(pluginId);
            if (metadata != null)
            {
                metadata.Disabled = false;
                PluginCache.SetMetadata(metadata);
                PluginUtils.SaveMetadataToJson(metadata);
            }
            Thread.Sleep(1200);
            return metadata;
        }

        public static PluginMetadata Disable(string pluginId)
        {
            var metadata = PluginCache.GetMetadata(pluginId);
            if (metadata != null)
            {
                metadata.Disabled = true;
                PluginCache.SetMetadata(metadata);
                PluginUtils.SaveMetadataToJson(metadata);
            }
            Thread.Sleep(1200);
            return metadata;
        }
    }
}
