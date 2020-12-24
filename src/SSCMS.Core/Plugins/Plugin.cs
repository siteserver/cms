using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using SSCMS.Configuration;
using SSCMS.Plugins;
using SSCMS.Utils;

namespace SSCMS.Core.Plugins
{
    public class Plugin : IPlugin
    {
        public Plugin(string folderPath, bool reloadOnChange)
        {
            ContentRootPath = folderPath;
            WebRootPath = PathUtils.Combine(folderPath, Constants.WwwrootDirectory);
            var builder = new ConfigurationBuilder()
                .SetBasePath(folderPath)
                .AddJsonFile(Constants.PackageFileName, optional: false, reloadOnChange: reloadOnChange);
            if (FileUtils.IsFileExists(PathUtils.Combine(ContentRootPath, Constants.PluginConfigFileName)))
            {
                builder.AddJsonFile(Constants.PluginConfigFileName, optional: false, reloadOnChange: reloadOnChange);
            }
            Configuration = builder.Build();
        }

        public string GetAssemblyPath()
        {
            string assemblyPath;
            if (FileUtils.IsFileExists(PathUtils.Combine(ContentRootPath, Output, Main)))
            {
                assemblyPath = PathUtils.Combine(ContentRootPath, Output, Main);
            }
            else if (FileUtils.IsFileExists(PathUtils.Combine(ContentRootPath, Main)))
            {
                assemblyPath = PathUtils.Combine(ContentRootPath, Main);
            }
            else
            {
                assemblyPath = Directory.GetFiles(ContentRootPath, Main, SearchOption.AllDirectories).FirstOrDefault();
            }

            return assemblyPath;
        }

        public (bool, string) LoadAssembly()
        {
            if (string.IsNullOrEmpty(Main)) return (true, string.Empty);

            var assemblyPath = GetAssemblyPath();

            if (string.IsNullOrEmpty(assemblyPath)) return (false, $"{Main}可执行文件不存在");

            try
            {
                Assembly = PluginUtils.LoadAssembly(assemblyPath);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }

            return (true, string.Empty);
        }

        public string PluginId => $"{Publisher}.{Name}";

        public string ContentRootPath { get; }

        public string WebRootPath { get; }

        public Assembly Assembly { get; private set; }

        public IConfiguration Configuration { get; }

        public string Name => Configuration[nameof(Name)];
        public string Version => Configuration[nameof(Version)];
        public string Publisher => Configuration[nameof(Publisher)];
        public string Repository => Configuration[nameof(Repository)];
        public string DisplayName => Configuration[nameof(DisplayName)];
        public string Description => Configuration[nameof(Description)];
        public string License => Configuration[nameof(License)];
        public string Icon => Configuration[nameof(Icon)];

        public Dictionary<string, string> Engines => Configuration.GetSection(nameof(Engines)).Get<Dictionary<string, string>>();

        public IEnumerable<string> Categories => Configuration.GetSection(nameof(Categories)).Get<string[]>();
        public IEnumerable<string> Keywords => Configuration.GetSection(nameof(Keywords)).Get<string[]>();
        public string Homepage => Configuration[nameof(Homepage)];
        public string Output => Configuration[nameof(Output)];
        public string Main => Configuration[nameof(Main)];
        public bool ApplyToSites => Configuration.GetValue(nameof(ApplyToSites), false);
        public bool ApplyToChannels => Configuration.GetValue(nameof(ApplyToChannels), false);

        public bool Disabled => Configuration.GetValue<bool>(nameof(Disabled));
        public bool AllSites => Configuration.GetValue(nameof(AllSites), true);
        public IEnumerable<int> SiteIds => Configuration.GetSection(nameof(SiteIds)).Get<int[]>();
        public IEnumerable<SiteConfig> SiteConfigs => Configuration.GetSection(nameof(SiteConfigs)).Get<SiteConfig[]>();
        public IEnumerable<Table> Tables => Configuration.GetSection(nameof(Tables)).Get<Table[]>();

        public bool Success { get; set; }
        public string ErrorMessage { get; set; }

        public int Taxis => Configuration.GetValue<int>(nameof(Taxis));

        public List<Menu> GetMenus()
        {
            var section = Configuration.GetSection("extensions:menus");
            return GetMenus(section);
        }

        private List<Menu> GetMenus(IConfigurationSection section)
        {
            var menus = new List<Menu>();
            if (section.Exists())
            {
                var children = section.GetChildren();
                if (children != null)
                {
                    foreach (var child in children)
                    {
                        var menu = child.Get<Menu>();
                        var childSection = child.GetSection("menus");

                        menus.Add(new Menu
                        {
                            Id = child.Key,
                            Text = menu.Text,
                            Type = menu.Type,
                            IconClass = menu.IconClass,
                            Link = menu.Link,
                            Target = menu.Target,
                            Permissions = menu.Permissions,
                            Order = menu.Order,
                            Children = GetMenus(childSection)
                        });
                    }
                }
            }

            return menus.OrderByDescending(x => x.Order.HasValue).ThenBy(x => x.Order).ToList();
        }
    }
}
