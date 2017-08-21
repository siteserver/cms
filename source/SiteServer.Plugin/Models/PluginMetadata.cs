using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace SiteServer.Plugin.Models
{
    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class PluginMetadata
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string Version { get; set; }
        public string Publisher { get; set; }
        public string Icon { get; set; }
        public List<string> Categories { get; set; }
        public string Homepage { get; set; }
        public string ExecuteFileName { get; set; }
        public List<string> Includes { get; set; }

        //后台配置选项
        public bool Disabled { get; set; }
        public string DatabaseType { get; set; }
        public string ConnectionString { get; set; }

        private string _directoryPath;
        [JsonIgnore]
        public string DirectoryPath
        {
            get { return _directoryPath; }
            internal set
            {
                _directoryPath = value;
                Id = (Publisher + "-" + Name).Replace(".", string.Empty).Replace(" ", string.Empty);
                DirectoryName = Path.GetFileName(Path.GetDirectoryName(value));
                ExecuteFilePath = Path.Combine(value, ExecuteFileName.Replace("/", Path.DirectorySeparatorChar.ToString()));
            }
        }

        [JsonIgnore]
        public string Id { get; private set; }

        [JsonIgnore]
        public string DirectoryName { get; private set; }

        [JsonIgnore]
        public string ExecuteFilePath { get; private set; }

        [JsonIgnore]
        public long InitTime { get; internal set; }

        public PluginMetadata Copy()
        {
            return (PluginMetadata)MemberwiseClone();
        }
    }
}
