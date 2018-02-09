using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web.UI.WebControls;
using SiteServer.CMS.Core;
using SiteServer.Utils;
using SiteServer.CMS.Plugin;
using SiteServer.CMS.Plugin.Model;

namespace SiteServer.BackgroundPages.Plugins
{
    public class PageManagement : BasePage
    {
        public Button BtnReload;
        public Repeater RptEnabled;
        public Repeater RptDisabled;
        public Repeater RptError;

        public static string GetRedirectUrl()
        {
            return PageUtils.GetPluginsUrl(nameof(PageManagement), new NameValueCollection
            {
                {"type", "1"}
            });
        }

        public static string GetRedirectUrl(int type)
        {
            return PageUtils.GetPluginsUrl(nameof(PageManagement), new NameValueCollection
            {
                {"type", type.ToString()}
            });
        }

        public int PageType { get; set; }

        public int CountEnabled { get; set; }
        public int CountDisabled { get; set; }
        public int CountError { get; set; }

        public string Packages
        {
            get
            {
                var list = new List<object>();

                var dict = PluginManager.GetPluginIdAndVersionDict();

                foreach (var pluginId in dict.Keys)
                {
                    var version = dict[pluginId];

                    var versionAndNotes = new
                    {
                        Id = pluginId,
                        Version = version
                    };

                    list.Add(versionAndNotes);
                }

                return TranslateUtils.JsonSerialize(list);
            }
        }

        public string PackageIds
        {
            get
            {
                var dict = PluginManager.GetPluginIdAndVersionDict();

                var list = dict.Keys.ToList();

                return TranslateUtils.ObjectCollectionToString(list);
            }
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (Body.IsQueryExists("delete"))
            {
                var pluginId = Body.GetQueryString("pluginId");

                PluginManager.Delete(pluginId);
                Body.AddAdminLog("删除插件", $"插件:{pluginId}");

                CacheUtils.ClearAll();
                CacheDbUtils.Clear();

                AddScript(AlertUtils.Success("插件删除成功", "插件删除成功，系统需要重载页面", "重新载入", "window.top.location.reload();"));
            }
            if (Body.IsQueryExists("enable"))
            {
                var pluginId = Body.GetQueryString("pluginId");

                PluginManager.UpdateDisabled(pluginId, false);
                Body.AddAdminLog("启用插件", $"插件:{pluginId}");

                CacheUtils.ClearAll();
                CacheDbUtils.Clear();

                AddScript(AlertUtils.Success("插件启用成功", "插件启用成功，系统需要重载页面", "重新载入", "window.top.location.reload();"));
            }
            else if (Body.IsQueryExists("disable"))
            {
                var pluginId = Body.GetQueryString("pluginId");

                PluginManager.UpdateDisabled(pluginId, true);
                Body.AddAdminLog("禁用插件", $"插件:{pluginId}");

                CacheUtils.ClearAll();
                CacheDbUtils.Clear();

                AddScript(AlertUtils.Success("插件禁用成功", "插件禁用成功，系统需要重载页面", "重新载入", "window.top.location.reload();"));
            }

            if (Page.IsPostBack) return;

            PageType = Body.GetQueryInt("type", 1);

            VerifyAdministratorPermissions(ConfigManager.Permissions.Plugins.Management);

            var listEnabled = new List<PluginInfo>();
            var listDisabled = new List<PluginInfo>();
            var listError = new List<PluginInfo>();

            foreach (var pluginInfo in PluginManager.AllPluginInfoList)
            {
                if (pluginInfo.Plugin == null)
                {
                    CountError++;
                    listError.Add(pluginInfo);
                }
                else
                {
                    if (pluginInfo.IsDisabled)
                    {
                        CountDisabled++;
                        listDisabled.Add(pluginInfo);
                    }
                    else
                    {
                        CountEnabled++;
                        listEnabled.Add(pluginInfo);
                    }
                }
            }

            RptEnabled.DataSource = listEnabled;
            RptEnabled.ItemDataBound += RptRunnable_ItemDataBound;
            RptEnabled.DataBind();

            RptDisabled.DataSource = listDisabled;
            RptDisabled.ItemDataBound += RptRunnable_ItemDataBound;
            RptDisabled.DataBind();

            RptError.DataSource = listError;
            RptError.ItemDataBound += RptError_ItemDataBound;
            RptError.DataBind();
        }

