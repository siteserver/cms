using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.BackgroundPages.Cms;
using SiteServer.CMS.Core;
using SiteServer.CMS.ImportExport;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Settings
{
    public class PageSiteTemplate : BasePageCms
    {
        public DataGrid DgDirectories;
        public DataGrid DgZipFiles;
        public Button BtnImport;

        private SortedList _sortedlist = new SortedList();

        public static string GetRedirectUrl()
        {
            return PageUtils.GetSettingsUrl(nameof(PageSiteTemplate), null);
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

            BreadCrumbSettings("站点模板管理", AppManager.Permissions.Settings.SiteManagement);

            _sortedlist = SiteTemplateManager.Instance.GetSiteTemplateSortedList();
            var directoryList = new List<DirectoryInfo>();
            foreach (string directoryName in _sortedlist.Keys)
            {
                var directoryPath = PathUtility.GetSiteTemplatesPath(directoryName);
                var dirInfo = new DirectoryInfo(directoryPath);
                directoryList.Add(dirInfo);
            }

            DgDirectories.DataSource = directoryList;
            DgDirectories.ItemDataBound += DgDirectories_ItemDataBound;
            DgDirectories.DataBind();

            var fileNames = SiteTemplateManager.Instance.GetZipSiteTemplateList();
            var fileList = new List<FileInfo>();
            foreach (var fileName in fileNames)
            {
                if (!DirectoryUtils.IsDirectoryExists(PathUtility.GetSiteTemplatesPath(PathUtils.GetFileNameWithoutExtension(fileName))))
                {
                    var filePath = PathUtility.GetSiteTemplatesPath(fileName);
                    var fileInfo = new FileInfo(filePath);
                    fileList.Add(fileInfo);
                }
            }
            if (fileList.Count > 0)
            {
                DgZipFiles.Visible = true;
                DgZipFiles.DataSource = fileList;
                DgZipFiles.ItemDataBound += DgZipFiles_ItemDataBound;
                DgZipFiles.DataBind();
            }
            else
            {
                DgZipFiles.Visible = false;
            }

            BtnImport.Attributes.Add("onclick", ModalImportZip.GetOpenWindowString(ModalImportZip.TypeSiteTemplate));
        }

        private void DgDirectories_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.AlternatingItem && e.Item.ItemType != ListItemType.Item) return;
            var dirInfo = (DirectoryInfo)e.Item.DataItem;

            var ltlTemplateName = (Literal)e.Item.FindControl("ltlTemplateName");
            var ltlDirectoryName = (Literal)e.Item.FindControl("ltlDirectoryName");
            var ltlDescription = (Literal)e.Item.FindControl("ltlDescription");
            var ltlCreationDate = (Literal)e.Item.FindControl("ltlCreationDate");
            var ltlDownloadUrl = (Literal)e.Item.FindControl("ltlDownloadUrl");
            var ltlCreateUrl = (Literal)e.Item.FindControl("ltlCreateUrl");
            var ltlDeleteUrl = (Literal)e.Item.FindControl("ltlDeleteUrl");

            var siteTemplateInfo = _sortedlist[dirInfo.Name] as SiteTemplateInfo;
            if (string.IsNullOrEmpty(siteTemplateInfo?.SiteTemplateName)) return;

            ltlTemplateName.Text = siteTemplateInfo.SiteTemplateName;
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
                    $@"<a href=""javascript:;"" onclick=""{ModalProgressBar.GetOpenWindowStringWithSiteTemplateZip(
                        dirInfo.Name)}"">重新压缩</a>&nbsp;&nbsp;";

                ltlDownloadUrl.Text +=
                    $@"<a href=""{PageUtility.GetSiteTemplatesUrl(fileName)}"" target=""_blank"">下载压缩包</a>";
            }
            else
            {
                ltlDownloadUrl.Text +=
                    $@"<a href=""javascript:;"" onclick=""{ModalProgressBar.GetOpenWindowStringWithSiteTemplateZip(
                        dirInfo.Name)}"">压缩</a>";
            }

            var urlAdd = PagePublishmentSystemAdd.GetRedirectUrl(dirInfo.Name);
            ltlCreateUrl.Text = $@"<a href=""{urlAdd}"">创建站点</a>";

            var urlDelete = PageUtils.GetSettingsUrl(nameof(PageSiteTemplate), new NameValueCollection
            {
                {"DeleteDirectory", "True"},
                {"SiteTemplateDir", dirInfo.Name}
            });
            ltlDeleteUrl.Text =
                $@"<a href=""{urlDelete}"" onClick=""javascript:return confirm('此操作将会删除此站点模板“{siteTemplateInfo
                    .SiteTemplateName}”，确认吗？');"">删除</a>";
        }

        private void DgZipFiles_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.AlternatingItem && e.Item.ItemType != ListItemType.Item) return;
            var fileInfo = (FileInfo)e.Item.DataItem;

            var ltlFileName = (Literal)e.Item.FindControl("ltlFileName");
            var ltlCreationDate = (Literal)e.Item.FindControl("ltlCreationDate");
            var ltlDownloadUrl = (Literal)e.Item.FindControl("ltlDownloadUrl");
            var ltlDeleteUrl = (Literal)e.Item.FindControl("ltlDeleteUrl");

            ltlFileName.Text = fileInfo.Name;
            
            ltlCreationDate.Text = DateUtils.GetDateString(fileInfo.CreationTime);

            ltlDownloadUrl.Text +=
                    $@"<a href=""javascript:;"" onclick=""{ModalProgressBar.GetOpenWindowStringWithSiteTemplateUnZip(
                        fileInfo.Name)}"">解压</a>&nbsp;&nbsp;";

            ltlDownloadUrl.Text +=
                $@"<a href=""{PageUtility.GetSiteTemplatesUrl(fileInfo.Name)}"" target=""_blank"">下载压缩包</a>";

            var urlDelete = PageUtils.GetSettingsUrl(nameof(PageSiteTemplate), new NameValueCollection
                {
                    {"DeleteZipFile", "True"},
                    {"FileName", fileInfo.Name}
                });
            ltlDeleteUrl.Text =
                $@"<a href=""{urlDelete}"" onClick=""javascript:return confirm('此操作将会删除未解压站点模板“{fileInfo.Name}”，确认吗？');"">删除</a>";
        }

    }
}
