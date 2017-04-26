using System.Collections;
using System.Web.Caching;
using System.Xml;

namespace BaiRong.Core.Configuration
{
    public class PermissionConfigManager
	{
        public const string CacheKey = "BaiRong.Core.Configuration.PermissionManager";
        public const string ModuleCacheKey = "BaiRong.Core.Configuration.ModulePermissions";

        private static string GetPermissionsFilePathOfPlatform()
        {
            return "~/SiteFiles/Configuration/Menus/Permissions.config";
        }

        private static string GetPermissionsFilePathOfApp(string appID)
        {
            return $"~/SiteFiles/Configuration/Menus/{appID}/Permissions.config";
        }

        ArrayList generalPermissions = new ArrayList();
        public ArrayList GeneralPermissions => generalPermissions;

	    ArrayList websitePermissions = new ArrayList();
        public ArrayList WebsitePermissions => websitePermissions;

	    ArrayList channelPermissions = new ArrayList();
        public ArrayList ChannelPermissions => channelPermissions;

	    ArrayList govInteractPermissions = new ArrayList();
        public ArrayList GovInteractPermissions => govInteractPermissions;

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

                    //ArrayList pathArrayList = new ArrayList();
                    //string path = PathUtils.MapPath(PermissionsPathGlobal);
                    //pathArrayList.Add(path);
                    //XmlDocument doc = new XmlDocument();
                    //doc.Load(path);
                    //permissionManager.LoadValuesFromConfigurationXml(doc, false);

                    var pathArrayList = new ArrayList();

                    var path = PathUtils.MapPath(GetPermissionsFilePathOfPlatform());
                    if (FileUtils.IsFileExists(path))
                    {
                        pathArrayList.Add(path);

                        var doc = new XmlDocument();
                        doc.Load(path);
                        permissionManager.LoadValuesFromConfigurationXml(doc);
                    }

                    foreach (var appID in AppManager.GetAppIdList())
                    {
                        path = PathUtils.MapPath(GetPermissionsFilePathOfApp(appID));
                        if (FileUtils.IsFileExists(path))
                        {
                            pathArrayList.Add(path);

                            var doc = new XmlDocument();
                            doc.Load(path);
                            permissionManager.LoadValuesFromConfigurationXml(doc);
                        }
                    }

                    CacheUtils.Max(CacheKey, permissionManager, new CacheDependency(TranslateUtils.ArrayListToStringArray(pathArrayList)));
				}
                return permissionManager;
			}
		}

        public static void SaveGeneralPermissionsOfApp(string appID, ArrayList permissions)
        {
            var path = PathUtils.MapPath(GetPermissionsFilePathOfApp(appID));
            if (FileUtils.IsFileExists(path))
            {
                var doc = new XmlDocument();
                doc.Load(path);

                var configSettings = doc.SelectSingleNode("Config/generalPermissions");
                if (configSettings == null)
                {
                    configSettings = doc.SelectSingleNode("Config");
                }

                configSettings.RemoveAll();
                foreach (PermissionConfig config in permissions)
                {
                    var setting = doc.CreateElement("add");
                    setting.SetAttribute("name", config.Name);
                    setting.SetAttribute("text", config.Text);
                    configSettings.AppendChild(setting);
                }

                var writer = new XmlTextWriter(path, System.Text.Encoding.UTF8);
                writer.Formatting = Formatting.Indented;
                doc.Save(writer);
                writer.Flush();
                writer.Close();
            }
        }

        public static ArrayList GetGeneralPermissionsOfProduct()
        {
            var permissions = new ArrayList();

            var path = PathUtils.MapPath(GetPermissionsFilePathOfPlatform());
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

            foreach (var appID in AppManager.GetAppIdList())
            {
                path = PathUtils.MapPath(GetPermissionsFilePathOfApp(appID));
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
            }
            return permissions;
        }

        public static ArrayList GetGeneralPermissionsOfApp(string appID)
        {
            var permissions = new ArrayList();

            var path = PathUtils.MapPath(GetPermissionsFilePathOfApp(appID));
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

        public static ArrayList GetWebsitePermissionsOfApp(string appID)
        {
            var permissions = new ArrayList();
            if (!string.IsNullOrEmpty(appID))
            {
                var path = PathUtils.MapPath(GetPermissionsFilePathOfApp(appID));
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
            }
            return permissions;
        }

        public static ArrayList GetChannelPermissionsOfApp(string appID)
        {
            var permissions = new ArrayList();
            if (!string.IsNullOrEmpty(appID))
            {
                var path = PathUtils.MapPath(GetPermissionsFilePathOfApp(appID));
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
                    GetPermissions(child, generalPermissions);
                }
                else if (child.Name == "websitePermissions")
                {
                    GetPermissions(child, websitePermissions);
                }
                else if (child.Name == "channelPermissions")
                {
                    GetPermissions(child, channelPermissions);
                }
                else if (child.Name == "govInteractPermissions")
                {
                    GetPermissions(child, govInteractPermissions);
                }
                else
                {
                    isMultiple = false;
                    break;
                }
            }
            if (!isMultiple)
            {
                GetPermissions(coreNode, generalPermissions);
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
