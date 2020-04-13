using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using CacheManager.Core;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.Utils
{
    public class PermissionConfigManager
	{
        public List<PermissionConfig> GeneralPermissions { get; } = new List<PermissionConfig>();

	    public List<PermissionConfig> WebsitePermissions { get; } = new List<PermissionConfig>();

	    public List<PermissionConfig> WebsiteSysPermissions { get; } = new List<PermissionConfig>();

	    public List<PermissionConfig> WebsitePluginPermissions { get; } = new List<PermissionConfig>();

        public List<PermissionConfig> ChannelPermissions { get; } = new List<PermissionConfig>();

        private readonly IOldPluginManager _pluginManager;

        private PermissionConfigManager(IOldPluginManager pluginManager)
        {
            _pluginManager = pluginManager;
        }

        public static async Task<PermissionConfigManager> GetInstanceAsync(ICacheManager<object> cacheManager, IPathManager pathManager, IOldPluginManager pluginManager)
		{
            var path = pathManager.GetConfigPath("Permissions.config");

			var permissionManager = cacheManager.Get<PermissionConfigManager>(CacheUtils.GetPathKey(path));
            if (permissionManager != null) return permissionManager;

            permissionManager = new PermissionConfigManager(pluginManager);

            if (FileUtils.IsFileExists(path))
            {
                var doc = new XmlDocument();
                doc.Load(path);
                await permissionManager.LoadValuesFromConfigurationXmlAsync(doc);
            }

            CacheUtils.SetFileContent(cacheManager, permissionManager, path);
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
