using System;
using System.Collections.Generic;
using System.IO;
using BaiRong.Core;
using Newtonsoft.Json;
using SiteServer.Plugin;

namespace SiteServer.CMS.Plugin
{
    internal abstract class PluginConfig
    {
        private const string PluginConfigName = "plugin.json";
        private static readonly List<PluginMetadata> PluginMetadatas = new List<PluginMetadata>();

        /// <summary>
        /// Parse plugin metadata in giving directories
        /// </summary>
        /// <returns></returns>
        public static List<PluginMetadata> Parse()
        {
            PluginMetadatas.Clear();
            var directories = DirectoryUtils.GetDirectoryPaths(PathUtils.GetPluginsPath());
            ParsePluginConfigs(directories);
            return PluginMetadatas;
        }

        private static void ParsePluginConfigs(IEnumerable<string> directories)
        {
            // todo use linq when diable plugin is implmented since parallel.foreach + list is not thread saft
            foreach (var directory in directories)
            {
                PluginMetadata metadata = GetPluginMetadata(directory);
                if (metadata != null)
                {
                    PluginMetadatas.Add(metadata);
                }
            }
        }

        private static PluginMetadata GetPluginMetadata(string directoryPath)
        {
            string configPath = Path.Combine(directoryPath, PluginConfigName);
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
            catch (Exception e)
            {
                return null;
            }

            if (!File.Exists(metadata.ExecuteFilePath))
            {
                return null;
            }

            return metadata;
        }
    }
}