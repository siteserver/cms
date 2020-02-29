using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using SS.CMS.Abstractions;

namespace SS.CMS.Core
{
    public class PermissionConfigManager
	{
        private static readonly string CacheKey = CacheUtils.GetCacheKey(nameof(PermissionConfigManager));

	    public List<PermissionConfig> GeneralPermissions { get; } = new List<PermissionConfig>();

	    public List<PermissionConfig> WebsitePermissions { get; } = new List<PermissionConfig>();

	    public List<PermissionConfig> WebsiteSysPermissions { get; } = new List<PermissionConfig>();

	    public List<PermissionConfig> WebsitePluginPermissions { get; } = new List<PermissionConfig>();

        public List<PermissionConfig> ChannelPermissions { get; } = new List<PermissionConfig>();

        private readonly IPluginManager _pluginManager;


		private PermissionConfigManager(IPluginManager pluginManager)
        {
            _pluginManager = pluginManager;
        }

        public static async Task<PermissionConfigManager> GetInstanceAsync(IPathManager pathManager, IPluginManager pluginManager)
		{
            var permissionManager = CacheUtils.Get<PermissionConfigManager>(CacheKey);
            if (permissionManager != null) return permissionManager;

            permissionManager = new PermissionConfigManager(pluginManager);

			var tabManager = new TabManager(pathManager, pluginManager);
            var path = tabManager.GetMenusPath("Permissions.config");
            if (FileUtils.IsFileExists(path))
            {
                var doc = new XmlDocument();
                doc.Load(path);
                await permissionManager.LoadValuesFromConfigurationXmlAsync(doc);
            }

            CacheUtils.Insert(CacheKey, permissionManager, path);
            return permissionManager;
        }

        private async Task LoadValuesFromConfigurationXmlAsync(XmlDocument doc) 
		{
            var coreNode = doc.SelectSingleNode("Config");
		    if (coreNode != null)
		    {
                var isMultiple = true;
                foreach (XmlNode child in coreNode.ChildNodes)
		        {
		            if (child.NodeType == XmlNodeType.Comment) continue;
		            if (child.Name == "generalPermissions")
		            {
		                GetPermissions(child, GeneralPermissions);
		            }
		            else if (child.Name == "websitePermissions")
		            {
		                GetPermissions(child, WebsiteSysPermissions);
                        GetPermissions(child, WebsitePermissions);
		            }
		            else if (child.Name == "channelPermissions")
		            {
		                GetPermissions(child, ChannelPermissions);
		            }
		            else
		            {
		                isMultiple = false;
		                break;
		            }
		        }
		        if (!isMultiple)
		        {
		            GetPermissions(coreNode, GeneralPermissions);
		        }
		    }

            GeneralPermissions.AddRange(await _pluginManager.GetTopPermissionsAsync());
		    var pluginPermissions = await _pluginManager.GetSitePermissionsAsync(0);
            WebsitePluginPermissions.AddRange(pluginPermissions);
		    WebsitePermissions.AddRange(pluginPermissions);
        }

        private static void GetPermissions(XmlNode node, List<PermissionConfig> list) 
		{
            foreach (XmlNode permission in node.ChildNodes) 
			{
			    if (permission.Name == "add" && permission.Attributes != null)
			    {
                    list.Add(new PermissionConfig(permission.Attributes["name"].Value, permission.Attributes["text"].Value));
                }
			}
		}
	}
}
