using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Plugin;
using SiteServer.CMS.Plugin.Model;

namespace SiteServer.BackgroundPages.Plugins
{
    public class PageManagement : BasePage
    {
        public Literal LtlNav;
        public PlaceHolder PhRunnable;
        public Repeater RptRunnable;
        public PlaceHolder PhNotRunnable;
        public Repeater RptNotRunnable;

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
                    PluginManager.Delete(pluginId);
                    Body.AddAdminLog("删除插件");
                    PageUtils.Redirect(PageMain.GetRedirectUrl());
                    return;
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
                    PluginManager.UpdateDisabled(pluginId, false);
                    Body.AddAdminLog("启用插件", $"插件:{pluginId}");
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
                    PluginManager.UpdateDisabled(pluginId, true);
                    Body.AddAdminLog("禁用插件", $"插件:{pluginId}");
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

            var list = new List<PluginInfo>();
            int[] arr = {0, 0, 0, 0};
            foreach (var pluginInfo in PluginManager.AllPluginInfoList)
            {
                if (pluginInfo.Context == null || pluginInfo.Plugin == null || pluginInfo.Metadata == null)
                {
                    arr[3]++;
                    if (_type == 3)
                    {
                        list.Add(pluginInfo);
                    }
                }
                else
                {
                    if (!pluginInfo.IsDisabled)
                    {
                        arr[0]++;
                        arr[1]++;
                    }
                    else
                    {
                        arr[0]++;
                        arr[2]++;
                    }

                    if (_type == 0)
                    {
                        list.Add(pluginInfo);
                    }
                    else if (_type == 1)
                    {
                        if (!pluginInfo.IsDisabled)
                        {
                            list.Add(pluginInfo);
                        }
                    }
                    else if (_type == 2)
                    {
                        if (pluginInfo.IsDisabled)
                        {
                            list.Add(pluginInfo);
                        }
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
            if (arr[3] > 0)
            {
                LtlNav.Text += $@"
<li class=""nav-item {(_type == 3 ? "active" : string.Empty)}"">
    <a class=""nav-link"" href=""{GetRedirectUrl(3)}"">运行错误 <span class=""badge badge-danger"" {(_type == 3
                    ? @"style=""color: #fff"""
                    : "")}>{arr[3]}</span></a>
</li>";
                
            }
            
            if (_type == 3)
            {
                PhRunnable.Visible = false;
                PhNotRunnable.Visible = true;

                RptNotRunnable.DataSource = list;
                RptNotRunnable.ItemDataBound += RptNotRunnable_ItemDataBound;
                RptNotRunnable.DataBind();
            }
            else
            {
                PhRunnable.Visible = true;
                PhNotRunnable.Visible = false;

                RptRunnable.DataSource = list;
                RptRunnable.ItemDataBound += RptRunnable_ItemDataBound;
                RptRunnable.DataBind();
            }
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
            ltlPluginId.Text = $@"<a href=""{pluginInfo.Metadata.ProjectUrl}"" target=""_blank"">{pluginInfo.Id}</a>";
            ltlPluginName.Text = pluginInfo.Metadata.Title;
            ltlVersion.Text = pluginInfo.Metadata.Version;
            if (pluginInfo.Metadata.Owners != null)
            {
                ltlOwners.Text = string.Join("&nbsp;", pluginInfo.Metadata.Owners);
            }
            
            ltlDescription.Text = pluginInfo.Metadata.Description;

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
<a href=""javascript:;"" onClick=""{AlertUtils.ConfirmDelete("删除插件", $"此操作将会删除“{pluginInfo.Metadata.Id}”插件，确认吗？", deleteUrl)}"">删除插件</a>";
        }

        private void RptNotRunnable_ItemDataBound(object sender, RepeaterItemEventArgs e)
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
