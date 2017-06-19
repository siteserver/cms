using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.BackgroundPages.Cms;
using SiteServer.CMS.Core;
using SiteServer.CMS.ImportExport;
using SiteServer.CMS.Model;
using SiteServer.CMS.Plugin;
using SiteServer.Plugin;

namespace SiteServer.BackgroundPages.Sys
{
    public class PagePlugin : BasePageCms
    {
        public DataGrid DgEnabled;
        public DataGrid DgDisabled;
        public Button BtnImport;

        public static string GetRedirectUrl()
        {
            return PageUtils.GetSysUrl(nameof(PagePlugin), null);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (Body.IsQueryExists("DeleteDirectory"))
            {
                var siteTemplateDir = Body.GetQueryString("SiteTemplateDir");

                try
                {
                    SiteTemplateManager.Instance.DeleteSiteTemplate(siteTemplateDir);

                    Body.AddAdminLog("删除站点模板", $"站点模板:{siteTemplateDir}");

                    SuccessDeleteMessage();
                }
                catch (Exception ex)
                {
                    FailDeleteMessage(ex);
                }
            }
            else if (Body.IsQueryExists("DeleteZipFile"))
            {
                var fileName = Body.GetQueryString("FileName");

                try
                {
                    SiteTemplateManager.Instance.DeleteZipSiteTemplate(fileName);

                    Body.AddAdminLog("删除未解压站点模板", $"站点模板:{fileName}");

                    SuccessDeleteMessage();
                }
                catch (Exception ex)
                {
                    FailDeleteMessage(ex);
                }
            }

            if (Page.IsPostBack) return;

            BreadCrumbSys(AppManager.Sys.LeftMenu.Plugin, "插件管理", AppManager.Sys.Permission.SysPlugin);

            var disabledList = new List<PluginPair>();
            var enabledList = new List<PluginPair>();
            foreach (var pluginPair in PluginManager.AllPlugins)
            {
                if (pluginPair.Metadata.Disabled)
                {
                    disabledList.Add(pluginPair);
                }
                else
                {
                    enabledList.Add(pluginPair);
                }
            }

            DgEnabled.DataSource = enabledList;
            DgEnabled.ItemDataBound += DgEnabled_ItemDataBound;
            DgEnabled.DataBind();

            if (disabledList.Count > 0)
            {
                DgDisabled.Visible = true;
                DgDisabled.DataSource = disabledList;
                DgDisabled.ItemDataBound += DgDisabled_ItemDataBound;
                DgDisabled.DataBind();
            }
            else
            {
                DgDisabled.Visible = false;
            }

            BtnImport.Attributes.Add("onclick", ModalUploadSiteTemplate.GetOpenWindowString());
        }

        private void DgEnabled_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.AlternatingItem && e.Item.ItemType != ListItemType.Item) return;
            var pluginPair = (PluginPair)e.Item.DataItem;

            var ltlPluginName = (Literal)e.Item.FindControl("ltlPluginName");
            var ltlDirectoryName = (Literal)e.Item.FindControl("ltlDirectoryName");
            var ltlDescription = (Literal)e.Item.FindControl("ltlDescription");
            var ltlInitTime = (Literal)e.Item.FindControl("ltlInitTime");
            var ltlDeleteUrl = (Literal)e.Item.FindControl("ltlDeleteUrl");

            ltlPluginName.Text = $@"<img src={PageUtils.GetPluginUrl(pluginPair.Metadata.Id, pluginPair.Metadata.LogoUrl)} width=""48"" height=""48"" /> {pluginPair.Metadata.Name}";
            ltlDirectoryName.Text = pluginPair.Metadata.Id;
            ltlDescription.Text = pluginPair.Metadata.Description;

            if (pluginPair.Metadata.InitTime > 1000)
            {
                ltlInitTime.Text = Math.Round((double)pluginPair.Metadata.InitTime / 1000) + "秒";
            }
            else
            {
                ltlInitTime.Text = pluginPair.Metadata.InitTime + "毫秒";
            }

            var urlDelete = PageUtils.GetSysUrl(nameof(PageSiteTemplate), new NameValueCollection
            {
                {"DeleteDirectory", "True"},
                {"SiteTemplateDir", pluginPair.Metadata.Id}
            });
            ltlDeleteUrl.Text =
                $@"<a href=""{urlDelete}"" onClick=""javascript:return confirm('此操作将会删除此插件“{pluginPair.Metadata.Name}”，确认吗？');"">删除</a>";
        }

        private void DgDisabled_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.AlternatingItem && e.Item.ItemType != ListItemType.Item) return;
            var pluginPair = (PluginPair)e.Item.DataItem;

            var ltlPluginName = (Literal)e.Item.FindControl("ltlPluginName");
            var ltlCreationDate = (Literal)e.Item.FindControl("ltlCreationDate");
            var ltlDownloadUrl = (Literal)e.Item.FindControl("ltlDownloadUrl");
            var ltlDeleteUrl = (Literal)e.Item.FindControl("ltlDeleteUrl");

            var urlDelete = PageUtils.GetSysUrl(nameof(PageSiteTemplate), new NameValueCollection
                {
                    {"DeleteZipFile", "True"},
                    {"FileName", pluginPair.Metadata.Id}
                });
            ltlDeleteUrl.Text =
                $@"<a href=""{urlDelete}"" onClick=""javascript:return confirm('此操作将会删除此插件“{pluginPair.Metadata.Name}”，确认吗？');"">删除</a>";
        }

    }
}
