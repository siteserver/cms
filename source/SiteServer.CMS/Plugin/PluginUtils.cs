using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using Newtonsoft.Json;
using SiteServer.CMS.Controllers.Plugins;
using SiteServer.Plugin;
using SiteServer.Plugin.Features;
using SiteServer.Plugin.Models;

namespace SiteServer.CMS.Plugin
{
    internal static class PluginUtils
    {
        internal const string PluginConfigName = "plugin.config";

        //static PluginUtils()
        //{
        //    AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        //}

        //private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        //{
        //    return AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(anAssembly => anAssembly.FullName == args.Name);
        //}

        internal static string GetDownloadUrl(string pluginId, string version)
        {
            return $@"http://plugins.siteserver.cn/download/{pluginId}.zip?version={version}";
        }

        /// <summary>
        /// Parse plugin metadata in giving directories
        /// </summary>
        /// <returns></returns>
        internal static PluginMetadata GetMetadataFromJson(string directoryPath)
        {
            var configPath = Path.Combine(directoryPath, PluginConfigName);
            if (!File.Exists(configPath))
            {
                return null;
            }

            PluginMetadata metadata;
            try
            {
                metadata = JsonConvert.DeserializeObject<PluginMetadata>(File.ReadAllText(configPath));
                metadata.DirectoryPath = directoryPath;
            }
            catch
            {
                return null;
            }

            if (string.IsNullOrEmpty(metadata.Id) || string.IsNullOrEmpty(metadata.Publisher) ||
                string.IsNullOrEmpty(metadata.Name) || string.IsNullOrEmpty(metadata.ExecuteFilePath) ||
                !File.Exists(metadata.ExecuteFilePath))
            {
                return null;
            }

            return metadata;
        }

        internal static bool SaveMetadataToJson(PluginMetadata metadata)
        {
            var retval = true;
            var configPath = Path.Combine(metadata.DirectoryPath, PluginConfigName);

            try
            {
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                };
                var json = JsonConvert.SerializeObject(metadata, Formatting.Indented, settings);
                FileUtils.WriteText(configPath, ECharset.utf_8, json);
            }
            catch (Exception ex)
            {
                retval = false;
                LogUtils.AddErrorLog(ex);
            }

            return retval;
        }

        internal static PluginMenu GetMenu(string pluginId, PluginMenu metadataMenu, string apiUrl, int siteId, int i)
        {
            var menu = new PluginMenu
            {
                Id = metadataMenu.Id,
                Text = metadataMenu.Text,
                Href = metadataMenu.Href,
                Target = metadataMenu.Target,
                IconClass = metadataMenu.IconClass
            };

            if (string.IsNullOrEmpty(menu.Id))
            {
                menu.Id = pluginId + i;
            }
            if (!string.IsNullOrEmpty(menu.Href))
            {
                menu.Href = PageUtils.GetPluginDirectoryUrl(pluginId, menu.Href);
                menu.Href = PageUtils.AddQueryString(menu.Href, new NameValueCollection
                {
                    {"apiUrl", JsonApi.GetUrl(apiUrl, pluginId)},
                    {"siteId", siteId.ToString()},
                    {"v", StringUtils.GetRandomInt(1, 1000).ToString()}
                });
            }
            if (string.IsNullOrEmpty(menu.Target))
            {
                menu.Target = "right";
            }

            if (metadataMenu.Menus != null && metadataMenu.Menus.Count > 0)
            {
                var chlildren = new List<PluginMenu>();
                var x = 1;
                foreach (var childMetadataMenu in metadataMenu.Menus)
                {
                    var child = GetMenu(pluginId, childMetadataMenu, apiUrl, siteId, x++);

                    chlildren.Add(child);
                }
                menu.Menus = chlildren;
            }

            return menu;
        }

