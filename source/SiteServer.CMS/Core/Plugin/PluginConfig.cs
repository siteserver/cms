using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BaiRong.Core;
using Newtonsoft.Json;
using SiteServer.Plugin;

namespace SiteServer.CMS.Core.Plugin
{
    internal abstract class PluginConfig
    {
        private const string PluginConfigName = "plugin.json";
        private static readonly Dictionary<string, PluginMetadata> PluginMetadatas = new Dictionary<string, PluginMetadata>();

        /// <summary>
        /// Parse plugin metadata in giving directories
        /// </summary>
        /// <returns></returns>
        public static List<PluginMetadata> Parse()
        {
            PluginMetadatas.Clear();
            var directories = DirectoryUtils.GetDirectoryPaths(PathUtils.GetPluginsPath());
            foreach (var directoryPath in directories)
            {
                string configPath = Path.Combine(directoryPath, PluginConfigName);
                if (!File.Exists(configPath))
                {
                    continue;
                }

                PluginMetadata metadata;
                try
                {
                    metadata = JsonConvert.DeserializeObject<PluginMetadata>(File.ReadAllText(configPath));
                    if (!string.IsNullOrEmpty(metadata.Id))
                    {
                        metadata.DirectoryPath = directoryPath;
                    }

                }
                catch (Exception e)
                {
                    continue;
                }

                if (!File.Exists(metadata.ExecuteFilePath))
                {
                    continue;
                }

                PluginMetadatas[metadata.Id] = metadata;
            }
            return PluginMetadatas.Values.ToList();
        }
    }
}