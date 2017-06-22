using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core.Plugin;
using SiteServer.Plugin;

namespace SiteServer.BackgroundPages.Sys
{
    public class PagePlugin : BasePageCms
    {
        public Literal LtlNav;
        public DataGrid DgPlugins;
        public Button BtnImport;

        private int _type;

        public static string GetRedirectUrl(int type)
        {
            return PageUtils.GetSysUrl(nameof(PagePlugin), new NameValueCollection
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
                    var pluginPair = PluginManager.Delete(pluginId);
                    Body.AddAdminLog("删除插件", $"插件:{pluginPair.Metadata.Name}");
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
                    var pluginPair = PluginManager.Enable(pluginId);
                    Body.AddAdminLog("启用插件", $"插件:{pluginPair.Metadata.Name}");
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
                    var pluginPair = PluginManager.Disable(pluginId);
                    Body.AddAdminLog("禁用插件", $"插件:{pluginPair.Metadata.Name}");
                    SuccessMessage("成功禁用插件");
                }
                catch (Exception ex)
                {
                    FailDeleteMessage(ex);
                }
            }

            if (Page.IsPostBack) return;

            _type = Body.GetQueryInt("type");

            BreadCrumbSys(AppManager.Sys.LeftMenu.Plugin, "插件管理", AppManager.Sys.Permission.SysPlugin);

            var list = new List<PluginPair>();
            int[] arr = {0, 0, 0};
            foreach (var pluginPair in PluginManager.AllPlugins)
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

            const string activeStyle = @"style=""color: #333;font-weight: bold;""";

            LtlNav.Text = $@"
<a href=""{GetRedirectUrl(0)}"" {(_type == 0 ? activeStyle : string.Empty)}> 所有插件 </a> <span class=""gray"">({arr[0]})</span>
<span class=""gray"">&nbsp;|&nbsp;</span>
<a href=""{GetRedirectUrl(1)}"" {(_type == 1 ? activeStyle : string.Empty)}> 已启用 </a> <span class=""gray"">({arr[1]})</span>
<span class=""gray"">&nbsp;|&nbsp;</span>
<a href=""{GetRedirectUrl(2)}"" {(_type == 2 ? activeStyle : string.Empty)}> 已禁用 </a> <span class=""gray"">({arr[2]})</span>";

            DgPlugins.DataSource = list;
            DgPlugins.ItemDataBound += DgPlugins_ItemDataBound;
            DgPlugins.DataBind();

            BtnImport.Attributes.Add("onclick", ModalImportZip.GetOpenWindowString(ModalImportZip.TypePlugin));
        }

        private void DgPlugins_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.AlternatingItem && e.Item.ItemType != ListItemType.Item) return;
            var pluginPair = (PluginPair)e.Item.DataItem;

            var ltlPluginId = (Literal)e.Item.FindControl("ltlPluginId");
            var ltlPluginName = (Literal)e.Item.FindControl("ltlPluginName");
            var ltlDescription = (Literal)e.Item.FindControl("ltlDescription");
            var ltlInitTime = (Literal)e.Item.FindControl("ltlInitTime");
            var ltlCmd = (Literal)e.Item.FindControl("ltlCmd");

            ltlPluginId.Text = pluginPair.Metadata.Id;
            ltlPluginName.Text = $@"<img src={PageUtils.GetPluginUrl(pluginPair.Metadata.Id, pluginPair.Metadata.LogoUrl)} width=""48"" height=""48"" /> {pluginPair.Metadata.Name}";
            ltlDescription.Text = $@"{pluginPair.Metadata.Description}<br />
Version： {pluginPair.Metadata.Version}
<span class=""gray"">&nbsp;|&nbsp;</span>
作者： <a href="""" target=""_blank"">{pluginPair.Metadata.Author}</a>
";

            if (pluginPair.Metadata.InitTime > 1000)
            {
                ltlInitTime.Text = Math.Round((double)pluginPair.Metadata.InitTime / 1000) + "秒";
            }
            else
            {
                ltlInitTime.Text = pluginPair.Metadata.InitTime + "毫秒";
            }

            var ableText = pluginPair.Metadata.Disabled ? "启用" : "禁用";
            ltlCmd.Text =
                $@"<a href=""{PageUtils.GetSysUrl(nameof(PagePlugin), new NameValueCollection
                {
                    {pluginPair.Metadata.Disabled ? "enable" : "disable", true.ToString()},
                    {"pluginId", pluginPair.Metadata.Id}
                })}"" onClick=""javascript:return confirm('此操作将会{ableText}“{pluginPair.Metadata.Name}”插件，确认吗？');"">{ableText}</a>
&nbsp;&nbsp;
<a href=""{PageUtils.GetSysUrl(nameof(PagePlugin), new NameValueCollection
                {
                    {"delete", true.ToString()},
                    {"pluginId", pluginPair.Metadata.Id}
                })}"" onClick=""javascript:return confirm('此操作将会删除“{pluginPair.Metadata.Name}”插件，确认吗？');"">删除</a>";
        }
    }
}