        internal static void OnConfigOrDllChanged(object sender, FileSystemEventArgs e)
        {
            var directoryPath = DirectoryUtils.GetDirectoryPath(e.FullPath);

            var pluginDirectoryPath = string.Empty;
            var dirPaths = DirectoryUtils.GetDirectoryPaths(PathUtils.GetPluginsPath(string.Empty));
            foreach (var dirPath in dirPaths)
            {
                if (!DirectoryUtils.IsInDirectory(dirPath, directoryPath)) continue;
                pluginDirectoryPath = dirPath;
                break;
            }
            if (string.IsNullOrEmpty(pluginDirectoryPath)) return;

            var plugin = PluginCache.AllPluginPairs.FirstOrDefault(pluginPair => PathUtils.IsEquals(pluginDirectoryPath, pluginPair.Metadata.DirectoryPath));
            if (plugin != null)
            {
                PluginManager.DeactiveAndRemove(plugin);
                Thread.Sleep(1000);
            }

            ActivePlugin(pluginDirectoryPath);
        }

        internal static void ActivePlugin(string directoryPath)
        {
            //if (directoryPath.IndexOf("image-poll") == -1) return;

            try
            {
                var metadata = GetMetadataFromJson(directoryPath);
                if (metadata == null)
                {
                    return;
                }

                //foreach (var filePath in DirectoryUtils.GetFilePaths(DirectoryUtils.GetDirectoryPath(metadata.ExecuteFilePath)))
                //{

                //    if (!StringUtils.EqualsIgnoreCase(PathUtils.GetExtension(filePath), ".dll")) continue;
                //    var fileName = PathUtils.GetFileName(filePath);
                //    if (StringUtils.EqualsIgnoreCase(fileName, PathUtils.GetFileName(metadata.ExecuteFilePath))) continue;
                //    if (FileUtils.IsFileExists(PathUtils.Combine(WebConfigUtils.PhysicalApplicationPath, "Bin", fileName))) continue;
                //    Assembly.Load(File.ReadAllBytes(filePath));
                //}
                //var assembly = Assembly.Load(File.ReadAllBytes(metadata.ExecuteFilePath));

                foreach (var filePath in DirectoryUtils.GetFilePaths(DirectoryUtils.GetDirectoryPath(metadata.ExecuteFilePath)))
                {
                    if (!StringUtils.EqualsIgnoreCase(PathUtils.GetExtension(filePath), ".dll")) continue;

                    var fileName = PathUtils.GetFileName(filePath);
                    var binFilePath = PathUtils.Combine(WebConfigUtils.PhysicalApplicationPath, "Bin", fileName);

                    if (!FileUtils.IsFileExists(binFilePath))
                    {
                        FileUtils.MoveFile(filePath, binFilePath, false);
                    }
                    else if (StringUtils.EqualsIgnoreCase(fileName, PathUtils.GetFileName(metadata.ExecuteFilePath)))
                    {
                        if (FileUtils.ComputeHash(filePath) != FileUtils.ComputeHash(binFilePath))
                        {
                            FileUtils.MoveFile(filePath, binFilePath, true);
                        }
                    }
                }
                //var assembly = Assembly.Load(File.ReadAllBytes(PathUtils.Combine(WebConfigUtils.PhysicalApplicationPath, "Bin", PathUtils.GetFileName(metadata.ExecuteFilePath))));
                var assembly = Assembly.Load(PathUtils.GetFileNameWithoutExtension(metadata.ExecuteFilePath));

                var type = assembly.GetTypes().First(o => o.IsClass && !o.IsAbstract && o.GetInterfaces().Contains(typeof(IPlugin)));
                var plugin = (IPlugin)Activator.CreateInstance(type);

                PluginManager.ActiveAndAdd(metadata, plugin);
            }
            catch (Exception e)
            {
                LogUtils.AddErrorLog(e, $"插件加载：{directoryPath}");
            }
        }

        internal static void OnDirectoryDeleted(object sender, FileSystemEventArgs e)
        {
            var directoryPath = DirectoryUtils.GetDirectoryPath(e.FullPath);

            foreach (var pluginPair in PluginCache.AllPluginPairs)
            {
                if (!PathUtils.IsEquals(pluginPair.Metadata.DirectoryPath, directoryPath)) continue;

                PluginManager.DeactiveAndRemove(pluginPair);
                break;
            }

            Thread.Sleep(1000);
        }
    }
}
