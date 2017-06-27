using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using Newtonsoft.Json;
using SiteServer.CMS.Controllers;
using SiteServer.Plugin;

namespace SiteServer.CMS.Core.Plugin
{
    internal static class PluginUtils
    {
        internal const string PluginConfigName = "plugin.json";

        static PluginUtils()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            return AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(anAssembly => anAssembly.FullName == args.Name);
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

            if (string.IsNullOrEmpty(metadata.Id) || string.IsNullOrEmpty(metadata.Name) || string.IsNullOrEmpty(metadata.ExecuteFilePath) || !File.Exists(metadata.ExecuteFilePath))
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

        internal static string GetPermissionName(string pluginId, string permission)
        {
            return pluginId + "_" + permission;
        }

        internal static PluginMenu GetMenu(string pluginId, PluginMenu metadataMenu, string apiUrl, int siteId, int i)
        {
            var menu = new PluginMenu
            {
                Id = metadataMenu.Id,
                TopId = metadataMenu.TopId,
                ParentId = metadataMenu.ParentId,
                Text = metadataMenu.Text,
                Href = metadataMenu.Href,
                Selected = metadataMenu.Selected,
                Target = metadataMenu.Target,
                IconUrl = metadataMenu.IconUrl
            };

            if (string.IsNullOrEmpty(menu.Id))
            {
                menu.Id = pluginId + i;
            }
            if (!string.IsNullOrEmpty(menu.Href))
            {
                if (!PageUtils.IsProtocolUrl(menu.Href) && !StringUtils.StartsWith(menu.Href, "/"))
                {
                    menu.Href = PageUtils.GetPluginUrl(pluginId, menu.Href);
                }
                menu.Href = PageUtils.AddQueryString(menu.Href, new NameValueCollection
                {
                    {"apiUrl", Plugins.GetUrl(apiUrl, pluginId, siteId)},
                    {"siteId", siteId.ToString()},
                    {"v", StringUtils.GetRandomInt(1, 1000).ToString()}
                });
            }
            if (!string.IsNullOrEmpty(menu.IconUrl))
            {
                menu.IconUrl = PageUtils.GetPluginUrl(pluginId, menu.IconUrl);
            }
            if (string.IsNullOrEmpty(menu.Target))
            {
                menu.Target = "right";
            }
            if (metadataMenu.Permissions != null && metadataMenu.Permissions.Count > 0)
            {
                menu.Permissions = new List<string>();
                foreach (var permission in metadataMenu.Permissions)
                {
                    menu.Permissions.Add(GetPermissionName(pluginId, permission));
                }
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

        internal static void OnConfigChanged(object sender, FileSystemEventArgs e)
        {
            var directoryPath = DirectoryUtils.GetDirectoryPath(e.FullPath);

            foreach (var pluginPair in PluginManager.AllPlugins)
            {
                if (!PathUtils.IsEquals(pluginPair.Metadata.DirectoryPath, directoryPath)) continue;
                pluginPair.Plugin.Deactive(pluginPair.Context);
                PluginManager.AllPlugins.Remove(pluginPair);
                break;
            }

            Thread.Sleep(1000);

            ActivePlugin(directoryPath);
        }

        internal static void ActivePlugin(string directoryPath)
        {
            var s = Stopwatch.StartNew();

            if (directoryPath.IndexOf("image-poll") == -1) return;

            var metadata = GetMetadataFromJson(directoryPath);
            if (metadata == null)
            {
                return;
            }

            Assembly assembly;
            Type type;
            IPlugin plugin;

            try
            {
                //assembly = Assembly.Load(AssemblyName.GetAssemblyName(metadata.ExecuteFilePath));
                foreach (var filePath in DirectoryUtils.GetFilePaths(DirectoryUtils.GetDirectoryPath(metadata.ExecuteFilePath)))
                {
                    
                    if (!StringUtils.EqualsIgnoreCase(PathUtils.GetExtension(filePath), ".dll")) continue;
                    var fileName = PathUtils.GetFileName(filePath);
                    if (StringUtils.EqualsIgnoreCase(fileName, PathUtils.GetFileName(metadata.ExecuteFilePath))) continue;
                    if (FileUtils.IsFileExists(PathUtils.Combine(WebConfigUtils.PhysicalApplicationPath, "Bin", fileName))) continue;
                    Assembly.Load(File.ReadAllBytes(filePath));
                }

                assembly = Assembly.Load(File.ReadAllBytes(metadata.ExecuteFilePath));
            }
            catch (Exception e)
            {
                LogUtils.AddErrorLog(e, $"Couldn't load assembly for {metadata.Name}");
                return;
            }
            
            try
            {
                type = assembly.GetTypes().First(o => o.IsClass && !o.IsAbstract && o.GetInterfaces().Contains(typeof(IPlugin)));
            }
            catch (Exception e)
            {
                LogUtils.AddErrorLog(e, $"Can't find class implement IPlugin for <{metadata.Name}>");
                return;
            }
            
            try
            {
                plugin = (IPlugin)Activator.CreateInstance(type);
            }
            catch (Exception e)
            {
                LogUtils.AddErrorLog(e, $"Can't create instance for <{metadata.Name}>");
                return;
            }

            var context = new PluginContext(PluginManager.Environment, metadata, new PublicApiInstance(metadata));
            try
            {
                plugin.Active(context);
            }
            catch (Exception e)
            {
                LogUtils.AddErrorLog(e, $"Can't initialize instance for <{metadata.Name}>");
                return;
            }

            var milliseconds = s.ElapsedMilliseconds;

            metadata.InitTime = milliseconds;

            var pair = new PluginPair(context, plugin);

            PluginManager.AllPlugins.Add(pair);
        }

        internal static void OnDirectoryDeleted(object sender, FileSystemEventArgs e)
        {
            var directoryPath = DirectoryUtils.GetDirectoryPath(e.FullPath);

            foreach (var pluginPair in PluginManager.AllPlugins)
            {
                if (!PathUtils.IsEquals(pluginPair.Metadata.DirectoryPath, directoryPath)) continue;
                pluginPair.Plugin.Deactive(pluginPair.Context);
                pluginPair.Plugin.Uninstall(pluginPair.Context);
                PluginManager.AllPlugins.Remove(pluginPair);
                break;
            }

            Thread.Sleep(1000);
        }
    }
}
