using System;
using System.Collections.Generic;
using System.Net;
using System.Web.UI.WebControls;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.Utils;

namespace SiteServer.BackgroundPages.Settings
{
	public class PageUtilityParameter : BasePage
    {
        public Repeater RptContents;

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (IsPostBack) return;

            VerifySystemPermissions(ConfigManager.SettingsPermissions.Utility);

            var parameterList = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("系统主机名", Dns.GetHostName().ToUpper()),
                new KeyValuePair<string, string>("系统根目录地址", WebConfigUtils.PhysicalApplicationPath),
                new KeyValuePair<string, string>("系统程序目录地址", PathUtils.PhysicalSiteServerPath),
                new KeyValuePair<string, string>("域名", PageUtils.GetHost()),
                new KeyValuePair<string, string>("访问IP", PageUtils.GetIpAddress()),
                new KeyValuePair<string, string>(".NET版本", Environment.Version.ToString()),
                new KeyValuePair<string, string>("SiteServer CMS 版本", SystemManager.Version),
                new KeyValuePair<string, string>("SiteServer.Plugin 版本", SystemManager.PluginVersion),
                new KeyValuePair<string, string>("最近升级时间", DateUtils.GetDateAndTimeString(ConfigManager.Instance.UpdateDate)),
                new KeyValuePair<string, string>("数据库类型", WebConfigUtils.DatabaseType.Value),
                new KeyValuePair<string, string>("数据库名称", SqlUtils.GetDatabaseNameFormConnectionString(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString))
            };

            RptContents.DataSource = parameterList;
            RptContents.ItemDataBound += RptContents_ItemDataBound;
            RptContents.DataBind();
        }

        private static void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var kvp = (KeyValuePair<string, string>) e.Item.DataItem;

            var ltlName = (Literal)e.Item.FindControl("ltlName");
            var ltlValue = (Literal)e.Item.FindControl("ltlValue");

            ltlName.Text = kvp.Key;
            ltlValue.Text = kvp.Value;
        }
	}
}
