using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.BackgroundPages.Cms;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;

namespace SiteServer.BackgroundPages.Settings
{
    public class PageSiteTemplate : BasePageCms
    {
        public Repeater RptDirectories;
        public Repeater RptZipFiles;
        public Button BtnImport;

        private SortedList _sortedlist = new SortedList();

        public static string GetRedirectUrl()
        {
            return PageUtils.GetSettingsUrl(nameof(PageSiteTemplate), null);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (AuthRequest.IsQueryExists("DeleteDirectory"))
            {
                var siteTemplateDir = AuthRequest.GetQueryString("SiteTemplateDir");

                try
                {
                    SiteTemplateManager.Instance.DeleteSiteTemplate(siteTemplateDir);

                    AuthRequest.AddAdminLog("删除站点模板", $"站点模板:{siteTemplateDir}");

                    SuccessDeleteMessage();
                }
                catch (Exception ex)
                {
                    FailDeleteMessage(ex);
                }
            }
            else if (AuthRequest.IsQueryExists("DeleteZipFile"))
            {
                var fileName = AuthRequest.GetQueryString("FileName");

                try
                {
                    SiteTemplateManager.Instance.DeleteZipSiteTemplate(fileName);

                    AuthRequest.AddAdminLog("删除未解压站点模板", $"站点模板:{fileName}");

                    SuccessDeleteMessage();
                }
                catch (Exception ex)
                {
                    FailDeleteMessage(ex);
                }
            }

            if (Page.IsPostBack) return;

            VerifySystemPermissions(ConfigManager.SettingsPermissions.Site);

            _sortedlist = SiteTemplateManager.Instance.GetSiteTemplateSortedList();
            var directoryList = new List<DirectoryInfo>();
            foreach (string directoryName in _sortedlist.Keys)
            {
                var directoryPath = PathUtility.GetSiteTemplatesPath(directoryName);
                var dirInfo = new DirectoryInfo(directoryPath);
                directoryList.Add(dirInfo);
            }

            RptDirectories.DataSource = directoryList;
            RptDirectories.ItemDataBound += RptDirectories_ItemDataBound;
            RptDirectories.DataBind();

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
                RptZipFiles.Visible = true;
                RptZipFiles.DataSource = fileList;
                RptZipFiles.ItemDataBound += RptZipFiles_ItemDataBound;
                RptZipFiles.DataBind();
            }
            else
            {
                RptZipFiles.Visible = false;
            }

            BtnImport.Attributes.Add("onclick", ModalImportZip.GetOpenWindowString(ModalImportZip.TypeSiteTemplate));
        }

        private void RptDirectories_ItemDataBound(object sender, RepeaterItemEventArgs e)
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
                var siteTemplateUrl = PageUtils.GetSiteTemplatesUrl(dirInfo.Name);
                ltlDownloadUrl.Text +=
                    $"<a href=\"{PageUtils.GetSiteTemplateMetadataUrl(siteTemplateUrl, siteTemplateInfo.PicFileName)}\" target=_blank>样图</a>&nbsp;&nbsp;";
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
                    $@"<a href=""javascript:;"" onclick=""{ModalProgressBar.GetOpenWindowStringWithSiteTemplateZip(0, dirInfo.Name)}"">重新压缩</a>&nbsp;&nbsp;";

                ltlDownloadUrl.Text +=
                    $@"<a href=""{PageUtils.GetSiteTemplatesUrl(fileName)}"" target=""_blank"">下载压缩包</a>";
            }
            else
            {
                ltlDownloadUrl.Text +=
                    $@"<a href=""javascript:;"" onclick=""{ModalProgressBar.GetOpenWindowStringWithSiteTemplateZip(0, dirInfo.Name)}"">压缩</a>";
            }

            //var urlAdd = PageSiteAdd.GetRedirectUrl(dirInfo.Name, string.Empty);
            var urlAdd = $"siteAdd.cshtml?type=create&createType=local&createTemplateId={dirInfo.Name}";
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

        private void RptZipFiles_ItemDataBound(object sender, RepeaterItemEventArgs e)
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
                    $@"<a href=""javascript:;"" onclick=""{ModalProgressBar.GetOpenWindowStringWithSiteTemplateUnZip(0, fileInfo.Name)}"">解压</a>&nbsp;&nbsp;";

            ltlDownloadUrl.Text +=
                $@"<a href=""{PageUtils.GetSiteTemplatesUrl(fileInfo.Name)}"" target=""_blank"">下载压缩包</a>";

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
