using System.Collections;
using System.Web.Caching;
using System.Xml;
using BaiRong.Core.Configuration;

namespace BaiRong.Core
{
    public class PermissionConfigManager
	{
        public const string CacheKey = "BaiRong.Core.PermissionManager";

        public const string FilePath = "~/SiteFiles/Configuration/Menus/Permissions.config";

	    public ArrayList GeneralPermissions { get; } = new ArrayList();

	    public ArrayList WebsitePermissions { get; } = new ArrayList();

	    public ArrayList ChannelPermissions { get; } = new ArrayList();

	    public ArrayList GovInteractPermissions { get; } = new ArrayList();

	    private PermissionConfigManager()
		{
		}

        public static PermissionConfigManager Instance
		{
			get
			{
                var permissionManager = CacheUtils.Get(CacheKey) as PermissionConfigManager;
                if (permissionManager == null)
				{
                    permissionManager = new PermissionConfigManager();

                    var pathArrayList = new ArrayList();

                    var path = PathUtils.MapPath(FilePath);
                    if (FileUtils.IsFileExists(path))
                    {
                        pathArrayList.Add(path);

                        var doc = new XmlDocument();
                        doc.Load(path);
                        permissionManager.LoadValuesFromConfigurationXml(doc);
                    }

                    CacheUtils.Max(CacheKey, permissionManager, new CacheDependency(TranslateUtils.ArrayListToStringArray(pathArrayList)));
				}
                return permissionManager;
			}
		}

        public static ArrayList GetGeneralPermissions()
        {
            var permissions = new ArrayList();

            var path = PathUtils.MapPath(FilePath);
            if (FileUtils.IsFileExists(path))
            {
                var XmlDoc = new XmlDocument();
                XmlDoc.Load(path);
                var coreNode = XmlDoc.SelectSingleNode("Config");

                var isGet = false;

                foreach (XmlNode child in coreNode.ChildNodes)
                {
                    if (child.Name == "generalPermissions")
                    {
                        isGet = true;
                        GetPermissions(child, permissions);
                        break;
                    }
                }

                if (isGet == false)
                {
                    GetPermissions(coreNode, permissions);
                }
            }

            return permissions;
        }

        public static ArrayList GetWebsitePermissions()
        {
            var permissions = new ArrayList();
            var path = PathUtils.MapPath(FilePath);
            if (FileUtils.IsFileExists(path))
            {
                var XmlDoc = new XmlDocument();
                XmlDoc.Load(path);
                var coreNode = XmlDoc.SelectSingleNode("Config");
                foreach (XmlNode child in coreNode.ChildNodes)
                {
                    if (child.Name == "websitePermissions")
                    {
                        GetPermissions(child, permissions);
                        break;
                    }
                }
            }
            return permissions;
        }

        public static ArrayList GetChannelPermissions()
        {
            var permissions = new ArrayList();
            var path = PathUtils.MapPath(FilePath);
            if (FileUtils.IsFileExists(path))
            {
                var XmlDoc = new XmlDocument();
                XmlDoc.Load(path);
                var coreNode = XmlDoc.SelectSingleNode("Config");
                foreach (XmlNode child in coreNode.ChildNodes)
                {
                    if (child.Name == "channelPermissions")
                    {
                        GetPermissions(child, permissions);
                        break;
                    }
                }
            }
            return permissions;
        }

        private void LoadValuesFromConfigurationXml(XmlDocument doc) 
		{
            var coreNode = doc.SelectSingleNode("Config");

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

        private static void GetPermissions(XmlNode node, ArrayList list) 
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
