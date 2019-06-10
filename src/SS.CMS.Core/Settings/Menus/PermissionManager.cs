using System.Collections.Generic;
using System.Xml;
using SS.CMS.Core.Cache.Core;
using SS.CMS.Core.Plugin;
using SS.CMS.Utils;

namespace SS.CMS.Core.Settings.Menus
{
    public class PermissionManager
    {
        private static readonly string CacheKey = DataCacheManager.GetCacheKey(nameof(PermissionManager));

        public List<Permission> GeneralPermissions { get; } = new List<Permission>();

        public List<Permission> WebsitePermissions { get; } = new List<Permission>();

        public List<Permission> WebsiteSysPermissions { get; } = new List<Permission>();

        public List<Permission> WebsitePluginPermissions { get; } = new List<Permission>();

        public List<Permission> ChannelPermissions { get; } = new List<Permission>();

        private PermissionManager()
        {
        }

        public static PermissionManager Instance
        {
            get
            {
                var permissionManager = DataCacheManager.Get<PermissionManager>(CacheKey);
                if (permissionManager != null) return permissionManager;

                permissionManager = new PermissionManager();

                var path = AppContext.GetMenusPath("Permissions.config");
                if (FileUtils.IsFileExists(path))
                {
                    var doc = new XmlDocument();
                    doc.Load(path);
                    permissionManager.LoadValuesFromConfigurationXml(doc);
                }

                DataCacheManager.Insert(CacheKey, permissionManager, path);
                return permissionManager;
            }
        }

        private void LoadValuesFromConfigurationXml(XmlDocument doc)
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

            GeneralPermissions.AddRange(PluginMenuManager.GetTopPermissions());
            var pluginPermissions = PluginMenuManager.GetSitePermissions(0);
            WebsitePluginPermissions.AddRange(pluginPermissions);
            WebsitePermissions.AddRange(pluginPermissions);
        }

        private static void GetPermissions(XmlNode node, List<Permission> list)
        {
            foreach (XmlNode permission in node.ChildNodes)
            {
                if (permission.Name == "add" && permission.Attributes != null)
                {
                    list.Add(new Permission(permission.Attributes["name"].Value, permission.Attributes["text"].Value));
                }
            }
        }
    }
}