        public void BtnReload_Click(object sender, EventArgs e)
        {
            CacheUtils.ClearAll();
            CacheDbUtils.Clear();

            AddScript(AlertUtils.Success("插件重新加载成功", "插件重新加载成功，系统需要重载页面", "重新载入", "window.top.location.reload();"));
        }

        private void RptRunnable_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.AlternatingItem && e.Item.ItemType != ListItemType.Item) return;

            var pluginInfo = (PluginInfo)e.Item.DataItem;

            var ltlLogo = (Literal)e.Item.FindControl("ltlLogo");
            var ltlPluginId = (Literal)e.Item.FindControl("ltlPluginId");
            var ltlPluginName = (Literal)e.Item.FindControl("ltlPluginName");
            var ltlVersion = (Literal)e.Item.FindControl("ltlVersion");
            var ltlOwners = (Literal)e.Item.FindControl("ltlOwners");
            var ltlDescription = (Literal)e.Item.FindControl("ltlDescription");
            var ltlInitTime = (Literal)e.Item.FindControl("ltlInitTime");
            var ltlCmd = (Literal)e.Item.FindControl("ltlCmd");

            ltlLogo.Text = $@"<img src=""{PluginManager.GetPluginIconUrl(pluginInfo.Service)}"" width=""48"" height=""48"" />";
            ltlPluginId.Text = $@"<a href=""{PageView.GetRedirectUrl(pluginInfo.Id, GetRedirectUrl())}"">{pluginInfo.Id}</a>";
            ltlPluginName.Text = pluginInfo.Plugin.Title;
            ltlVersion.Text = pluginInfo.Plugin.Version;
            if (pluginInfo.Plugin.Owners != null)
            {
                ltlOwners.Text = string.Join("&nbsp;", pluginInfo.Plugin.Owners);
            }
            
            ltlDescription.Text = pluginInfo.Plugin.Description;

            if (pluginInfo.InitTime > 1000)
            {
                ltlInitTime.Text = Math.Round((double)pluginInfo.InitTime / 1000) + "秒";
            }
            else
            {
                ltlInitTime.Text = pluginInfo.InitTime + "毫秒";
            }

            var ableUrl = PageUtils.GetPluginsUrl(nameof(PageManagement), new NameValueCollection
            {
                {pluginInfo.IsDisabled ? "enable" : "disable", true.ToString()},
                {"pluginId", pluginInfo.Id}
            });

            var deleteUrl = PageUtils.GetPluginsUrl(nameof(PageManagement), new NameValueCollection
            {
                {"delete", true.ToString()},
                {"pluginId", pluginInfo.Id}
            });

            var ableText = pluginInfo.IsDisabled ? "启用" : "禁用";
            ltlCmd.Text = $@"
<a href=""javascript:;"" onClick=""{ModalTaxis.GetOpenWindowString()}"">排序</a>
&nbsp;&nbsp;
<a href=""javascript:;"" onClick=""{AlertUtils.ConfirmRedirect($"{ableText}插件", $"此操作将会{ableText}“{pluginInfo.Id}”插件，确认吗？", ableText, ableUrl)}"">
{ableText}
</a>
&nbsp;&nbsp;
<a href=""javascript:;"" onClick=""{AlertUtils.ConfirmDelete("删除插件", $"此操作将会删除“{pluginInfo.Id}”插件，确认吗？", deleteUrl)}"">删除插件</a>";
        }

        private void RptError_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.AlternatingItem && e.Item.ItemType != ListItemType.Item) return;

            var pluginInfo = (PluginInfo)e.Item.DataItem;

            var ltlPluginId = (Literal)e.Item.FindControl("ltlPluginId");
            var ltlErrorMessage = (Literal)e.Item.FindControl("ltlErrorMessage");
            var ltlCmd = (Literal)e.Item.FindControl("ltlCmd");

            ltlPluginId.Text = pluginInfo.Id;
            ltlErrorMessage.Text = pluginInfo.ErrorMessage;

            var deleteUrl = PageUtils.GetPluginsUrl(nameof(PageManagement), new NameValueCollection
            {
                {"delete", true.ToString()},
                {"pluginId", pluginInfo.Id}
            });

            ltlCmd.Text = $@"
<a href=""javascript:;"" onClick=""{AlertUtils.ConfirmDelete("删除插件", $"此操作将会删除“{pluginInfo.Id}”插件，确认吗？", deleteUrl)}"">删除插件</a>";
        }
    }
}
