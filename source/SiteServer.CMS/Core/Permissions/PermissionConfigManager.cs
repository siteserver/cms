using System.Collections.Generic;
using System.Xml;
using BaiRong.Core;
using SiteServer.CMS.Plugin;

namespace SiteServer.CMS.Core.Permissions
{
    public class PermissionConfigManager
	{
        public const string CacheKey = "SiteServer.CMS.Core.Permissions.PermissionConfigManager";

        public const string FilePath = "~/SiteFiles/Configuration/Permissions.config";

	    public List<PermissionConfig> GeneralPermissions { get; } = new List<PermissionConfig>();

	    public List<PermissionConfig> WebsitePermissions { get; } = new List<PermissionConfig>();

	    public List<PermissionConfig> ChannelPermissions { get; } = new List<PermissionConfig>();

	    public List<PermissionConfig> GovInteractPermissions { get; } = new List<PermissionConfig>();

	    private PermissionConfigManager()
		{
		}

        public static PermissionConfigManager Instance
		{
			get
			{
                var permissionManager = CacheUtils.Get(CacheKey) as PermissionConfigManager;
			    if (permissionManager != null) return permissionManager;

			    permissionManager = new PermissionConfigManager();

			    var path = PathUtils.MapPath(FilePath);
			    if (FileUtils.IsFileExists(path))
			    {
			        var doc = new XmlDocument();
			        doc.Load(path);
			        permissionManager.LoadValuesFromConfigurationXml(doc);
			    }

			    CacheUtils.Insert(CacheKey, permissionManager, path);
			    return permissionManager;
			}
		}

        //public static List<PermissionConfig> GetGeneralPermissions()
        //{
        //    var permissions = new List<PermissionConfig>();

        //    var path = PathUtils.MapPath(FilePath);
        //    if (FileUtils.IsFileExists(path))
        //    {
        //        var xmlDoc = new XmlDocument();
        //        xmlDoc.Load(path);
        //        var coreNode = xmlDoc.SelectSingleNode("Config");

        //        var isGet = false;

        //        foreach (XmlNode child in coreNode.ChildNodes)
        //        {
        //            if (child.Name == "generalPermissions")
        //            {
        //                isGet = true;
        //                GetPermissions(child, permissions);
        //                break;
        //            }
        //        }

        //        if (isGet == false)
        //        {
        //            GetPermissions(coreNode, permissions);
        //        }
        //    }

        //    return permissions;
        //}

        //public static List<PermissionConfig> GetWebsitePermissions()
        //{
        //    var permissions = new List<PermissionConfig>();
        //    var path = PathUtils.MapPath(FilePath);
        //    if (FileUtils.IsFileExists(path))
        //    {
        //        var xmlDoc = new XmlDocument();
        //        xmlDoc.Load(path);
        //        var coreNode = xmlDoc.SelectSingleNode("Config");
        //        foreach (XmlNode child in coreNode.ChildNodes)
        //        {
        //            if (child.Name == "websitePermissions")
        //            {
        //                GetPermissions(child, permissions);
        //                break;
        //            }
        //        }
        //    }
        //    return permissions;
        //}

        //public static List<PermissionConfig> GetChannelPermissions()
        //{
        //    var permissions = new List<PermissionConfig>();
        //    var path = PathUtils.MapPath(FilePath);
        //    if (FileUtils.IsFileExists(path))
        //    {
        //        var xmlDoc = new XmlDocument();
        //        xmlDoc.Load(path);
        //        var coreNode = xmlDoc.SelectSingleNode("Config");
        //        foreach (XmlNode child in coreNode.ChildNodes)
        //        {
        //            if (child.Name == "channelPermissions")
        //            {
        //                GetPermissions(child, permissions);
        //                break;
        //            }
        //        }
        //    }
        //    return permissions;
        //}

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
		                GetPermissions(child, WebsitePermissions);
		            }
		            else if (child.Name == "channelPermissions")
		            {
		                GetPermissions(child, ChannelPermissions);
		            }
		            else if (child.Name == "govInteractPermissions")
		            {
		                GetPermissions(child, GovInteractPermissions);
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

            GeneralPermissions.AddRange(PluginCache.GetTopPermissions());
            WebsitePermissions.AddRange(PluginCache.GetSitePermissions(0));
		}

        private static void GetPermissions(XmlNode node, List<PermissionConfig> list) 
		{
            foreach (XmlNode permission in node.ChildNodes) 
			{
                switch (permission.Name) 
				{
					case "add" :
                        list.Add(new PermissionConfig(permission.Attributes));
						break;
				}
			}
		}
	}
}
