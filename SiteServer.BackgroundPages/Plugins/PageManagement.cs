using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Plugin;
using SiteServer.Plugin.Models;

namespace SiteServer.BackgroundPages.Plugins
{
    public class PageManagement : BasePage
    {
        public Literal LtlNav;
        public Repeater RptContents;

        private int _type;

        public static string GetRedirectUrl()
        {
            return PageUtils.GetPluginsUrl(nameof(PageManagement), new NameValueCollection
            {
                {"type", "0"}
            });
        }

        public static string GetRedirectUrl(int type)
        {
            return PageUtils.GetPluginsUrl(nameof(PageManagement), new NameValueCollection
            {
                {"type", type.ToString()}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (Body.IsQueryExists("delete"))
            {
                var pluginId = Body.GetQueryString("pluginId");

                try
                {
                    var metadata = PluginManager.Delete(pluginId);
                    Body.AddAdminLog("删除插件", $"插件:{metadata.DisplayName}");
                    SuccessDeleteMessage();
                }
                catch (Exception ex)
                {
                    FailDeleteMessage(ex);
                }
            }
            else if (Body.IsQueryExists("enable"))
            {
                var pluginId = Body.GetQueryString("pluginId");

                try
                {
                    var metadata = PluginManager.UpdateDisabled(pluginId, false);
                    Body.AddAdminLog("启用插件", $"插件:{metadata.DisplayName}");
                    SuccessMessage("成功启用插件");
                }
                catch (Exception ex)
                {
                    FailDeleteMessage(ex);
                }
            }
            else if (Body.IsQueryExists("disable"))
            {
                var pluginId = Body.GetQueryString("pluginId");

                try
                {
                    var metadata = PluginManager.UpdateDisabled(pluginId, true);
                    Body.AddAdminLog("禁用插件", $"插件:{metadata.DisplayName}");
                    SuccessMessage("成功禁用插件");
                }
                catch (Exception ex)
                {
                    FailDeleteMessage(ex);
                }
            }

            if (Page.IsPostBack) return;

            _type = Body.GetQueryInt("type");

            VerifyAdministratorPermissions(AppManager.Permissions.Plugins.Management);

            var list = new List<PluginPair>();
            int[] arr = {0, 0, 0};
            foreach (var pluginPair in PluginManager.AllPluginPairs)
            {
                arr[0]++;
                if (!pluginPair.Metadata.Disabled)
                {
                    arr[1]++;
                }
                else
                {
                    arr[2]++;
                }

                if (_type == 0)
                {
                    list.Add(pluginPair);
                }
                else if (_type == 1)
                {
                    if (!pluginPair.Metadata.Disabled)
                    {
                        list.Add(pluginPair);
                    }
                }
                else if (_type == 2)
                {
                    if (pluginPair.Metadata.Disabled)
                    {
                        list.Add(pluginPair);
                    }
                }
            }

            LtlNav.Text = $@"
<li class=""nav-item {(_type == 0 ? "active" : string.Empty)}"">
    <a class=""nav-link"" href=""{GetRedirectUrl(0)}"">所有插件 <span class=""badge {(_type == 0 ? "badge-light" : "badge-secondary")}"">{arr[0]}</span></a>
</li>
<li class=""nav-item {(_type == 1 ? "active" : string.Empty)}"">
    <a class=""nav-link"" href=""{GetRedirectUrl(1)}"">已启用 <span class=""badge {(_type == 1 ? "badge-light" : "badge-secondary")}"">{arr[1]}</span></a>
</li>
<li class=""nav-item {(_type == 2 ? "active" : string.Empty)}"">
    <a class=""nav-link"" href=""{GetRedirectUrl(2)}"">已禁用 <span class=""badge {(_type == 2 ? "badge-light" : "badge-secondary")}"">{arr[2]}</span></a>
</li>";

            RptContents.DataSource = list;
            RptContents.ItemDataBound += RptContents_ItemDataBound;
            RptContents.DataBind();
        }

        private void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.AlternatingItem && e.Item.ItemType != ListItemType.Item) return;

            var pluginPair = (PluginPair)e.Item.DataItem;

            var ltlPluginId = (Literal)e.Item.FindControl("ltlPluginId");
            var ltlPluginName = (Literal)e.Item.FindControl("ltlPluginName");
            var ltlDescription = (Literal)e.Item.FindControl("ltlDescription");
            var ltlInitTime = (Literal)e.Item.FindControl("ltlInitTime");
            var ltlCmd = (Literal)e.Item.FindControl("ltlCmd");

            ltlPluginId.Text = pluginPair.Metadata.Id;
            ltlPluginName.Text = $@"<img src={PageUtils.GetPluginDirectoryUrl(pluginPair.Metadata.Id, pluginPair.Metadata.Icon)} width=""48"" height=""48"" /> {pluginPair.Metadata.DisplayName}";
            ltlDescription.Text = $@"{pluginPair.Metadata.Description}<br />
Version： {pluginPair.Metadata.Version}
<span class=""gray"">&nbsp;|&nbsp;</span>
作者： <a href="""" target=""_blank"">{pluginPair.Metadata.Publisher}</a>
";

            if (pluginPair.Metadata.InitTime > 1000)
            {
                ltlInitTime.Text = Math.Round((double)pluginPair.Metadata.InitTime / 1000) + "秒";
            }
            else
            {
                ltlInitTime.Text = pluginPair.Metadata.InitTime + "毫秒";
            }

            var ableUrl = PageUtils.GetPluginsUrl(nameof(PageManagement), new NameValueCollection
            {
                {pluginPair.Metadata.Disabled ? "enable" : "disable", true.ToString()},
                {"pluginId", pluginPair.Metadata.Id}
            });

            var deleteUrl = PageUtils.GetPluginsUrl(nameof(PageManagement), new NameValueCollection
            {
                {"delete", true.ToString()},
                {"pluginId", pluginPair.Metadata.Id}
            });

            var ableText = pluginPair.Metadata.Disabled ? "启用" : "禁用";
            ltlCmd.Text = $@"
<a href=""{PageConfig.GetRedirectUrl(pluginPair.Metadata.Id)}"">设置</a>
&nbsp;&nbsp;
<a href=""javascript:;"" onClick=""{AlertUtils.ConfirmRedirect($"{ableText}插件", $"此操作将会{ableText}“{pluginPair.Metadata.Id}”插件，确认吗？", ableText, ableUrl)}"">
{ableText}
</a>
&nbsp;&nbsp;
<a href=""javascript:;"" onClick=""{AlertUtils.ConfirmDelete("删除插件", $"此操作将会删除“{pluginPair.Metadata.Id}”插件，确认吗？", deleteUrl)}"">删除</a>";
        }
    }
}
