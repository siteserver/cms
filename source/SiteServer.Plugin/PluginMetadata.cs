using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace SiteServer.Plugin
{
    [JsonObject(MemberSerialization.OptOut)]
    public class PluginMetadata
    {
        public string Name { get; set; }
        public string Author { get; set; }
        public string Version { get; set; }
        public string Description { get; set; }
        public string Website { get; set; }
        public bool Disabled { get; set; }
        public string ExecuteFileName { get; set; }
        public string LogoUrl { get; set; }
        public Dictionary<string, string> Permissions { get; set; }
        public List<PluginMenu> Menus { get; set; }

        public override string ToString()
        {
            return Name;
        }

        private string _directoryPath;
        [JsonIgnore]
        public string DirectoryPath
        {
            get { return _directoryPath; }
            internal set
            {
                _directoryPath = value;
                ExecuteFilePath = Path.Combine(value, ExecuteFileName);
                Id = new DirectoryInfo(_directoryPath).Name;
            }
        }

        [JsonIgnore]
        public string ExecuteFilePath { get; private set; }

        [JsonIgnore]
        public string Id { get; private set; }

        [JsonIgnore]
        public long InitTime { get; internal set; }
    }
}
