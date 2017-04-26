using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using BaiRong.Core.Text;
using SiteServer.BackgroundPages.Cms;
using SiteServer.CMS.Core;
using SiteServer.CMS.ImportExport;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Sys
{
    public class PageSiteTemplate : BasePageCms
    {
        public DataGrid dgContents;
        public Button Import;

        private SortedList _sortedlist = new SortedList();

        public static string GetRedirectUrl()
        {
            return PageUtils.GetSysUrl(nameof(PageSiteTemplate), null);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (Body.IsQueryExists("Delete"))
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

            if (!Page.IsPostBack)
            {
                BreadCrumbSys(AppManager.Sys.LeftMenu.Site, "站点模板管理", AppManager.Sys.Permission.SysSite);

                _sortedlist = SiteTemplateManager.Instance.GetSiteTemplateSortedList();
                BindGrid();

                Import.Attributes.Add("onclick", ModalUploadSiteTemplate.GetOpenWindowString());
            }
        }

        public void BindGrid()
        {
            try
            {
                var directoryArrayList = new ArrayList();
                foreach (string directoryName in _sortedlist.Keys)
                {
                    var directoryPath = PathUtility.GetSiteTemplatesPath(directoryName);
                    var dirInfo = new DirectoryInfo(directoryPath);
                    directoryArrayList.Add(dirInfo);
                }

                dgContents.DataSource = directoryArrayList;
                dgContents.ItemDataBound += dgContents_ItemDataBound;
                dgContents.DataBind();
            }
            catch (Exception ex)
            {
                PageUtils.RedirectToErrorPage(ex.Message);
            }
        }

        void dgContents_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                var dirInfo = (DirectoryInfo)e.Item.DataItem;

                var ltlTemplateName = e.Item.FindControl("ltlTemplateName") as Literal;
                var ltlTemplateType = e.Item.FindControl("ltlTemplateType") as Literal;
                var ltlDirectoryName = e.Item.FindControl("ltlDirectoryName") as Literal;
                var ltlDescription = e.Item.FindControl("ltlDescription") as Literal;
                var ltlCreationDate = e.Item.FindControl("ltlCreationDate") as Literal;
                var ltlDownloadUrl = e.Item.FindControl("ltlDownloadUrl") as Literal;
                var ltlCreateUrl = e.Item.FindControl("ltlCreateUrl") as Literal;
                var ltlDeleteUrl = e.Item.FindControl("ltlDeleteUrl") as Literal;

                var siteTemplateInfo = _sortedlist[dirInfo.Name] as SiteTemplateInfo;
                if (siteTemplateInfo != null && !string.IsNullOrEmpty(siteTemplateInfo.SiteTemplateName))
                {
                    ltlTemplateName.Text = siteTemplateInfo.SiteTemplateName;
                    var publishmentSystemType = EPublishmentSystemTypeUtils.GetEnumType(siteTemplateInfo.PublishmentSystemType);
                    ltlTemplateType.Text = EPublishmentSystemTypeUtils.GetHtml(publishmentSystemType);
                    ltlDirectoryName.Text = dirInfo.Name;
                    ltlDescription.Text = siteTemplateInfo.Description;
                    if (!string.IsNullOrEmpty(siteTemplateInfo.PicFileName))
                    {
                        var siteTemplateUrl = PageUtility.GetSiteTemplatesUrl(dirInfo.Name);
                        ltlDownloadUrl.Text +=
                            $"<a href=\"{PageUtility.GetSiteTemplateMetadataUrl(siteTemplateUrl, siteTemplateInfo.PicFileName)}\" target=_blank>样图</a>&nbsp;&nbsp;";
                    }
                    ltlCreationDate.Text = DateUtils.GetDateString(dirInfo.CreationTime);
                    if (!string.IsNullOrEmpty(siteTemplateInfo.WebSiteUrl))
                    {
                        ltlDownloadUrl.Text +=
                            $"<a href=\"{PageUtils.ParseConfigRootUrl(siteTemplateInfo.WebSiteUrl)}\" target=_blank>演示</a>&nbsp;&nbsp;";
                    }

                    var fileName = dirInfo.Name + ".zip";
                    var filePath = PathUtility.GetSiteTemplatesPath(fileName);
                    if (FileUtils.IsFileExists(filePath))
                    {
                        ltlDownloadUrl.Text +=
                            $@"<a href=""javascript:;"" onclick=""{Cms.ModalProgressBar.GetOpenWindowStringWithSiteTemplateZip(
                                dirInfo.Name)}"">重新压缩</a>&nbsp;&nbsp;";

                        ltlDownloadUrl.Text +=
                            $@"<a href=""{PageUtility.GetSiteTemplatesUrl(fileName)}"" target=""_blank"">下载压缩包</a>";
                    }
                    else
                    {
                        ltlDownloadUrl.Text +=
                            $@"<a href=""javascript:;"" onclick=""{Cms.ModalProgressBar.GetOpenWindowStringWithSiteTemplateZip(
                                dirInfo.Name)}"">压缩</a>";
                    }

                    var urlAdd = PagePublishmentSystemAdd.GetRedirectUrl(dirInfo.Name);
                    ltlCreateUrl.Text = $@"<a href=""{urlAdd}"">创建站点</a>";

                    var urlDelete = PageUtils.GetSysUrl(nameof(PageSiteTemplate), new NameValueCollection
                    {
                        {"Delete", "True"},
                        {"SiteTemplateDir", dirInfo.Name}
                    });
                    ltlDeleteUrl.Text =
                        $@"<a href=""{urlDelete}"" onClick=""javascript:return confirm('此操作将会删除此站点模板“{siteTemplateInfo
                            .SiteTemplateName}”，确认吗？');"">删除</a>";
                }
            }
        }

    }
}
