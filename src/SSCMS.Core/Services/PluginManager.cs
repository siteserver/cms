using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SSCMS.Plugins;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.Services
{
    public class PluginManager : IPluginManager
    {
        private readonly List<IPluginMetadata> _plugins;

        public PluginManager(ISettingsManager settingsManager)
        {
            DirectoryPath = PathUtils.Combine(settingsManager.ContentRootPath, "plugins");
            DirectoryUtils.CreateDirectoryIfNotExists(DirectoryPath);
            _plugins = new List<IPluginMetadata>();
        }

        internal void Add(IPluginMetadata plugin)
        {
            _plugins.Add(plugin);
        }

        public string DirectoryPath { get; }

        public IEnumerable<IPluginMetadata> Plugins => _plugins.ToArray();

        public IEnumerable<Assembly> Assemblies => _plugins.Select(x => x.Assembly).Where(x => x != null);
    }
}
